using System;
using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.MainLevels;

[MainMapFeature]
public sealed class HardLevelsManager : IFeatureTypeHandler<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>, IFeatureTypeHandler
{
	public HardLevelsManager([NotNull] FeatureManager featureManager, [NotNull] IHardLevelsProvider hardLevelsProvider, MainProgressionManager mainProgressionManager)
	{
		if (featureManager == null)
		{
			throw new ArgumentNullException("featureManager");
		}
		if (hardLevelsProvider == null)
		{
			throw new ArgumentNullException("hardLevelsProvider");
		}
		this.featureManager = featureManager;
		this.hardLevelsProvider = hardLevelsProvider;
		this.mainProgressionManager = mainProgressionManager;
		new MainMapHardLevelsCompletedPopup(hardLevelsProvider, mainProgressionManager, this);
		new MainMapHardLevelsExpiredPopup(hardLevelsProvider, this);
		new MainMapHardLevelsReachedPopup(hardLevelsProvider, this);
		new MainMapHardLevelsReminderPopup(hardLevelsProvider, this);
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnHardLevelsChanged;

	public bool ShouldShowStartPopup(LevelProxy levelProxy)
	{
		if (!this.CanShowPopups(levelProxy, HardLevelsManager.RangeMatchFilter.Any))
		{
			return false;
		}
		ActivatedFeatureInstanceData activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any);
		return activationDataForLevelInRange != null && !activationDataForLevelInRange.GetCustomInstanceData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).ShowedStartedPopup;
	}

	public bool ShouldShowCompletedPopup(LevelProxy levelProxy)
	{
		if (!this.CanShowPopups(levelProxy, HardLevelsManager.RangeMatchFilter.End))
		{
			return false;
		}
		ActivatedFeatureInstanceData activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.End);
		int toHumanNumber = activationDataForLevelInRange.GetMetaData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).LevelRange.ToHumanNumber;
		int farthestCompletedLevelHumanNumber = this.mainProgressionManager.GetFarthestCompletedLevelHumanNumber();
		return farthestCompletedLevelHumanNumber >= toHumanNumber;
	}

	public bool ShouldShowExpiredPopup(LevelProxy levelProxy)
	{
		return this.CanShowPopups(levelProxy, HardLevelsManager.RangeMatchFilter.Any) && this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any) != null && this.IsFeatureExpired(levelProxy);
	}

	public bool ShouldEndFeatureSilently(LevelProxy levelProxy)
	{
		return !this.ShouldShowExpiredPopup(levelProxy) && this.IsFeatureExpired(levelProxy);
	}

	public bool ShouldShowReminderPopup(LevelProxy levelProxy)
	{
		ActivatedFeatureInstanceData activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any);
		return activationDataForLevelInRange != null && activationDataForLevelInRange.GetCustomInstanceData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).ShowedStartedPopup && this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(this, activationDataForLevelInRange) > 0;
	}

	public ActivatedFeatureInstanceData GetActivationDataForLevelInRange(LevelProxy levelProxy, HardLevelsManager.RangeMatchFilter matchFilter = HardLevelsManager.RangeMatchFilter.Any)
	{
		if (!levelProxy.IsValid)
		{
			return null;
		}
		foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in this.featureManager.GetActivatedFeatures(this))
		{
			HardLevelsMetaData metaData = activatedFeatureInstanceData.GetMetaData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this);
			if (!(metaData.LevelDatabaseContext != levelProxy.AnalyticsDescriptors[0]))
			{
				HardLevelsMetaData.HardLevelRange levelRange = metaData.LevelRange;
				if (matchFilter != HardLevelsManager.RangeMatchFilter.Any)
				{
					if (matchFilter != HardLevelsManager.RangeMatchFilter.Start)
					{
						if (matchFilter == HardLevelsManager.RangeMatchFilter.End)
						{
							if (levelProxy.HumanNumber == levelRange.ToHumanNumber)
							{
								return activatedFeatureInstanceData;
							}
						}
					}
					else if (levelProxy.HumanNumber == levelRange.FromHumanNumber)
					{
						return activatedFeatureInstanceData;
					}
				}
				else if (levelProxy.HumanNumber >= levelRange.FromHumanNumber && levelProxy.HumanNumber <= levelRange.ToHumanNumber)
				{
					return activatedFeatureInstanceData;
				}
			}
		}
		return null;
	}

	public bool IsLevelHard(LevelProxy levelProxy)
	{
		return this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any) != null;
	}

	public DateTime GetExpirationDateForHardLevels(ActivatedFeatureInstanceData instanceData)
	{
		DateTime result = new DateTime(1970, 1, 1, 0, 0, 0);
		result = result.AddSeconds((double)instanceData.FeatureData.CorrectedEndUnixTime);
		return result;
	}

	public void MarkHardLevelsAsStarted(ActivatedFeatureInstanceData instanceData)
	{
		instanceData.GetCustomInstanceData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).ShowedStartedPopup = true;
		this.StartFeatureTimeFromNow(instanceData);
		PuzzleGame.UserSettings.SaveLocal();
	}

	public void DeactivateHardLevels(LevelProxy levelProxy, HardLevelsView.EventState eventState)
	{
		ActivatedFeatureInstanceData activationDataForLevelInRange;
		if (eventState == HardLevelsView.EventState.Completed)
		{
			activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.End);
		}
		else
		{
			if (eventState != HardLevelsView.EventState.Expired)
			{
				throw new Exception("Hard levels should only deactivate due to completion or expiration");
			}
			activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any);
		}
		this.featureManager.DeactivateFeature(this, activationDataForLevelInRange);
		if (this.OnHardLevelsChanged != null)
		{
			this.OnHardLevelsChanged();
		}
	}

	private void StartFeatureTimeFromNow(ActivatedFeatureInstanceData activatedFeatureInstanceData)
	{
		activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = this.featureManager.ServerTime;
	}

	private bool IsFeatureExpired(LevelProxy levelProxy)
	{
		ActivatedFeatureInstanceData activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any);
		return activationDataForLevelInRange != null && this.featureManager.ShouldDeactivateFeature(this, activationDataForLevelInRange);
	}

	private bool TryDeactivateSilently()
	{
		bool result = false;
		for (int i = this.featureManager.GetActivatedFeatures(this).Count - 1; i >= 0; i--)
		{
			ActivatedFeatureInstanceData activatedFeatureInstanceData = this.featureManager.GetActivatedFeatures(this)[i];
			if (this.featureManager.ShouldDeactivateFeature(this, activatedFeatureInstanceData))
			{
				if (PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber < activatedFeatureInstanceData.GetMetaData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).LevelRange.FromHumanNumber)
				{
					if (this.featureManager.GetTimeLeftToFeatureEnd(activatedFeatureInstanceData.FeatureData) > 0L && activatedFeatureInstanceData.FeatureData.MaxDuration != 0 && !activatedFeatureInstanceData.GetCustomInstanceData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).ShowedStartedPopup)
					{
						return false;
					}
					this.featureManager.DeactivateFeature(this, activatedFeatureInstanceData);
					result = true;
				}
			}
		}
		return result;
	}

	private bool TryActivate()
	{
		bool result = false;
		foreach (FeatureData featureData in this.featureManager.GetFeatures(this))
		{
			if (this.featureManager.CanActivateFeature(this, featureData))
			{
				bool flag = PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber > featureData.GetMetaData(this).LevelRange.ToHumanNumber;
				if (!flag)
				{
					this.featureManager.ActivateFeature(this, featureData);
					result = true;
				}
			}
		}
		return result;
	}

	private bool CanShowPopups(LevelProxy levelProxy, HardLevelsManager.RangeMatchFilter filter)
	{
		if (!this.hardLevelsProvider.Config().ShowLevelDifficulty)
		{
			return false;
		}
		ActivatedFeatureInstanceData activationDataForLevelInRange = this.GetActivationDataForLevelInRange(levelProxy, filter);
		return activationDataForLevelInRange != null && activationDataForLevelInRange.GetMetaData<HardLevelsInstanceCustomData, HardLevelsMetaData, FeatureTypeCustomData>(this).ShowInfoViews;
	}

	private void RefreshFeatures()
	{
		bool flag = false;
		flag |= this.TryActivate();
		flag |= this.TryDeactivateSilently();
		if (flag && this.OnHardLevelsChanged != null)
		{
			this.OnHardLevelsChanged();
		}
	}

	public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
	{
		activatedFeatureInstanceData.FeatureInstanceActivationData.ActivatedFeatureData.SetEndUnixTime(1);
	}

	public void FadeToBlack()
	{
		this.RefreshFeatures();
	}

	public string FeatureType
	{
		get
		{
			return "hard-levels";
		}
	}

	public bool AllowMultipleFeatureInstances
	{
		get
		{
			return true;
		}
	}

	public int MetaDataVersion
	{
		get
		{
			return 3;
		}
	}

	public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
	{
		if (fromVersion == 0 && toVersion == 1)
		{
			return metaData;
		}
		if (fromVersion == 1 && toVersion == 2)
		{
			if (metaData.ContainsKey("levelRanges"))
			{
				Hashtable hashtable = new Hashtable
				{
					{
						"LevelDatabaseContext",
						metaData["levelDatabaseContext"]
					},
					{
						"ShowInfoViews",
						metaData["showInfoViews"]
					}
				};
				ArrayList arrayList = (ArrayList)metaData["levelRanges"];
				Hashtable hashtable2 = arrayList[0] as Hashtable;
				Hashtable value = new Hashtable
				{
					{
						"IndexFrom",
						Convert.ToInt32(hashtable2["indexFrom"])
					},
					{
						"IndexTo",
						Convert.ToInt32(hashtable2["indexTo"])
					}
				};
				hashtable.Add("LevelRange", value);
				return hashtable;
			}
			if (metaData.ContainsKey("LevelRange"))
			{
				return metaData;
			}
			throw new Exception("Current meta data did not contain expected data");
		}
		else
		{
			if (fromVersion == 2 && toVersion == 3)
			{
				Hashtable hashtable3 = new Hashtable
				{
					{
						"LevelDatabaseContext",
						metaData["LevelDatabaseContext"]
					},
					{
						"ShowInfoViews",
						metaData["ShowInfoViews"]
					}
				};
				Hashtable hashtable4 = metaData["LevelRange"] as Hashtable;
				int index = Convert.ToInt32(hashtable4["IndexFrom"]);
				int index2 = Convert.ToInt32(hashtable4["IndexTo"]);
				int humanNumber = this.hardLevelsProvider.GetHumanNumber(index);
				int humanNumber2 = this.hardLevelsProvider.GetHumanNumber(index2);
				Hashtable value2 = new Hashtable
				{
					{
						"FromHumanNumber",
						humanNumber
					},
					{
						"ToHumanNumber",
						humanNumber2
					}
				};
				hashtable3.Add("LevelRange", value2);
				return hashtable3;
			}
			throw new NotSupportedException();
		}
	}

	public HardLevelsInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
	{
		return new HardLevelsInstanceCustomData();
	}

	public FeatureTypeCustomData NewFeatureTypeCustomData()
	{
		return new FeatureTypeCustomData();
	}

	public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : HardLevelsInstanceCustomData
	{
		toMerge.ShowedStartedPopup = (current.ShowedStartedPopup || cloud.ShowedStartedPopup);
	}

	public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
	{
		if (this.OnHardLevelsChanged != null)
		{
			this.OnHardLevelsChanged();
		}
	}

	private readonly FeatureManager featureManager;

	private readonly IHardLevelsProvider hardLevelsProvider;

	private readonly MainProgressionManager mainProgressionManager;

	public enum RangeMatchFilter
	{
		Any,
		Start,
		End
	}
}
