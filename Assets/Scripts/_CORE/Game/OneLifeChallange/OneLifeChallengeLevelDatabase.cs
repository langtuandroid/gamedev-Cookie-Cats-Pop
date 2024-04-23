using System;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

public class OneLifeChallengeLevelDatabase : LevelDatabase
{
	private static OneLifeChallengeManager Manager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<OneLifeChallengeManager>();
		}
	}

	public override LevelProxy GetLevel(int levelIndex)
	{
		ActivatedFeatureInstanceData activatedFeature = FeatureManager.Instance.GetActivatedFeature(OneLifeChallengeLevelDatabase.Manager);
		if (activatedFeature == null)
		{
			LevelProxy levelProxy = new LevelProxy(this, new int[1]);
			return levelProxy.CreateChildProxy(levelIndex);
		}
		int num = Mathf.Abs(activatedFeature.Id.GetHashCode());
		int num2 = num % this.NumberOfAvailableLevels;
		LevelProxy levelProxy2 = new LevelProxy(this, new int[]
		{
			num2
		});
		return levelProxy2.CreateChildProxy(levelIndex);
	}

	public override string GetAnalyticsDescriptor()
	{
		return "oneLifeChallenge";
	}

	public override MapIdentifier GetMapAndLevelsIdentifier()
	{
		return "OneLifeChallenge";
	}

	public override string GetPersistedKey(LevelProxy levelProxy)
	{
		return "DummyEntryOneLifeChallenge";
	}

	public override void Save()
	{
		PuzzleGame.UserSettings.SaveLocal();
	}

	public override ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
	{
		int index = levelProxy.Index;
		int farthestCompletedLevel = OneLifeChallengeLevelDatabase.Manager.FarthestCompletedLevel;
		LevelAccomplishment levelAccomplishment = new LevelAccomplishment();
		if (index <= farthestCompletedLevel)
		{
			levelAccomplishment.Points = 1;
		}
		return levelAccomplishment;
	}

	public override void RemoveLevelData(LevelProxy levelProxy)
	{
	}

	public override double GetGateProgress(LevelProxy levelProxy)
	{
		return 0.0;
	}

	public override int GetHumanNumber(LevelProxy levelProxy)
	{
		return levelProxy.LevelCollection.LevelStubs[levelProxy.Index].humanNumber;
	}
}
