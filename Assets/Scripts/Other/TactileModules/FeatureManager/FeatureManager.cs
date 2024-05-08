using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fibers;
using JetBrains.Annotations;
using Tactile;
using TactileModules.FeatureManager.Analytics;
using TactileModules.FeatureManager.Cloud;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.DummyFeature;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation.CloudSynchronization;
using TactileModules.TactileCloud.AssetBundles;
using TactileModules.Timing.Interfaces;
using UnityEngine;

namespace TactileModules.FeatureManager
{
	public class FeatureManager : IFeatureManager, ICloudSynchronizable
	{
		public FeatureManager(ITimingManager timeStampManager, IFeatureManagerProvider featureManagerProvider, IUserSettings userSettings, IFeatureAssetBundles featureAssetBundles, IFeatureUrlFileCaching featureUrlFileCaching, IFeatureAvailabilityModel featureAvailability, IFeaturesCloud featuresCloud)
		{
			if (FeatureManager.Instance != null)
			{
				throw new Exception("FeatureManager Instance already exists!");
			}
			this.featureManagerProvider = featureManagerProvider;
			this.timeStampManager = timeStampManager;
			this.assetBundles = featureAssetBundles;
			this.urlFilesCaching = featureUrlFileCaching;
			this.featureAvailabilityModel = featureAvailability;
			this.featuresCloud = featuresCloud;
			this.userSettings = userSettings;
			FeatureManager.StabilizedTime.SyncTime(this);
			this.assetBundles.AvailableAssetBundlesUpdated += this.UpdateFeatureAssetBundles;
			FeatureManager.Instance = this;
		}

		private ITimingManager TimeStampManager
		{
			get
			{
				return this.timeStampManager;
			}
		}

		private int TimePassed
		{
			get
			{
				return this.timeStampManager.GetTimePassedInSeconds("FeatureManagerTimeStampKey");
			}
		}

		private int ServerTimeStamp
		{
			get
			{
				if (this.serverTimeStamp == 0)
				{
					this.serverTimeStamp = this.featureManagerProvider.PersistableState().ServerTimestamp;
				}
				return this.serverTimeStamp;
			}
			set
			{
				this.serverTimeStamp = value;
				this.featureManagerProvider.PersistableState().ServerTimestamp = value;
			}
		}

		public int ServerTime
		{
			get
			{
				return this.ServerTimeStamp + this.TimePassed;
			}
			private set
			{
				this.timeStampManager.CreateTimeStamp("FeatureManagerTimeStampKey", int.MaxValue);
				this.ServerTimeStamp = value;
				if (FeatureManager.StabilizedTime.TimeStamp == 0)
				{
					FeatureManager.StabilizedTime.SyncTime(this);
				}
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnFeatureListUpdated;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ActivatedFeatureInstanceData> OnFeatureActivated;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ActivatedFeatureInstanceData> OnFeatureDeactivated;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action FeatureHandlersRegistered;

		public static FeatureManager Instance { get; private set; }

		public IEnumerable<IFeatureTypeHandler> GetAllFeatureHandlers()
		{
			return this.featureHandlers;
		}

		public void FadedToBlack()
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				featureTypeHandler.FadeToBlack();
			}
			FeatureManager.StabilizedTime.SyncTime(this);
			this.UpdateFeatureAvailabilityModelAndLogAnalyticsEvents();
		}

		public void UpgradeOutdatedMetaData(IFeatureTypeHandler featureHandler)
		{
			Dictionary<ActivatedFeatureInstanceData, IFeatureTypeHandler> dictionary = new Dictionary<ActivatedFeatureInstanceData, IFeatureTypeHandler>();
			List<ActivatedFeatureInstanceData> list = new List<ActivatedFeatureInstanceData>(this.GetActivatedFeaturesUnfiltered(featureHandler));
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in list)
			{
				while (activatedFeatureInstanceData.FeatureData.MetaVersion < featureHandler.MetaDataVersion)
				{
					int metaVersion = activatedFeatureInstanceData.FeatureData.MetaVersion;
					Hashtable metaData = activatedFeatureInstanceData.FeatureData.MetaData;
					Hashtable hashtable;
					try
					{
						hashtable = featureHandler.UpgradeMetaData(metaData, metaVersion, metaVersion + 1);
					}
					catch (Exception e)
					{
						this.DeactivateFeature(featureHandler, activatedFeatureInstanceData);
						break;
					}
					if (hashtable == null)
					{
						dictionary.Add(activatedFeatureInstanceData, featureHandler);
						break;
					}
					activatedFeatureInstanceData.FeatureData.MetaData = hashtable;
					activatedFeatureInstanceData.FeatureData.MetaVersion = metaVersion + 1;
				}
			}
			foreach (KeyValuePair<ActivatedFeatureInstanceData, IFeatureTypeHandler> keyValuePair in dictionary)
			{
				this.DeactivateFeature(keyValuePair.Value, keyValuePair.Key);
			}
		}

		public bool IsUpdatingFeatureList()
		{
			return !this.featureUpdateFiber.IsTerminated;
		}

		public IEnumerator WaitForFeatureListUpdate()
		{
			while (!this.featureUpdateFiber.IsTerminated)
			{
				yield return null;
			}
			yield break;
		}

		public void UpdateFeatureLists(Action<object> callback = null)
		{
			if (!this.isUpdatingFeatureLists)
			{
				this.featureUpdateFiber.Start(this.UpdateFeatureListsInternal(callback));
			}
		}

		public void RegisterFeatureHandlers([NotNull] params IFeatureTypeHandler[] featureTypeHandlers)
		{
            if (featureTypeHandlers == null)
            {
                throw new ArgumentNullException("featureTypeHandlers");
            }
            if (this.featureHandlerRegistrationCompleted)
            {
                throw new InvalidOperationException("RegisterFeatureHandlers can only be invoked once. Please registerall IFeatureTypeHandlers in one invocation");
            }
            DummyFeatureHandler featureTypeHandler = new DummyFeatureHandler(this);
            this.RegisterFeatureHandler(featureTypeHandler);
            int i = 0;
            foreach (IFeatureTypeHandler featureTypeHandler2 in featureTypeHandlers)
            {
                i++;
                this.RegisterFeatureHandler(featureTypeHandler2);
            }
            this.featureHandlerRegistrationCompleted = true;
            this.featureManagerProvider.PersistableState().CleanStateIfInvalid(this.featureManagerProvider.PersistableState());
            if (this.FeatureHandlersRegistered != null)
            {
                this.FeatureHandlersRegistered();
            }
        }

