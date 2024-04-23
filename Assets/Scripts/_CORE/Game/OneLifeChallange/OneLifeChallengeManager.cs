using System;
using System.Collections;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Foundation;
using UnityEngine;

public sealed class OneLifeChallengeManager : IFeatureTypeHandler<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureTypeHandler
{
	public OneLifeChallengeManager([NotNull] FeatureManager featureManager, [NotNull] IOneLifeChallengeProvider provider)
	{
		if (featureManager == null)
		{
			throw new ArgumentNullException("featureManager");
		}
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		this.featureManager = featureManager;
		this.provider = provider;
		this.FailedLevelProxy = LevelProxy.Invalid;
		new OneLifeChallengePopups.OneLifeChallengeStartPopup(this, featureManager);
		new OneLifeChallengePopups.OneLifeChallengeSessionStartPopup(this);
		new OneLifeChallengePopups.OneLifeChallengeEndPopup(this);
	}

	public int SecondsLeft
	{
		get
		{
			return FeatureManager.Instance.GetStabilizedTimeLeftToFeatureDurationEnd(this);
		}
	}

	public string TimeLeftAsText
	{
		get
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, this.SecondsLeft);
			int num = (int)timeSpan.TotalSeconds;
			if (num <= 0)
			{
				return L.Get((this.LevelsLeft != 1) ? "Ended" : "Last chance!");
			}
			return L.FormatSecondsAsColumnSeparated(num, L.Get("Ended"), TimeFormatOptions.None);
		}
	}

	public LevelProxy GetNextLevel
	{
		get
		{
			return this.OneLifeChallengeLevelDatabase.GetLevel(this.FarthestCompletedLevel + 1);
		}
	}

	public void LevelCompleted(LevelProxy levelProxy)
	{
		this.FailedLevelProxy = LevelProxy.Invalid;
		this.FarthestCompletedLevel = levelProxy.Index;
		PuzzleGame.UserSettings.SaveLocal();
	}

	public LevelProxy FailedLevelProxy { get; set; }

	public bool LevelFailed
	{
		get
		{
			return this.FailedLevelProxy.IsValid;
		}
	}

	public bool EventCompleted
	{
		get
		{
			return this.FarthestCompletedLevel == this.LevelCount - 1;
		}
	}

	public IEnumerator ClaimReward()
	{
		yield return this.provider.ClaimReward();
		this.DeactivateFeature();
		yield break;
	}

	public void DeactivateFeature()
	{
		FeatureManager.Instance.DeactivateFeature(this);
	}

	public bool IsActive
	{
		get
		{
			return this.HasActiveFeature() && !this.GetActivatedFeature().GetCustomInstanceData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this).RewardClaimed;
		}
	}

	public OneLifeChallengeLevelDatabase OneLifeChallengeLevelDatabase
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>().GetLevelDatabase<OneLifeChallengeLevelDatabase>("OneLifeChallenge");
		}
	}

	public OneLifeChallengeConfig Config
	{
		get
		{
			return this.provider.Config();
		}
	}

	public int FarthestCompletedLevel
	{
		get
		{
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
			if (activatedFeature == null)
			{
				return -1;
			}
			return activatedFeature.GetCustomInstanceData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this).FarthestCompletedLevel;
		}
		set
		{
			ActivatedFeatureInstanceData activatedFeature = this.GetActivatedFeature();
			if (activatedFeature == null)
			{
				return;
			}
			OneLifeChallengeInstanceCustomData customInstanceData = activatedFeature.GetCustomInstanceData<OneLifeChallengeInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>(this);
			customInstanceData.FarthestCompletedLevel = value;
			customInstanceData.Revision++;
		}
	}

	public void ResetProgress()
	{
		this.FarthestCompletedLevel = -1;
		PuzzleGame.UserSettings.SaveLocal();
	}

	private int LevelCount
	{
		get
		{
			LevelProxy levelProxy = new LevelProxy(this.OneLifeChallengeLevelDatabase, new int[1]);
			return (levelProxy.LevelMetaData as GateMetaData).levelCount;
		}
	}

	public int LevelsLeft
	{
		get
		{
			return this.LevelCount - (this.FarthestCompletedLevel + 1);
		}
	}

	public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
	{
		activatedFeatureInstanceData.FeatureInstanceActivationData.ActivationServerTimeStamp = -1;
	}

	public void FadeToBlack()
	{
	}

	public string FeatureType
	{
		get
		{
			return "one-life-challenge";
		}
	}

	public bool AllowMultipleFeatureInstances
	{
		get
		{
			return false;
		}
	}

	public int MetaDataVersion
	{
		get
		{
			return 0;
		}
	}

	public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
	{
		throw new NotSupportedException();
	}

	public OneLifeChallengeInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
	{
		return new OneLifeChallengeInstanceCustomData();
	}

	public FeatureTypeCustomData NewFeatureTypeCustomData()
	{
		return new FeatureTypeCustomData();
	}

	public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : OneLifeChallengeInstanceCustomData
	{
		if (current.Revision != cloud.Revision)
		{
			toMerge.FarthestCompletedLevel = ((current.Revision <= cloud.Revision) ? cloud.FarthestCompletedLevel : current.FarthestCompletedLevel);
		}
		else
		{
			toMerge.FarthestCompletedLevel = Mathf.Max(current.FarthestCompletedLevel, cloud.FarthestCompletedLevel);
		}
		toMerge.Revision = Mathf.Max(current.Revision, cloud.Revision);
		toMerge.RewardClaimed = (current.RewardClaimed || cloud.RewardClaimed);
	}

	public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
	{
	}

	public FeatureNotificationSettings FeatureNotificationSettings
	{
		get
		{
			return this.Config.FeatureNotificationSettings;
		}
	}

	public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
	{
		return this.provider.GetNotificationText(timeSpan, instanceData);
	}

	[NotNull]
	private readonly FeatureManager featureManager;

	public IOneLifeChallengeProvider provider;
}
