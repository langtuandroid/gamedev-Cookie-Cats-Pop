using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.ReengagementRewards;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class ReengagementRewardProvider : IReengagementDataProvider
{
	public ReengagementRewardProvider(InventoryManager inventoryManager, GateManager gateManager, GameEventManager gameEventManager)
	{
		this.inventoryManager = inventoryManager;
		this.gateManager = gateManager;
		gameEventManager.OnGameEvent += this.OnGameEvent;
	}

	public PersistableState PersistableState
	{
		get
		{
			return UserSettingsManager.Instance.GetSettings<PersistableState>();
		}
	}

	public Config Config
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>().GetConfig<Config>();
		}
	}

	public void Save()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	public void Reset()
	{
		UserSettingsManager.Instance.ResetSettings<PersistableState>();
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	public void ClaimReward(List<ItemAmount> configRewards)
	{
		foreach (ItemAmount itemAmount in configRewards)
		{
			this.inventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, "ReengagementReward");
		}
	}

	public IEnumerator UnlockCurrentGate()
	{
		this.gateManager.UnlockGate();
		FlowStack flowstack = ManagerRepository.Get<FlowStack>();
		MainMapFlow mainMapFlow = flowstack.Find<MainMapFlow>();
		yield return mainMapFlow.MapContentController.Avatars.AnimateProgressIfAny();
		mainMapFlow.MapContentController.Refresh();
		yield break;
	}

	public void UpdateGate(int levelIndex)
	{
		this.gateManager.UpdateGate();
	}

	public LevelProxy GetCurrentGate()
	{
		return this.gateManager.GetCurrentGate();
	}

	public bool IsPlayerOnGate()
	{
		return this.gateManager.PlayerOnGate;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnLevelComplete;

	private void OnGameEvent(GameEvent gameEvent)
	{
		if (gameEvent.type == 5 && this.OnLevelComplete != null)
		{
			this.OnLevelComplete(((LevelProxy)gameEvent.context).Index);
		}
	}

	private readonly InventoryManager inventoryManager;

	private readonly GateManager gateManager;
}
