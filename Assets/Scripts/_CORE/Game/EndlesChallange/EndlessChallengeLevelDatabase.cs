using System;

public class EndlessChallengeLevelDatabase : LevelDatabase
{
	public override LevelProxy GetLevel(int levelIndex)
	{
		return new LevelProxy(this, new int[]
		{
			levelIndex
		});
	}

	public override string GetAnalyticsDescriptor()
	{
		return "endless";
	}

	public override MapIdentifier GetMapAndLevelsIdentifier()
	{
		return "EndlessChallenge";
	}

	public override string GetPersistedKey(LevelProxy levelProxy)
	{
		return "DummyEntryEndlessChallenge";
	}

	public override void Save()
	{
	}

	public override ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
	{
		return null;
	}

	public override void RemoveLevelData(LevelProxy levelProxy)
	{
	}

	public int FarthestCompleteLevelID
	{
		get
		{
			int num = 0;
			while (this.GetLevel(num).IsCompleted)
			{
				num++;
				if (num > this.NumberOfAvailableLevels - 1)
				{
					return this.NumberOfAvailableLevels - 1;
				}
			}
			return num - 1;
		}
	}

	public override double GetGateProgress(LevelProxy levelProxy)
	{
		GateMetaData gateMetaData = levelProxy.LevelMetaData as GateMetaData;
		if (gateMetaData != null)
		{
			return 0.1;
		}
		return 0.0;
	}

	public int FarthestUnlockedLevelID
	{
		get
		{
			int farthestCompleteLevelID = this.FarthestCompleteLevelID;
			if (farthestCompleteLevelID >= this.NumberOfAvailableLevels - 1)
			{
				return this.NumberOfAvailableLevels - 1;
			}
			return farthestCompleteLevelID + 1;
		}
	}

	public const int NO_LEVEL = -1;

	private int loadedLevelGroup;

	public GateAsset[] dummyGates;
}
