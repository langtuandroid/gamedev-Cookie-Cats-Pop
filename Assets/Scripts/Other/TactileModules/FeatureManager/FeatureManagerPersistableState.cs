using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConfigSchema;
using JetBrains.Annotations;
using Tactile;
using TactileModules.FeatureManager.Analytics;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using UnityEngine;

namespace TactileModules.FeatureManager
{
	[Description("The persisted state for the FeatureManager.")]
	[SettingsProvider("fmps", false, new Type[]
	{

	})]
	public class FeatureManagerPersistableState : IPersistableState<FeatureManagerPersistableState>, IPersistableState
	{
		public FeatureManagerPersistableState()
		{
			this.FeatureTypeDatas = new Dictionary<string, FeatureTypeData>();
			this.DeactivatedFeatures = new List<string>();
			this.DeactivatedFeaturesPermanent = new List<string>();
			this.SerializedVersion = 1;
		}

		[JsonSerializable("sv", null)]
		public int SerializedVersion { get; set; }

		[Description("Dictionary that holds all persisted data for all registered feature handlers")]
		[JsonSerializable("fid", typeof(FeatureTypeData))]
		public Dictionary<string, FeatureTypeData> FeatureTypeDatas { get; set; }

		[Description("A trimmed list of IDs of features that has been deactivated")]
		[JsonSerializable("df", typeof(string))]
		public List<string> DeactivatedFeatures { get; set; }

		[JsonSerializable("dfp", typeof(string))]
		[Description("A untrimmed list of IDs of features that has been deactivated. Only used for features without end date set.")]
		public List<string> DeactivatedFeaturesPermanent { get; set; }

		[JsonSerializable("st", null)]
		[Description("Timestamp from the server. Persisted for situations where a player starts the game on offline mode.")]
		public int ServerTimestamp { get; set; }

		public static void Initialize(IFeatureMergeUtil featureMergeUtil, IFeatureManager featureManager)
		{
			FeatureManagerPersistableState.featureMergeUtility = featureMergeUtil;
			FeatureManagerPersistableState.featureManager = featureManager;
		}

		[JsonPreSerialize]
		[UsedImplicitly]
		private void OnPreSerialize()
		{
			this.TrimDeactivatedList();
		}

