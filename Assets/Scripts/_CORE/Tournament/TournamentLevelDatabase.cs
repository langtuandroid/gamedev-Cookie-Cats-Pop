using System;

public class TournamentLevelDatabase : LevelDatabase
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
		return "tournament";
	}

	public override MapIdentifier GetMapAndLevelsIdentifier()
	{
		return "Tournament";
	}

	public override string GetPersistedKey(LevelProxy levelProxy)
	{
		return "DummyEntryTournament";
	}

	public override void Save()
	{
	}

	public override ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
	{
		return new TournamentLevelDatabase.TournamentLevelAccomplishment
		{
			Stars = TournamentManager.Instance.GetStarsForDot(levelProxy.Index),
			Points = TournamentManager.Instance.GetScoreForDot(levelProxy.Index)
		};
	}

	public override void RemoveLevelData(LevelProxy levelProxy)
	{
	}

	public override double GetGateProgress(LevelProxy levelProxy)
	{
		return 0.0;
	}

	public LevelProxy GetTournamentLevel(int index, int period)
	{
		return new LevelProxy(this, new int[]
		{
			index
		});
	}

	public override int GetHumanNumber(LevelProxy levelProxy)
	{
		return levelProxy.LevelCollection.LevelStubs[levelProxy.Index].humanNumber;
	}

	public class TournamentLevelAccomplishment : ILevelAccomplishment
	{
		public int Stars { get; set; }

		public int Points { get; set; }
	}
}
