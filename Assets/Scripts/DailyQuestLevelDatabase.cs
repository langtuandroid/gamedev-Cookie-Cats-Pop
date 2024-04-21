using System;

public class DailyQuestLevelDatabase : LevelDatabase
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
		return "daily";
	}

	public override MapIdentifier GetMapAndLevelsIdentifier()
	{
		return "Daily";
	}

	public override string GetPersistedKey(LevelProxy levelProxy)
	{
		return "DummyEntryDaily";
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

	public override double GetGateProgress(LevelProxy levelProxy)
	{
		return 0.0;
	}
}
