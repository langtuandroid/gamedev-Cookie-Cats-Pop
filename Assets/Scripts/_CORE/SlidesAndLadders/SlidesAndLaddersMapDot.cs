using System;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.SlidesAndLadders;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using UnityEngine;

public abstract class SlidesAndLaddersMapDot : MapDotBase
{
	[Instantiator.SerializeProperty]
	public override int LevelId
	{
		get
		{
			return this.levelId;
		}
		set
		{
			this.levelId = value;
			if (Application.isPlaying)
			{
				this.UpdateUI();
			}
		}
	}

	public bool IsSlideLevel()
	{
		return this.Handler.LevelDatabase.IsSlideLevel(this.levelId);
	}

	public bool IsLadderLevel()
	{
		return this.Handler.LevelDatabase.IsLadderLevel(this.levelId);
	}

	public bool IsNeitherSlideOrLadderLevel()
	{
		return this.Handler.LevelDatabase.IsNeitherSlideOrLadderLevel(this.levelId);
	}

	private SlidesAndLaddersHandler Handler
	{
		get
		{
			return FeatureManager.GetFeatureHandler<SlidesAndLaddersHandler>();
		}
	}

	protected ILevelProxy LevelProxy
	{
		get
		{
			return this.Handler.LevelDatabase.GetLevelProxy(this.LevelId);
		}
	}

	public override bool IsUnlocked
	{
		get
		{
			return this.levelId == this.Handler.InstanceCustomData.FarthestUnlockedLevelIndex;
		}
	}

	public override bool IsCompleted
	{
		get
		{
			return this.levelId < this.Handler.InstanceCustomData.FarthestUnlockedLevelIndex;
		}
	}

	protected bool IsActiveTreasureLevel()
	{
		return this.Handler.LevelDatabase.IsTreasureLevel(this.levelId) && !this.Handler.InstanceCustomData.RewardsClaimed.Contains(this.levelId);
	}

	protected bool IsCompletedLevel()
	{
		return this.Handler.InstanceCustomData.CurrentLevelIndex == this.levelId && this.Handler.InstanceCustomData.State == ResultState.Completed;
	}

	public override void Initialize()
	{
	}

	private int levelId;

	private SlidesAndLaddersSystem system;
}
