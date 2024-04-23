using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.SlidesAndLadders;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using UnityEngine;

public class SlidesAndLaddersLevelDatabase : LevelDatabase, ISlidesAndLaddersLevelDatabase
{
	public SlidesAndLaddersHandler Handler
	{
		get
		{
			return FeatureManager.GetFeatureHandler<SlidesAndLaddersHandler>();
		}
	}

	public List<LevelConnection> LevelConnections
	{
		get
		{
			return this.levelConnections;
		}
	}

	public ILevelProxy NextLevel(ILevelProxy current, bool isLevelCompleted)
	{
		return this.levelConnections[current.Index].NextLevel(current, isLevelCompleted);
	}

	public bool IsTreasureLevel(int levelIndex)
	{
		return this.levelConnections[levelIndex].IsTreasureLevel;
	}

	public bool IsSlideLevel(int index)
	{
		return this.levelConnections[index].FailedLevelSteps < 0;
	}

	public bool IsLadderLevel(int index)
	{
		return this.levelConnections[index].CompletedLevelSteps > 0;
	}

	public bool IsNeitherSlideOrLadderLevel(int index)
	{
		return this.levelConnections[index].CompletedLevelSteps == 0 && this.levelConnections[index].FailedLevelSteps == 0;
	}

	public int GetChestRank(int index)
	{
		int num = 0;
		for (int i = 0; i < this.levelConnections.Count; i++)
		{
			if (this.levelConnections[i].IsTreasureLevel)
			{
				num++;
				if (i == index)
				{
					return num;
				}
			}
		}
		return 0;
	}

	public override int NumberOfAvailableLevels
	{
		get
		{
			return this.levelConnections.Count;
		}
	}

	public int NumberOfLevels
	{
		get
		{
			return this.NumberOfAvailableLevels;
		}
	}

	public bool IsChestClaimed(int index)
	{
		return this.Handler.InstanceCustomData.RewardsClaimed.Contains(index);
	}

	public override LevelProxy GetLevel(int levelIndex)
	{
		if (levelIndex < 0)
		{
			return LevelProxy.Invalid;
		}
		return new LevelProxy(this, new int[]
		{
			levelIndex
		});
	}

	public ILevelProxy GetLevelProxy(int levelIndex)
	{
		return this.GetLevel(levelIndex);
	}

	public override string GetAnalyticsDescriptor()
	{
		return "slides-and-ladders";
	}

	public override MapIdentifier GetMapAndLevelsIdentifier()
	{
		return "SlidesAndLadders";
	}

	public override string GetPersistedKey(LevelProxy levelProxy)
	{
		return "DummyEntrySlidesAndLadders";
	}

	public override void Save()
	{
		this.Handler.Save();
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

	public override int GetHumanNumber(LevelProxy levelProxy)
	{
		return levelProxy.LevelCollection.LevelStubs[levelProxy.Index].humanNumber;
	}

	[SerializeField]
	[Hashable(null)]
	private List<LevelConnection> levelConnections = new List<LevelConnection>();
}