		public void AddMergeDependency(Type type)
		{
			SettingsProviderAttribute settingsProviderAttribute = (SettingsProviderAttribute)Attribute.GetCustomAttribute(typeof(FeatureManagerPersistableState), typeof(SettingsProviderAttribute));
			settingsProviderAttribute.MergeDependencies.Add(type);
		}

		public bool IsFeatureDeactivated(FeatureData featureData)
		{
			return this.IsFeatureDeactivated(featureData.Id);
		}

		public bool IsFeatureDeactivated(string id)
		{
			return this.featureManagerProvider.PersistableState().DeactivatedFeatures.Contains(id) || this.featureManagerProvider.PersistableState().DeactivatedFeaturesPermanent.Contains(id);
		}

		public int GetTimeLeftToFeatureStart(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetTimeLeftToFeatureStart(this.GetFeature(featureTypeHandler));
		}

		public int GetTimeLeftToFeatureStart(FeatureData featureData)
		{
			return (int)(featureData.StartUnixTime - (long)this.ServerTime);
		}

		public long GetTimeLeftToFeatureEnd(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetTimeLeftToFeatureEnd(this.GetFeature(featureTypeHandler));
		}

		public long GetTimeLeftToFeatureEnd(FeatureData featureData)
		{
			return featureData.CorrectedEndUnixTime - (long)this.ServerTime;
		}

		public int GetTimeLeftToFeatureActivationWindowClose(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetTimeLeftToFeatureActivationWindowClose(this.GetFeature(featureTypeHandler));
		}

		public int GetTimeLeftToFeatureActivationWindowClose(FeatureData featureData)
		{
			return (int)(featureData.CorrectedEndUnixTime - (long)Mathf.Max(featureData.MinDuration, 0) - (long)this.ServerTime);
		}

		public int GetTimePassedSinceFeatureActivation(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetTimePassedSinceFeatureActivation(this.GetActivatedFeature(featureTypeHandler));
		}

		public int GetTimePassedSinceFeatureActivation(ActivatedFeatureInstanceData state)
		{
			return this.ServerTime - state.FeatureInstanceActivationData.ActivationServerTimeStamp;
		}

		public int GetTimeLeftToFeatureDurationEnd(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetTimeLeftToFeatureDurationEnd(featureTypeHandler, this.GetActivatedFeature(featureTypeHandler));
		}

		public int GetTimeLeftToFeatureDurationEnd(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData state)
		{
			int num = (state.FeatureData.MaxDuration > 0) ? state.FeatureData.MaxDuration : int.MaxValue;
			int num2 = num - this.GetTimePassedSinceFeatureActivation(state);
			return (int)Mathf.Clamp((float)num2, 0f, Mathf.Max(0f, (float)this.GetTimeLeftToFeatureEnd(state.FeatureData)));
		}

		public int GetStabilizedTimeLeftToFeatureEnd(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetStabilizedTimeLeftToFeatureEnd(this.GetFeature(featureTypeHandler));
		}

		public int GetStabilizedTimeLeftToFeatureEnd(FeatureData featureData)
		{
			long num = featureData.CorrectedEndUnixTime - (long)FeatureManager.StabilizedTime.ServerTime;
			return (num < 2147483647L) ? ((int)num) : int.MaxValue;
		}

		public int GetStabilizedTimeLeftToFeatureActivationWindowClose(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetStabilizedTimeLeftToFeatureActivationWindowClose(this.GetFeature(featureTypeHandler));
		}

		public int GetStabilizedTimeLeftToFeatureActivationWindowClose(FeatureData featureData)
		{
			return (int)(featureData.CorrectedEndUnixTime - (long)Mathf.Max(featureData.MinDuration, 0) - (long)FeatureManager.StabilizedTime.ServerTime);
		}

		public int GetStabilizedTimePassedSinceFeatureActivation(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature(featureTypeHandler);
			if (activatedFeature == null)
			{
				return -1;
			}
			return this.GetStabilizedTimePassedSinceFeatureActivation(activatedFeature);
		}

		public int GetStabilizedTimePassedSinceFeatureActivation(ActivatedFeatureInstanceData state)
		{
			return FeatureManager.StabilizedTime.ServerTime - state.FeatureInstanceActivationData.ActivationServerTimeStamp;
		}

		public int GetStabilizedTimeLeftToFeatureDurationEnd(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature(featureTypeHandler);
			if (activatedFeature == null)
			{
				return -1;
			}
			return this.GetStabilizedTimeLeftToFeatureDurationEnd(activatedFeature);
		}

		[Obsolete("The FeatureTypeHandler parameter is not needed. Use GetStabilizedTimeLeftToFeatureDurationEnd(ActivatedFeatureInstanceData) instead")]
		public int GetStabilizedTimeLeftToFeatureDurationEnd(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData state)
		{
			return this.GetStabilizedTimeLeftToFeatureDurationEnd(state);
		}

		public int GetStabilizedTimeLeftToFeatureDurationEnd(ActivatedFeatureInstanceData state)
		{
			int num = (state.FeatureData.MaxDuration > 0) ? state.FeatureData.MaxDuration : int.MaxValue;
			int value = num - this.GetStabilizedTimePassedSinceFeatureActivation(state);
			int stabilizedTimeLeftToFeatureEnd = this.GetStabilizedTimeLeftToFeatureEnd(state.FeatureData);
			return Mathf.Clamp(value, 0, Mathf.Max(0, stabilizedTimeLeftToFeatureEnd));
		}

