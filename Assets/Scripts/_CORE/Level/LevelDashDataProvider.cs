using System;
using System.Diagnostics;
using Tactile;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.LevelDash;
using TactileModules.PuzzleGames.LevelDash.Providers;

public class LevelDashDataProvider : ILevelDashDataProvider
{
	public LevelDashDataProvider()
	{
		this.GameEventManager.OnGameEvent += this.OnGameEvent;
	}

	private GameEventManager GameEventManager
	{
		get
		{
			return ManagerRepository.Get<GameEventManager>();
		}
	}

	private MainProgressionManager MainProgressionManager
	{
		get
		{
			return ManagerRepository.Get<MainProgressionManager>();
		}
	}

	public int FarthestCompletedLevelHumanNumber
	{
		get
		{
			return this.MainProgressionManager.GetFarthestCompletedLevelHumanNumber();
		}
	}

	public int MaxAvailableLevelHumanNumber
	{
		get
		{
			return this.MainProgressionManager.GetMaxAvailableLevelHumanNumber();
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action LevelCompleted;

	private void OnGameEvent(GameEvent e)
	{
		if (e.type == 5 && this.LevelCompleted != null)
		{
			this.LevelCompleted();
		}
	}

	public void ClaimReward(LevelDashConfig.Reward reward)
	{
		if (reward == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in reward.Items)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "LevelDash");
		}
	}

	public string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
	{
		return string.Format(L.Get("There are only {0} hours left of the Level Dash!"), timeSpan.TotalHours);
	}
}