		public void MergeFromOther(FeatureManagerPersistableState newest, FeatureManagerPersistableState last)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Mathf.Max(this.DeactivatedFeatures.Count, newest.DeactivatedFeatures.Count); i++)
			{
				if (i < this.DeactivatedFeatures.Count && !list.Contains(this.DeactivatedFeatures[i]))
				{
					list.Add(this.DeactivatedFeatures[i]);
				}
				if (i < newest.DeactivatedFeatures.Count && !list.Contains(newest.DeactivatedFeatures[i]))
				{
					list.Add(newest.DeactivatedFeatures[i]);
				}
			}
			this.DeactivatedFeatures = list;
			List<string> list2 = new List<string>();
			for (int j = 0; j < Mathf.Max(this.DeactivatedFeaturesPermanent.Count, newest.DeactivatedFeaturesPermanent.Count); j++)
			{
				if (j < this.DeactivatedFeaturesPermanent.Count && !list2.Contains(this.DeactivatedFeaturesPermanent[j]))
				{
					list2.Add(this.DeactivatedFeaturesPermanent[j]);
				}
				if (j < newest.DeactivatedFeaturesPermanent.Count && !list2.Contains(newest.DeactivatedFeaturesPermanent[j]))
				{
					list2.Add(newest.DeactivatedFeaturesPermanent[j]);
				}
			}
			this.DeactivatedFeaturesPermanent = list2;
			this.ServerTimestamp = Mathf.Max(this.ServerTimestamp, newest.ServerTimestamp);
			this.MergeFeatureData(newest);
		}

		private void MergeFeatureData(FeatureManagerPersistableState newest)
		{
			this.CleanStateIfInvalid(newest);
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, FeatureTypeData> keyValuePair in this.FeatureTypeDatas)
			{
				FeatureTypeData value = keyValuePair.Value;
				if (newest.FeatureTypeDatas.ContainsKey(keyValuePair.Key))
				{
					FeatureTypeData featureTypeData = newest.FeatureTypeDatas[keyValuePair.Key];
					IFeatureTypeHandler handler = this.GetHandler(keyValuePair.Key);
					if (handler == null)
					{
						continue;
					}
					List<ActivatedFeatureInstanceData> activatedFeatureInstanceDatas = FeatureManagerPersistableState.featureMergeUtility.MergeMultiple(handler, value, featureTypeData);
					value.ActivatedFeatureInstanceDatas = activatedFeatureInstanceDatas;
					FeatureTypeCustomData featureTypeCustomData = FeatureManagerPersistableState.MergeFeatureTypeCustomData(handler, value.GetFeatureTypeCustomData(), featureTypeData.GetFeatureTypeCustomData());
					value.FeatureTypeCustomData = featureTypeCustomData;
				}
				list.Add(keyValuePair.Key);
			}
			foreach (KeyValuePair<string, FeatureTypeData> keyValuePair2 in newest.FeatureTypeDatas)
			{
				if (!list.Contains(keyValuePair2.Key))
				{
					FeatureTypeData value2 = keyValuePair2.Value;
					if (this.FeatureTypeDatas.ContainsKey(keyValuePair2.Key))
					{
						FeatureTypeData featureTypeData2 = this.FeatureTypeDatas[keyValuePair2.Key];
						IFeatureTypeHandler handler2 = this.GetHandler(keyValuePair2.Key);
						if (handler2 == null)
						{
							this.FeatureTypeDatas[keyValuePair2.Key] = keyValuePair2.Value;
						}
						else
						{
							List<ActivatedFeatureInstanceData> activatedFeatureInstanceDatas2 = FeatureManagerPersistableState.featureMergeUtility.MergeMultiple(handler2, featureTypeData2, value2);
							featureTypeData2.ActivatedFeatureInstanceDatas = activatedFeatureInstanceDatas2;
							FeatureTypeCustomData featureTypeCustomData2 = FeatureManagerPersistableState.MergeFeatureTypeCustomData(handler2, featureTypeData2.GetFeatureTypeCustomData(), value2.GetFeatureTypeCustomData());
							featureTypeData2.FeatureTypeCustomData = featureTypeCustomData2;
						}
					}
					else
					{
						this.FeatureTypeDatas[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
			}
			foreach (IFeatureTypeHandler featureHandler in FeatureManagerPersistableState.featureManager.GetAllFeatureHandlers())
			{
				FeatureManagerPersistableState.featureManager.UpgradeOutdatedMetaData(featureHandler);
			}
		}

		private static FeatureTypeCustomData MergeFeatureTypeCustomData(IFeatureTypeHandler featureTypeHandler, FeatureTypeCustomData current, FeatureTypeCustomData cloud)
		{
			FeatureTypeCustomData result = FeatureHandlerInvokers.NewFeatureTypeCustomData(featureTypeHandler);
			FeatureHandlerInvokers.MergeFeatureTypeState(featureTypeHandler, ref result, current, cloud);
			return result;
		}

		public void CleanStateIfInvalid(FeatureManagerPersistableState state)
		{
			foreach (KeyValuePair<string, FeatureTypeData> keyValuePair in state.FeatureTypeDatas)
			{
				FeatureTypeData value = keyValuePair.Value;
				IFeatureTypeHandler handler = this.GetHandler(keyValuePair.Key);
				if (handler != null)
				{
					Type typeOfFeatureHandlerGeneric = FeatureHandlerInvokers.GetTypeOfFeatureHandlerGeneric(handler, typeof(FeatureTypeCustomData));
					string customDataTypeAsString = keyValuePair.Value.CustomDataTypeAsString;
					Type customDataType = keyValuePair.Value.CustomDataType;
					if (customDataType != typeOfFeatureHandlerGeneric && !string.IsNullOrEmpty(customDataTypeAsString))
					{
						value.ActivatedFeatureInstanceDatas.Clear();
						keyValuePair.Value.CustomDataTypeAsString = typeOfFeatureHandlerGeneric.AssemblyQualifiedName;
						keyValuePair.Value.FeatureTypeCustomData = FeatureHandlerInvokers.NewFeatureTypeCustomData(handler);
					}
					else if (customDataType == typeOfFeatureHandlerGeneric)
					{
						Type typeOfFeatureHandlerGeneric2 = FeatureHandlerInvokers.GetTypeOfFeatureHandlerGeneric(handler, typeof(FeatureInstanceCustomData));
						for (int i = keyValuePair.Value.ActivatedFeatureInstanceDatas.Count - 1; i >= 0; i--)
						{
							ActivatedFeatureInstanceData activatedFeatureInstanceData = keyValuePair.Value.ActivatedFeatureInstanceDatas[i];
							if (activatedFeatureInstanceData.CustomDataType != typeOfFeatureHandlerGeneric2)
							{
								keyValuePair.Value.ActivatedFeatureInstanceDatas.RemoveAt(i);
							}
						}
					}
					if (value.ActivatedFeatureInstanceDatas.Count > 1)
					{
						List<ActivatedFeatureInstanceData> activatedFeatureInstanceDatas = value.ActivatedFeatureInstanceDatas;
						IFeatureTypeHandler featureTypeHandler = activatedFeatureInstanceDatas[0].FeatureData.FeatureHandler();
						if (!featureTypeHandler.AllowMultipleFeatureInstances)
						{
							ActivatedFeatureInstanceData activatedFeatureInstanceData2 = FeatureManagerPersistableState.featureMergeUtility.MergeSingle(featureTypeHandler, activatedFeatureInstanceDatas[0], activatedFeatureInstanceDatas[1]);
							activatedFeatureInstanceDatas.Clear();
							activatedFeatureInstanceDatas.Add(activatedFeatureInstanceData2);
						}
					}
				}
			}
		}

		private IFeatureTypeHandler GetHandler(string key)
		{
			foreach (IFeatureTypeHandler featureTypeHandler in FeatureManagerPersistableState.featureManager.GetAllFeatureHandlers())
			{
				if (featureTypeHandler.FeatureType == key)
				{
					return featureTypeHandler;
				}
			}
			return null;
		}

		private void TrimDeactivatedList()
		{
			while (this.DeactivatedFeatures.Count > 50)
			{
				this.DeactivatedFeatures.RemoveAt(0);
			}
		}

		private static IFeatureMergeUtil featureMergeUtility;

		private static IFeatureManager featureManager;

		private const int MAX_DEACTIVATED_FEATURES = 50;

		public const int SERIALIZED_VERSION = 1;
	}
}
