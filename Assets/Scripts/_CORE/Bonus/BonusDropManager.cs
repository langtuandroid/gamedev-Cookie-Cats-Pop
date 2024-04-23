using System;
using System.Collections;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class BonusDropManager : Singleton<BonusDropManager>, MapPopupManager.IMapPopup
{
	public BonusDropManager()
	{
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	public void TryShowPopup(int unlockedLevelId, MapPopupManager.PopupFlow flow)
	{
		if (this.ArePresentsEnabled && this.PrizeAvailable)
		{
			flow.AddPopup(this.ShowPopup());
		}
	}

	private IEnumerator ShowPopup()
	{
		UIViewManager.UIViewStateGeneric<BonusDropInfoView> vs = UIViewManager.Instance.ShowView<BonusDropInfoView>(new object[0]);
		yield return vs.WaitForClose();
		yield break;
	}

	public bool ArePresentsEnabled
	{
		get
		{
			BonusDropConfig bonusDropConfig = ConfigurationManager.Get<BonusDropConfig>();
			int farthestUnlockedLevelHumanNumber = MainProgressionManager.Instance.GetFarthestUnlockedLevelHumanNumber();
			return farthestUnlockedLevelHumanNumber >= bonusDropConfig.LevelRequired && bonusDropConfig.ItemsRequiredForPrize > 0 && bonusDropConfig.SpawnIntervalInMoves > 0;
		}
	}

	public int DropsRequiredForPrize
	{
		get
		{
			return this.Config.ItemsRequiredForPrize;
		}
	}

	private BonusDropConfig Config
	{
		get
		{
			return ConfigurationManager.Get<BonusDropConfig>();
		}
	}

	private BonusDropManager.PersistableState GetState()
	{
		return UserSettingsManager.Instance.GetSettings<BonusDropManager.PersistableState>();
	}

	public int DropsCollected
	{
		get
		{
			return InventoryManager.Instance.GetAmount("BonusDrop");
		}
	}

	public void CollectDrop()
	{
		InventoryManager.Instance.Add("BonusDrop", 1, "CollectDrop");
	}

	public bool PrizeAvailable
	{
		get
		{
			return this.DropsCollected >= this.DropsRequiredForPrize;
		}
	}

	public float NormalizedPrizeProgress
	{
		get
		{
			int itemsRequiredForPrize = ConfigurationManager.Get<BonusDropConfig>().ItemsRequiredForPrize;
			if (itemsRequiredForPrize == 0)
			{
				return 0f;
			}
			return (float)this.DropsCollected / (float)itemsRequiredForPrize;
		}
	}

	public ItemAmount ClaimPrize()
	{
		if (!this.PrizeAvailable)
		{
			return null;
		}
		Lottery<ItemAmount> lottery = new Lottery<ItemAmount>();
		foreach (ItemAmount itemAmount in this.Config.RewardPool)
		{
			if (!BoosterManagerBase<BoosterManager>.Instance.IsInventoryItemABooster(itemAmount.ItemId) || BoosterManagerBase<BoosterManager>.Instance.IsBoosterUnlocked(itemAmount.ItemId))
			{
				lottery.Add(1f, itemAmount);
			}
		}
		ItemAmount itemAmount2;
		if (lottery.Count > 0)
		{
			itemAmount2 = lottery.PickRandomItem(false);
		}
		else
		{
			itemAmount2 = this.Config.RewardPool.GetRandom<ItemAmount>();
		}
		if (itemAmount2 != null)
		{
			InventoryManager.Instance.Add(itemAmount2.ItemId, itemAmount2.Amount, "ClaimBonusDropPrize");
			InventoryManager.Instance.Consume("BonusDrop", this.DropsRequiredForPrize, "ClaimBonusDropPrize");
		}
		return itemAmount2;
	}

	public void MarkMove()
	{
		this.GetState().Moves++;
	}

	public void ResetMoves()
	{
		this.GetState().Moves = 0;
	}

	public bool CanSpawnItem
	{
		get
		{
			return this.GetState().Moves >= this.Config.SpawnIntervalInMoves && this.DropsCollected < this.Config.ItemsRequiredForPrize;
		}
	}

	[SettingsProvider("pr", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<BonusDropManager.PersistableState>, IPersistableState
	{
		[JsonSerializable("se", null)]
		public bool HasSeenExplanation { get; set; }

		[JsonSerializable("mo", null)]
		public int Moves { get; set; }

		public void MergeFromOther(BonusDropManager.PersistableState newState, BonusDropManager.PersistableState lastCloudState)
		{
			this.HasSeenExplanation = (this.HasSeenExplanation || newState.HasSeenExplanation);
			this.Moves = Mathf.Max(this.Moves, newState.Moves);
		}
	}
}