		public bool CanActivateFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			List<ActivatedFeatureInstanceData> activatedFeaturesUnfiltered = this.GetActivatedFeaturesUnfiltered(featureTypeHandler);
			if (activatedFeaturesUnfiltered != null && activatedFeaturesUnfiltered.Count > 0)
			{
				return false;
			}
			FeatureData availableFeature = this.GetAvailableFeature(featureTypeHandler);
			return availableFeature != null && this.CanActivateFeature(featureTypeHandler, availableFeature);
		}

		public bool CanActivateFeature(IFeatureTypeHandler featureTypeHandler, [NotNull] FeatureData featureData)
		{
			if (featureData == null)
			{
				throw new ArgumentNullException("featureData");
			}
			if (this.ServerTimeStamp == 0)
			{
				return false;
			}
			FeatureManager.AvailabilityState availabilityForFeature = this.GetAvailabilityForFeature(featureData);
			return availabilityForFeature == FeatureManager.AvailabilityState.Available && !this.IsFeatureActivated(featureTypeHandler, featureData);
		}

		public DownloadResult LoadAssetBundleFromCache(string assetBundleName)
		{
			return this.assetBundles.LoadAssetBundleFromCache(assetBundleName);
		}

		public Texture2D LoadTextureFromCache(IFeatureTypeHandler handler, string url)
		{
			return this.urlFilesCaching.LoadTextureFromCache(handler, url);
		}

		public bool IsFeatureActivated(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			return this.GetActivatedFeaturesUnfiltered(featureTypeHandler).Count > 0;
		}

		private bool IsFeatureActivated(IFeatureTypeHandler featureTypeHandler, FeatureData featureData)
		{
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.GetActivatedFeaturesUnfiltered(featureTypeHandler))
			{
				if (activatedFeatureInstanceData.Id == featureData.Id)
				{
					return true;
				}
			}
			return false;
		}

		public ActivatedFeatureInstanceData ActivateFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			FeatureData feature = this.GetFeature(featureTypeHandler);
			if (feature == null)
			{
				return null;
			}
			return this.ActivateFeature(featureTypeHandler, feature);
		}

		public ActivatedFeatureInstanceData ActivateFeature(IFeatureTypeHandler featureTypeHandler, FeatureData featureData)
		{
			if (featureData == null)
			{
				return null;
			}
			if (featureData.Type != featureTypeHandler.FeatureType)
			{
				return null;
			}
			FeatureManager.ActivationState activationStateForFeature = this.GetActivationStateForFeature(featureTypeHandler, featureData);
			if (activationStateForFeature != FeatureManager.ActivationState.None)
			{
				if (activationStateForFeature == FeatureManager.ActivationState.Activated)
				{
				}
				if (activationStateForFeature == FeatureManager.ActivationState.Deactivated)
				{
				}
				return null;
			}
			FeatureInstanceActivationData featureInstanceActivationData = new FeatureInstanceActivationData(featureData, this.ServerTime);
			FeatureInstanceCustomData featureInstanceCustomData = FeatureHandlerInvokers.NewFeatureInstanceCustomData(featureTypeHandler, featureData);
			ActivatedFeatureInstanceData activatedFeatureInstanceData = new ActivatedFeatureInstanceData(featureInstanceCustomData, featureInstanceActivationData);
			this.GetFeatureTypeData(featureTypeHandler).ActivatedFeatureInstanceDatas.Add(activatedFeatureInstanceData);
			this.Save();
			this.ScheduleNotifications(featureTypeHandler, activatedFeatureInstanceData);
			if (this.OnFeatureActivated != null)
			{
				this.OnFeatureActivated(activatedFeatureInstanceData);
			}
			return activatedFeatureInstanceData;
		}

		public bool CanActivateLocalFeature(IFeatureTypeHandler featureTypeHandler)
		{
			return (featureTypeHandler.AllowMultipleFeatureInstances || this.GetActivatedFeatureUnfiltered(featureTypeHandler) == null) && this.ServerTimeStamp != 0;
		}

		public ActivatedFeatureInstanceData ActivateLocalFeature(IFeatureTypeHandler featureTypeHandler, int duration)
		{
			Hashtable metaDataHashtable = new Hashtable();
			return this.ActivateLocalFeature(featureTypeHandler, duration, metaDataHashtable);
		}

		public ActivatedFeatureInstanceData ActivateLocalFeature<T>(IFeatureTypeHandler featureTypeHandler, int duration, T metaData) where T : FeatureMetaData
		{
			Hashtable metaDataHashtable = JsonSerializer.ObjectToHashtable(metaData);
			return this.ActivateLocalFeature(featureTypeHandler, duration, metaDataHashtable);
		}

		private ActivatedFeatureInstanceData ActivateLocalFeature(IFeatureTypeHandler featureTypeHandler, int duration, Hashtable metaDataHashtable)
		{
			if (!featureTypeHandler.AllowMultipleFeatureInstances && this.IsFeatureActivated(featureTypeHandler))
			{
				return null;
			}
			FeatureData featureData = new FeatureData
			{
				Id = "local-" + this.ServerTime.ToString(),
				MaxDuration = 0,
				MetaData = metaDataHashtable,
				MinDuration = 0,
				StartUnixTime = (long)this.ServerTime,
				Type = featureTypeHandler.FeatureType,
				MetaVersion = featureTypeHandler.MetaDataVersion
			};
			featureData.SetEndUnixTime(this.ServerTime + duration);
			featureData.MetaData["isLocal"] = true;
			return this.ActivateFeature(featureTypeHandler, featureData);
		}

		public bool ShouldDeactivateFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature(featureTypeHandler);
			return activatedFeature != null && this.ShouldDeactivateFeature(featureTypeHandler, activatedFeature);
		}

		public bool ShouldDeactivateFeature(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			return this.IsFeatureActivated(featureTypeHandler, state.FeatureData) && this.GetTimeLeftToFeatureDurationEnd(featureTypeHandler, state) <= 0;
		}

		public void DeactivateFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature(featureTypeHandler);
			if (activatedFeature == null)
			{
				return;
			}
			this.DeactivateFeature(featureTypeHandler, activatedFeature);
		}

		public void DeactivateFeature(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData activatedInstanceData)
		{
			FeatureData featureData = activatedInstanceData.FeatureData;
			if (!this.HasEndDate(featureData) && featureData.MaxDuration > 0)
			{
				this.featureManagerProvider.PersistableState().DeactivatedFeaturesPermanent.Add(activatedInstanceData.Id);
			}
			else
			{
				this.featureManagerProvider.PersistableState().DeactivatedFeatures.Add(activatedInstanceData.Id);
			}
			FeatureTypeData featureTypeData = this.GetFeatureTypeData(featureTypeHandler);
			featureTypeData.ActivatedFeatureInstanceDatas.Remove(activatedInstanceData);
			this.Save();
			this.CancelNotifications(featureTypeHandler, activatedInstanceData.FeatureData);
			if (this.OnFeatureDeactivated != null)
			{
				this.OnFeatureDeactivated(activatedInstanceData);
			}
		}

		private bool HasEndDate(FeatureData featureData)
		{
			return featureData.CorrectedEndUnixTime != 2147483647L;
		}

		public List<FeatureData> GetAvailableFeatures(IFeatureTypeHandler featureTypeHandler)
		{
			List<FeatureData> list = new List<FeatureData>();
			if (!this.IsFeatureTypeReadyToUse(featureTypeHandler))
			{
				return list;
			}
			foreach (FeatureData featureData in this.cloudAvailableFeatures)
			{
				if (featureData.Type == featureTypeHandler.FeatureType && this.GetAvailabilityForFeature(featureData) == FeatureManager.AvailabilityState.Available)
				{
					list.Add(featureData);
				}
			}
			return list;
		}

		public FeatureData GetAvailableFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			List<FeatureData> availableFeatures = this.GetAvailableFeatures(featureTypeHandler);
			return (availableFeatures.Count != 1) ? null : availableFeatures[0];
		}

		[Obsolete("Do not use this yet", true)]
		public FeatureManager.AvailabilityState GetAvailabilityForFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			FeatureData feature = this.GetFeature(featureTypeHandler);
			return (feature != null) ? this.GetAvailabilityStateForFeatureInternal(feature) : FeatureManager.AvailabilityState.Unavailable;
		}

		public FeatureManager.AvailabilityState GetAvailabilityForFeature([NotNull] FeatureData featureData)
		{
			if (featureData == null)
			{
				throw new ArgumentNullException("featureData");
			}
			return this.GetAvailabilityStateForFeatureInternal(featureData);
		}

		private FeatureManager.ActivationState GetActivationStateForFeature(IFeatureTypeHandler featureTypeHandler, [NotNull] FeatureData featureData)
		{
			if (featureData == null)
			{
				throw new ArgumentNullException("featureData");
			}
			string id = featureData.Id;
			if (this.IsFeatureDeactivated(featureData))
			{
				return FeatureManager.ActivationState.Deactivated;
			}
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.GetActivatedFeaturesUnfiltered(featureTypeHandler))
			{
				if (activatedFeatureInstanceData.Id == id)
				{
					return FeatureManager.ActivationState.Activated;
				}
			}
			return FeatureManager.ActivationState.None;
		}

		public bool HasActiveFeature(IFeatureTypeHandler featureTypeHandler)
		{
			return this.GetActivatedFeatures(featureTypeHandler).Count > 0;
		}

		public ActivatedFeatureInstanceData GetActivatedFeature(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			List<ActivatedFeatureInstanceData> activatedFeatures = this.GetActivatedFeatures(featureTypeHandler);
			return (activatedFeatures.Count != 1) ? null : activatedFeatures[0];
		}

		private ActivatedFeatureInstanceData GetActivatedFeatureUnfiltered(IFeatureTypeHandler featureTypeHandler)
		{
			this.VerifySingleInstanceFeature(featureTypeHandler);
			List<ActivatedFeatureInstanceData> activatedFeaturesUnfiltered = this.GetActivatedFeaturesUnfiltered(featureTypeHandler);
			return (activatedFeaturesUnfiltered.Count != 1) ? null : activatedFeaturesUnfiltered[0];
		}

		public ActivatedFeatureInstanceData GetActivatedFeature(IFeatureTypeHandler featureTypeHandler, string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.GetActivatedFeatures(featureTypeHandler))
			{
				if (activatedFeatureInstanceData.Id == id)
				{
					return activatedFeatureInstanceData;
				}
			}
			return null;
		}

		private bool IsFeatureTypeReadyToUse(IFeatureTypeHandler featureTypeHandler)
		{
			if (this.featureHandlers.Contains(featureTypeHandler))
			{
				return true;
			}
			string data = string.Format("Dont call GetActivatedFeatures at boot time. FeatureHandler registration is not Complete. Please fix this in Featurehandler={0}", featureTypeHandler.FeatureType);
			ClientErrorEvent clientErrorEvent = new ClientErrorEvent("GetFeaturesCalledInBoot", new StackTrace(false).ToString(), null, data, null, null, null, null, null);
			
			return false;
		}

		public List<ActivatedFeatureInstanceData> GetActivatedFeatures(IFeatureTypeHandler featureTypeHandler)
		{
			List<ActivatedFeatureInstanceData> list = new List<ActivatedFeatureInstanceData>();
			if (!this.IsFeatureTypeReadyToUse(featureTypeHandler))
			{
				return list;
			}
			List<ActivatedFeatureInstanceData> activatedFeaturesUnfiltered = this.GetActivatedFeaturesUnfiltered(featureTypeHandler);
			if (activatedFeaturesUnfiltered != null)
			{
				foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in activatedFeaturesUnfiltered)
				{
					if (this.AreFeatureAssetsAvailable(activatedFeatureInstanceData.FeatureData, featureTypeHandler))
					{
						list.Add(activatedFeatureInstanceData);
					}
				}
			}
			
			return list;
		}

		private List<ActivatedFeatureInstanceData> GetActivatedFeaturesUnfiltered(IFeatureTypeHandler featureTypeHandler)
		{
			return this.GetFeatureInstanceDatas(featureTypeHandler);
		}

		public FeatureData GetFeature(IFeatureTypeHandler featureTypeHandler)
		{
			foreach (FeatureData featureData in this.cloudAvailableFeatures)
			{
				if (featureData.Type == featureTypeHandler.FeatureType && !this.IsFeatureDeactivated(featureData))
				{
					return featureData;
				}
			}
			return null;
		}

		public IFeatureTypeHandler GetFeatureHandler(ActivatedFeatureInstanceData activatedFeatureInstanceData)
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.GetAllFeatureHandlers())
			{
				List<ActivatedFeatureInstanceData> activatedFeaturesUnfiltered = this.GetActivatedFeaturesUnfiltered(featureTypeHandler);
				foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData2 in activatedFeaturesUnfiltered)
				{
					if (activatedFeatureInstanceData2.Id == activatedFeatureInstanceData.Id)
					{
						return featureTypeHandler;
					}
				}
			}
			throw new Exception("Featurehandler not found for feature instance " + activatedFeatureInstanceData.Id);
		}

		public FeatureData GetFeature(IFeatureTypeHandler featureTypeHandler, string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			List<FeatureData> list = new List<FeatureData>(this.cloudAvailableFeatures);
			foreach (FeatureData featureData in list)
			{
				if (featureData.Id == id && !this.IsFeatureDeactivated(featureData) && featureData.Type == featureTypeHandler.FeatureType)
				{
					return featureData;
				}
			}
			return null;
		}

		public List<FeatureData> GetFeatures(IFeatureTypeHandler featureTypeHandler)
		{
			List<FeatureData> list = new List<FeatureData>();
			foreach (FeatureData featureData in this.cloudAvailableFeatures)
			{
				if (featureData.Type == featureTypeHandler.FeatureType && !this.IsFeatureDeactivated(featureData))
				{
					list.Add(featureData);
				}
			}
			return list;
		}

		public List<FeatureData> GetAllCloudAvailableFeatures()
		{
			return this.ExcludeDisabledFeaturesFromList(this.cloudAvailableFeatures);
		}

		public List<FeatureData> GetAllCloudUnavailableFeatures()
		{
			return this.ExcludeDisabledFeaturesFromList(this.cloudUnavailableFeatures);
		}

		public List<FeatureData> GetAllCloudUpcomingFeatures()
		{
			return this.ExcludeDisabledFeaturesFromList(this.cloudUpcomingFeatures);
		}

		public List<ActivatedFeatureInstanceData> GetAllActivatedFeatures()
		{
			List<ActivatedFeatureInstanceData> list = new List<ActivatedFeatureInstanceData>();
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				list.AddRange(this.GetActivatedFeatures(featureTypeHandler));
			}
			return list;
		}

		public List<ActivatedFeatureInstanceData> GetAllActivatedFeaturesUnfiltered()
		{
			List<ActivatedFeatureInstanceData> list = new List<ActivatedFeatureInstanceData>();
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				list.AddRange(this.GetActivatedFeaturesUnfiltered(featureTypeHandler));
			}
			return list;
		}

		public List<FeatureData> GetDeactivatedFeatures()
		{
			List<FeatureData> list = new List<FeatureData>();
			foreach (FeatureData featureData in this.cloudAvailableFeatures)
			{
				if (this.IsFeatureDeactivated(featureData))
				{
					list.Add(featureData);
				}
			}
			foreach (FeatureData featureData2 in this.cloudUnavailableFeatures)
			{
				if (this.IsFeatureDeactivated(featureData2))
				{
					list.Add(featureData2);
				}
			}
			foreach (FeatureData featureData3 in this.cloudUpcomingFeatures)
			{
				if (this.IsFeatureDeactivated(featureData3))
				{
					list.Add(featureData3);
				}
			}
			return list;
		}

		public List<string> GetDeactivatedFeatureIds()
		{
			FeatureManagerPersistableState featureManagerPersistableState = this.featureManagerProvider.PersistableState();
			List<string> list = new List<string>(featureManagerPersistableState.DeactivatedFeatures);
			list.AddRange(featureManagerPersistableState.DeactivatedFeaturesPermanent);
			return list;
		}

		public T GetFeatureInstanceCustomData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler, FeatureData featureData) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			ActivatedFeatureInstanceData featureInstanceData = this.GetFeatureInstanceData(featureTypeHandler, featureData);
			if (featureInstanceData == null)
			{
				return (T)((object)null);
			}
			return featureInstanceData.GetCustomInstanceData<T, U, V>(featureTypeHandler);
		}

		public U GetFeatureInstanceMetaData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler, FeatureData featureData) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			return this.GetFeatureInstanceMetaData<U>(featureData);
		}

		public U GetFeatureInstanceMetaData<U>(FeatureData featureData) where U : FeatureMetaData
		{
			if (!this.deserializedMetaData.ContainsKey(featureData.MetaData))
			{
				this.deserializedMetaData.Add(featureData.MetaData, JsonSerializer.HashtableToObject(typeof(U), featureData.MetaData, JsonSerializer.SerializationType.WithPreAndPostCallbacks));
			}
			return (U)((object)this.deserializedMetaData[featureData.MetaData]);
		}

		public V GetFeatureTypeCustomData<T, U, V>(IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			FeatureTypeData featureTypeData = this.GetFeatureTypeData(featureTypeHandler);
			if (featureTypeData == null)
			{
				return (V)((object)null);
			}
			return featureTypeData.GetFeatureTypeCustomData<T, U, V>(featureTypeHandler);
		}

		public FeatureTypeCustomData GetFeatureTypeCustomData(IFeatureTypeHandler featureTypeHandler)
		{
			FeatureTypeData featureTypeData = this.GetFeatureTypeData(featureTypeHandler);
			if (featureTypeData == null)
			{
				return null;
			}
			return featureTypeData.GetFeatureTypeCustomData();
		}

		public T GetFeatureTypeCustomData<T>(IFeatureTypeHandler featureTypeHandler) where T : class
		{
			FeatureTypeData featureTypeData = this.GetFeatureTypeData(featureTypeHandler);
			if (featureTypeData == null)
			{
				return (T)((object)null);
			}
			return featureTypeData.GetFeatureTypeCustomData() as T;
		}

		public FeatureInstanceActivationData GetFeatureInstanceActivationData(IFeatureTypeHandler featureTypeHandler, FeatureData featureData)
		{
			ActivatedFeatureInstanceData featureInstanceData = this.GetFeatureInstanceData(featureTypeHandler, featureData);
			if (featureInstanceData == null)
			{
				return null;
			}
			return featureInstanceData.FeatureInstanceActivationData;
		}

		public FeatureTypeData GetFeatureTypeData(IFeatureTypeHandler featureTypeHandler)
		{
			FeatureManagerPersistableState featureManagerPersistableState = this.featureManagerProvider.PersistableState();
			if (!featureManagerPersistableState.FeatureTypeDatas.ContainsKey(featureTypeHandler.FeatureType))
			{
				return null;
			}
			return featureManagerPersistableState.FeatureTypeDatas[featureTypeHandler.FeatureType];
		}

		public List<ActivatedFeatureInstanceData> GetFeatureInstanceDatas(IFeatureTypeHandler featureTypeHandler)
		{
			FeatureTypeData featureTypeData = this.GetFeatureTypeData(featureTypeHandler);
			if (featureTypeData == null)
			{
				return null;
			}
			return featureTypeData.ActivatedFeatureInstanceDatas;
		}

		public ActivatedFeatureInstanceData GetFeatureInstanceData(IFeatureTypeHandler featureTypeHandler, FeatureData featureData)
		{
			FeatureTypeData featureTypeData = this.GetFeatureTypeData(featureTypeHandler);
			if (featureTypeData == null)
			{
				return null;
			}
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in featureTypeData.ActivatedFeatureInstanceDatas)
			{
				if (activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.Id == featureData.Id)
				{
					return activatedFeatureInstanceData;
				}
			}
			return null;
		}

		public static T GetFeatureHandler<T>() where T : IFeatureTypeHandler
		{
			foreach (IFeatureTypeHandler featureTypeHandler in FeatureManager.Instance.featureHandlers)
			{
				if (featureTypeHandler is T)
				{
					return (T)((object)featureTypeHandler);
				}
			}
			return default(T);
		}

		public string GetCommaSeperatedListOfActiveFeaturesIds()
		{
			List<string> list = new List<string>();
			List<ActivatedFeatureInstanceData> allActivatedFeatures = this.GetAllActivatedFeatures();
			for (int i = 0; i < allActivatedFeatures.Count; i++)
			{
				list.Add(allActivatedFeatures[i].Id);
			}
			return string.Join(",", list.ToArray());
		}

		public string GetCommaSeperatedListOfActiveFeatureTypes()
		{
			List<string> list = new List<string>();
			List<ActivatedFeatureInstanceData> allActivatedFeatures = this.GetAllActivatedFeatures();
			for (int i = 0; i < allActivatedFeatures.Count; i++)
			{
				list.Add(allActivatedFeatures[i].FeatureData.Type);
			}
			return string.Join(",", list.ToArray());
		}

		public List<string> GetAllMainMapActiveFeatureTypes()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.GetAllActivatedFeatures())
			{
				Type type = this.GetFeatureHandler(activatedFeatureInstanceData).GetType();
				if (Attribute.GetCustomAttribute(type, typeof(MainMapFeatureAttribute)) != null)
				{
					hashSet.Add(activatedFeatureInstanceData.FeatureData.Type);
				}
			}
			return hashSet.ToList<string>();
		}

		private void RegisterFeatureHandler([NotNull] IFeatureTypeHandler featureTypeHandler)
		{
            if (featureTypeHandler == null)
            {
                throw new ArgumentNullException("featureTypeHandler");
            }

            if (!this.featureHandlers.Contains(featureTypeHandler))
            {
                this.featureHandlers.Add(featureTypeHandler);
                if (this.GetFeatureTypeData(featureTypeHandler) == null)
                {
                    FeatureTypeCustomData customData = FeatureHandlerInvokers.NewFeatureTypeCustomData(featureTypeHandler);
                    FeatureTypeData value = new FeatureTypeData(customData);
                    this.featureManagerProvider.PersistableState().FeatureTypeDatas.Add(featureTypeHandler.FeatureType, value);
                }
                this.UpgradeOutdatedMetaData(featureTypeHandler);
            }
        }

		private void VerifySingleInstanceFeature(IFeatureTypeHandler featureTypeHandler)
		{
			if (featureTypeHandler.AllowMultipleFeatureInstances)
			{
				throw new InvalidOperationException("Trying to get persisted state for a multi-instance feature through the single instance method. Please use a different override!");
			}
			FeatureTypeData featureTypeData = featureTypeHandler.GetFeatureTypeData();
			if (featureTypeData.ActivatedFeatureInstanceDatas.Count > 1)
			{
				throw new InvalidOperationException("More than one instance detected in a single-instance feature! This should never happen!");
			}
		}

		private void Save()
		{
			this.userSettings.SaveLocalSettings();
		}

		private List<FeatureData> ExcludeDisabledFeaturesFromList(List<FeatureData> featureDatas)
		{
			List<FeatureData> list = new List<FeatureData>();
			foreach (FeatureData featureData in featureDatas)
			{
				if (!this.IsFeatureDeactivated(featureData))
				{
					list.Add(featureData);
				}
			}
			return list;
		}

		private void ScheduleNotifications(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData activatedFeature)
		{
			IFeatureNotifications featureNotifications = featureTypeHandler as IFeatureNotifications;
			if (featureNotifications == null)
			{
				return;
			}
			if (featureNotifications.FeatureNotificationSettings == null)
			{
				return;
			}
			FeatureData featureData = activatedFeature.FeatureData;
			int timeLeftToFeatureDurationEnd = this.GetTimeLeftToFeatureDurationEnd(featureTypeHandler, activatedFeature);
			foreach (int num in featureNotifications.FeatureNotificationSettings.GetNotificationTimes())
			{
				if (timeLeftToFeatureDurationEnd > num)
				{
					int num2 = this.ServerTime + timeLeftToFeatureDurationEnd - num;
					TimeSpan timeSpan = new TimeSpan(0, 0, num);
					string notificationText = featureNotifications.GetNotificationText(timeSpan, activatedFeature);
					string noteId = string.Format("{0}_expires_{1}", featureData.Id, num);
					LocalNotificationManager.Schedule(noteId, new LocalNotificationManager.Notification
					{
						WhenEpochSeconds = (double)num2,
						AlertBody = notificationText,
						AlertAction = L.Get("play now"),
						BadgeCount = 1
					});
				}
			}
		}

		private void CancelNotifications(IFeatureTypeHandler featureTypeHandler, FeatureData featureData)
		{
			IFeatureNotifications featureNotifications = featureTypeHandler as IFeatureNotifications;
			if (featureNotifications == null)
			{
				return;
			}
			if (featureNotifications.FeatureNotificationSettings == null)
			{
				return;
			}
			foreach (int num in featureNotifications.FeatureNotificationSettings.GetNotificationTimes())
			{
				string noteId = string.Format("{0}_expires_{1}", featureData.Id, num);
				LocalNotificationManager.Cancel(noteId);
			}
		}

		private FeatureManager.AvailabilityState GetAvailabilityStateForFeatureInternal(FeatureData featureData)
		{
			int num = (int)featureData.StartUnixTime;
			long correctedEndUnixTime = featureData.CorrectedEndUnixTime;
			long num2 = correctedEndUnixTime - (long)Mathf.Max(featureData.MinDuration, 0);
			if (this.ServerTime < num)
			{
				return FeatureManager.AvailabilityState.Upcoming;
			}
			if (this.ServerTime <= num || (long)this.ServerTime >= num2)
			{
				return FeatureManager.AvailabilityState.Unavailable;
			}
			bool flag = false;
			foreach (FeatureData featureData2 in this.cloudAvailableFeatures)
			{
				if (featureData2.Id == featureData.Id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return FeatureManager.AvailabilityState.Unavailable;
			}
			if (this.IsFeatureDeactivated(featureData))
			{
				return FeatureManager.AvailabilityState.Unavailable;
			}
			if (!this.AreFeatureAssetsAvailable(featureData, featureData.FeatureHandler()))
			{
				return FeatureManager.AvailabilityState.Unavailable;
			}
			return FeatureManager.AvailabilityState.Available;
		}

		public void ServerTimeUpdated(int serverTime)
		{
			this.ServerTime = serverTime;
		}

		public void ApplicationWillEnterForeground()
		{
			this.cloudUpcomingFeatures.Clear();
			this.cloudAvailableFeatures.Clear();
			this.cloudUnavailableFeatures.Clear();
			this.deserializedMetaData.Clear();
			FeatureManager.StabilizedTime.SyncTime(this);
			GC.Collect();
		}

		private IEnumerator UpdateFeatureListsInternal(Action<object> callback = null)
		{
			if (this.isUpdatingFeatureLists)
			{
				yield break;
			}
			this.isUpdatingFeatureLists = true;
			yield return this.featuresCloud.RefreshFeatures(this, delegate(object err, List<FeatureData> availableFeatures, List<FeatureData> upcomingFeatures, List<FeatureData> unavailableFeatures)
			{
				if (err == null)
				{
					this.OnReceiveFeaturesFromServer(availableFeatures, upcomingFeatures, unavailableFeatures);
					if (callback != null)
					{
						callback(null);
					}
				}
				else if (callback != null)
				{
					callback(err);
				}
			});
			this.isUpdatingFeatureLists = false;
			yield break;
		}

		private void UpdateFeatureAssets()
		{
			this.UpdateFeatureAssetBundles();
			this.UpdateFeatureUrlFiles();
		}

		private void UpdateFeatureAssetBundles()
		{
			this.DownloadFeatureAssetBundles();
			this.HideFeaturesBasedOnAssetBundleState();
		}

		private void UpdateFeatureUrlFiles()
		{
			this.DeleteUnusedUrlFiles();
			this.CacheFeatureUrlFiles();
			this.HideFeaturesBasedOnUrlFilesState();
		}

		private void DownloadFeatureAssetBundles()
		{
			this.assetBundles.DownloadAssetBundlesForFeatures(this.cloudAvailableFeatures);
		}

		private void CacheFeatureUrlFiles()
		{
			this.urlFilesCaching.CacheUrlFilesForFeatures(this.cloudAvailableFeatures, this.featureHandlers);
		}

		private void HideFeaturesBasedOnAssetBundleState()
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				IFeatureAssetBundleHandler featureAssetBundleHandler = featureTypeHandler as IFeatureAssetBundleHandler;
				if (featureAssetBundleHandler != null)
				{
					foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.GetActivatedFeaturesUnfiltered(featureTypeHandler))
					{
						if (!this.assetBundles.AreAssetBundlesForFeatureDownloaded(activatedFeatureInstanceData.FeatureData))
						{
							featureAssetBundleHandler.FeatureInstanceWasHidden(activatedFeatureInstanceData);
						}
					}
				}
			}
		}

		private void HideFeaturesBasedOnUrlFilesState()
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				IFeatureUrlFileHandler featureUrlFileHandler = featureTypeHandler as IFeatureUrlFileHandler;
				if (featureUrlFileHandler != null)
				{
					foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.GetActivatedFeaturesUnfiltered(featureTypeHandler))
					{
						if (!this.urlFilesCaching.AreUrlFilesForFeatureInstanceCached(activatedFeatureInstanceData.FeatureData, featureTypeHandler))
						{
							featureUrlFileHandler.FeatureInstanceWasHidden(activatedFeatureInstanceData);
						}
					}
				}
			}
		}

		private void DeleteUnusedUrlFiles()
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				List<FeatureData> allFeatures = this.GetAllFeatures(featureTypeHandler);
				this.urlFilesCaching.DeleteUnusedUrlFiles(allFeatures, featureTypeHandler);
			}
		}

		private List<FeatureData> GetAllFeatures(IFeatureTypeHandler featureTypeHandler)
		{
			List<FeatureData> list = new List<FeatureData>();
			foreach (FeatureData featureData in this.cloudAvailableFeatures)
			{
				if (featureData.Type == featureTypeHandler.FeatureType)
				{
					list.Add(featureData);
				}
			}
			List<ActivatedFeatureInstanceData> activatedFeatures = this.GetActivatedFeatures(featureTypeHandler);
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in activatedFeatures)
			{
				bool flag = false;
				foreach (FeatureData featureData2 in list)
				{
					if (featureData2.Id == activatedFeatureInstanceData.Id)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					list.Add(activatedFeatureInstanceData.FeatureData);
				}
			}
			return list;
		}

		private void UpdateFeatureAvailabilityModelAndLogAnalyticsEvents()
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				foreach (FeatureData featureData in this.GetFeatures(featureTypeHandler))
				{
					bool isAvailable = this.GetAvailabilityForFeature(featureData) == FeatureManager.AvailabilityState.Available;
					bool areFeatureAssetsAvailable = this.AreFeatureAssetsAvailable(featureData, featureTypeHandler);
					this.featureAvailabilityModel.UpdateFeatureAvailabilityAndLogAnalyticsEvent(featureData, isAvailable, areFeatureAssetsAvailable);
				}
			}
			foreach (string featureId in this.GetDeactivatedFeatureIds())
			{
				this.featureAvailabilityModel.EnsureFeatureIsRemoved(featureId);
			}
		}

		private bool AreFeatureAssetsAvailable(FeatureData featureData, IFeatureTypeHandler featureTypeHandler)
		{
			return this.assetBundles.AreAssetBundlesForFeatureDownloaded(featureData) && this.urlFilesCaching.AreUrlFilesForFeatureInstanceCached(featureData, featureTypeHandler);
		}

		private void OnReceiveFeaturesFromServer(List<FeatureData> availableFeatures, List<FeatureData> upcomingFeatures, List<FeatureData> unavailableFeatures)
		{
			this.cloudAvailableFeatures = availableFeatures;
			this.cloudUpcomingFeatures = upcomingFeatures;
			this.cloudUnavailableFeatures = unavailableFeatures;
			this.RemoveMultipleInstancesOfSingleInstanceFeatures();
			availableFeatures.Sort((FeatureData a, FeatureData b) => (a.StartUnixTime >= b.StartUnixTime) ? 1 : -1);
			upcomingFeatures.Sort((FeatureData a, FeatureData b) => (a.StartUnixTime >= b.StartUnixTime) ? 1 : -1);
			unavailableFeatures.Sort((FeatureData a, FeatureData b) => (a.StartUnixTime >= b.StartUnixTime) ? 1 : -1);
			
			this.UpdateFeatureDatasInFeatureHandlers();
			this.UpdateFeatureAssets();
		}

		private void RemoveMultipleInstancesOfSingleInstanceFeatures()
		{
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				if (!featureTypeHandler.AllowMultipleFeatureInstances)
				{
					List<FeatureData> availableFeatures = this.GetAvailableFeatures(featureTypeHandler);
					if (availableFeatures.Count > 1)
					{
						this.RemoveAvailableInstances(featureTypeHandler);
					}
				}
			}
		}

		private void RemoveAvailableInstances(IFeatureTypeHandler feature)
		{
			this.cloudAvailableFeatures.RemoveAll((FeatureData data) => data.Type == feature.FeatureType);
		}

		private void UpdateFeatureDatasInFeatureHandlers()
		{
			List<FeatureData> list = new List<FeatureData>();
			list.AddRange(this.cloudAvailableFeatures);
			list.AddRange(this.cloudUnavailableFeatures);
			list.AddRange(this.cloudUpcomingFeatures);
			foreach (IFeatureTypeHandler featureTypeHandler in this.featureHandlers)
			{
				for (int i = this.GetFeatureInstanceDatas(featureTypeHandler).Count - 1; i >= 0; i--)
				{
					ActivatedFeatureInstanceData activatedFeatureInstanceData = this.GetFeatureInstanceDatas(featureTypeHandler)[i];
					bool flag = false;
					foreach (FeatureData featureData in list)
					{
						if (activatedFeatureInstanceData.Id == featureData.Id && activatedFeatureInstanceData.FeatureData.Type == featureData.Type)
						{
							activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData = featureData;
							this.ScheduleNotifications(featureTypeHandler, activatedFeatureInstanceData);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Hashtable metaData = activatedFeatureInstanceData.FeatureData.MetaData;
						if (metaData == null || !metaData.ContainsKey("isLocal") || !(bool)metaData["isLocal"])
						{
							featureTypeHandler.FeatureRemovedFromDashboard(activatedFeatureInstanceData);
						}
					}
				}
			}

			this.Save();
			if (this.OnFeatureListUpdated != null)
			{
				this.OnFeatureListUpdated();
			}
		}

		public IEnumerator Synchronize()
		{
			yield return this.UpdateFeatureListsInternal(null);
			yield break;
		}

		private const string TIME_STAMP_KEY = "FeatureManagerTimeStampKey";

		private readonly Fiber featureUpdateFiber = new Fiber();

		private readonly IFeatureManagerProvider featureManagerProvider;

		private readonly ITimingManager timeStampManager;

		private readonly IFeatureAssetBundles assetBundles;

		private readonly IFeatureUrlFileCaching urlFilesCaching;

		private readonly IFeatureAvailabilityModel featureAvailabilityModel;

		private readonly IFeaturesCloud featuresCloud;

		private readonly IUserSettings userSettings;

		private readonly List<IFeatureTypeHandler> featureHandlers = new List<IFeatureTypeHandler>();

		private readonly Dictionary<Hashtable, object> deserializedMetaData = new Dictionary<Hashtable, object>();

		private bool featureHandlerRegistrationCompleted;

		private bool isUpdatingFeatureLists;

		private int serverTimeStamp;

		private List<FeatureData> cloudUpcomingFeatures = new List<FeatureData>();

		private List<FeatureData> cloudAvailableFeatures = new List<FeatureData>();

		private List<FeatureData> cloudUnavailableFeatures = new List<FeatureData>();

		public enum ActivationState
		{
			None,
			Activated,
			Deactivated
		}

		public enum AvailabilityState
		{
			Upcoming,
			Available,
			Unavailable
		}

		public static class StabilizedTime
		{
			public static int TimeStamp { get; private set; }

			private static int TimePassed
			{
				get
				{
					return FeatureManager.Instance.TimeStampManager.GetTimePassedInSeconds("FeatureManagerStabilizedTimeKey");
				}
			}

			public static int ServerTime
			{
				get
				{
					return FeatureManager.StabilizedTime.TimeStamp + FeatureManager.StabilizedTime.TimePassed;
				}
			}

			public static void SyncTime(FeatureManager featureManager)
			{
				FeatureManager.StabilizedTime.TimeStamp = featureManager.ServerTime;
				featureManager.TimeStampManager.CreateTimeStamp("FeatureManagerStabilizedTimeKey", int.MaxValue);
			}

			private const string STABILIZED_TIME_STAMP_KEY = "FeatureManagerStabilizedTimeKey";
		}
	}
}
