using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGames.EndlessChallenge;
using TactileModules.PuzzleGames.EndlessChallenge.Data;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class EndlessCheckpoint : MonoBehaviour
{
	public bool HasCollectedReward { get; set; }

	public int Row { get; set; }

	public GameBoard GameBoard { get; set; }

	private EndlessChallengeHandler EndlessChallengeHandler
	{
		get
		{
			return FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
		}
	}

	public void HandleAspectRatioSizes(float pillarSize, float aspectRatioScale)
	{
		float num = base.GetComponent<UIElement>().Size.x / 2f;
		this.rowLabel.GetElement().Size = new Vector2(pillarSize * 0.9f, this.rowLabel.GetElement().Size.y);
		this.rowLabel.GetElement().LocalPosition = new Vector2(-num - pillarSize / 2f, this.rowLabel.GetElement().LocalPosition.y);
		this.dottedConnector.Size = new Vector2(Mathf.Abs(this.rowLabel.GetElement().LocalPosition.x) - num, this.dottedConnector.Size.y);
		this.dottedConnector.transform.SetParent(this.rowLabel.transform);
		this.dottedConnector.LocalPosition = new Vector2(this.dottedConnector.Size.x / 2f, 0f);
		this.rewardInventory.LocalPosition = new Vector2(num + pillarSize / 2f - pillarSize * (1f - aspectRatioScale), this.rewardInventory.GetElement().LocalPosition.y);
		this.rewardInventory.transform.localScale *= aspectRatioScale;
	}

	public void AlignGameObjectToRow()
	{
		int num = this.GameBoard.TotalRows - this.Row;
		int num2 = (num % 2 != 0) ? 13 : 0;
		base.gameObject.transform.localPosition = Vector3.up * ((float)(-(float)this.GameBoard.Topology.GetCoordFromIndex(23 * (num / 2) + num2).y) * this.GameBoard.TileSize * BubbleTopology.LINE_DIST_FACTOR);
		base.gameObject.transform.localPosition = new Vector3(base.gameObject.transform.localPosition.x, base.gameObject.transform.localPosition.y, 100f);
		this.rowLabel.text = this.Row.ToString();
	}

	public void SetReward(EndlessChallengeCheckpointData endlessChallengeCheckpointData)
	{
		this.SetupInventoryBonuses(endlessChallengeCheckpointData.InventoryBonuses);
		this.SetupIngameBonuses(endlessChallengeCheckpointData.EndlessChallengeBonuses);
		int numberOfBonuses = endlessChallengeCheckpointData.InventoryBonuses.Count + endlessChallengeCheckpointData.EndlessChallengeBonuses.Count;
		this.AdjustSizeOfRewardGrid(numberOfBonuses);
	}

	private void SetupInventoryBonuses(List<ItemAmount> inventoryBonuses)
	{
		if (inventoryBonuses.Count > 0)
		{
			this.inventoryItems = inventoryBonuses;
			this.rewardGrid.Initialize(inventoryBonuses, false);
		}
	}

	private void SetupIngameBonuses(List<EndlessChallengeBonus> inGameBonuses)
	{
		this.ingameRewardItems = new List<EndlessCheckpoint.IngameRewardItems>();
		foreach (EndlessChallengeBonus endlessChallengeBonus in inGameBonuses)
		{
			EndlessChallengeBonusInfo endlessChallengeBonusInfo = this.EndlessChallengeHandler.GetEndlessChallengeBonusInfo(endlessChallengeBonus.Type);
			RewardItem rewardItem = UnityEngine.Object.Instantiate<RewardItem>(this.rewardItem);
			rewardItem.label.text = endlessChallengeBonus.Amount.ToString();
			rewardItem.transform.parent = this.rewardGrid.transform;
			rewardItem.transform.localPosition = Vector3.zero;
			rewardItem.icon.SpriteName = endlessChallengeBonusInfo.checkpointSprite;
			this.ingameRewardItems.Add(new EndlessCheckpoint.IngameRewardItems(rewardItem, endlessChallengeBonusInfo.logicPrefab, endlessChallengeBonus.Amount));
		}
	}

	private void AdjustSizeOfRewardGrid(int numberOfBonuses)
	{
		if (this.rewardGridPivotElement == null)
		{
			this.rewardGridPivotElement = this.rewardGrid.transform.parent.GetComponent<UIElement>();
			this.rewardGridPivotElementInitSize = this.rewardGridPivotElement.Size;
		}
		this.rewardGridPivotElement.Size = new Vector2(this.rewardGridPivotElementInitSize.x, this.rewardGridPivotElementInitSize.y * (float)numberOfBonuses);
	}

	private IEnumerator AnimateInGameBonuses(LevelSession levelSession)
	{
		foreach (EndlessCheckpoint.IngameRewardItems ingameRewardItem in this.ingameRewardItems)
		{
			yield return ingameRewardItem.GiveReward(levelSession);
		}
		yield break;
	}

	public IEnumerator AnimateCollect(LevelSession levelSession)
	{
		foreach (ItemAmount itemAmount in this.inventoryItems)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "EndlessChallenge checkpoint");
			this.HandleRewardStats(levelSession, itemAmount);
		}
		yield return this.AnimateInGameBonuses(levelSession);
		if (this.inventoryItems.Count > 0)
		{
			yield return this.rewardGrid.Animate(false, true);
		}
		this.HasCollectedReward = true;
		yield break;
	}

	private void HandleRewardStats(LevelSession levelSession, ItemAmount reward)
	{
		if (reward.ItemId == "BoosterRainbow")
		{
			levelSession.EndlessChallengeStats.RainbowBoosterReceived(reward.Amount);
		}
		else if (reward.ItemId == "BoosterFinalPower")
		{
			levelSession.EndlessChallengeStats.FinalPowerBoosterReceived(reward.Amount);
		}
		else if (reward.ItemId == "Coin")
		{
			levelSession.EndlessChallengeStats.CoinsReceived(reward.Amount);
		}
	}

	[SerializeField]
	private UILabel rowLabel;

	[SerializeField]
	private RewardItem rewardItem;

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private UIElement dottedConnector;

	[SerializeField]
	private UIElement rewardInventory;

	private List<ItemAmount> inventoryItems = new List<ItemAmount>();

	private List<EndlessCheckpoint.IngameRewardItems> ingameRewardItems;

	private UIElement rewardGridPivotElement;

	private Vector2 rewardGridPivotElementInitSize;

	private class IngameRewardItems
	{
		public IngameRewardItems(RewardItem rewardItem, BoosterLogic boosterLogic, int amount)
		{
			this.rewardItem = rewardItem;
			this.boosterLogic = boosterLogic;
			this.amount = amount;
		}

		public IEnumerator GiveReward(LevelSession levelSession)
		{
			this.rewardItem.gameObject.SetActive(false);
			if (this.boosterLogic is VariableExtraMovesLogic)
			{
				((VariableExtraMovesLogic)this.boosterLogic).variableExtraMoves = this.amount;
				levelSession.EndlessChallengeStats.GotExtraMoves(this.amount);
				this.amount = 1;
			}
			else if (this.boosterLogic is CookieJarBoosterLogic)
			{
				levelSession.EndlessChallengeStats.CookieJarSmallReceived();
			}
			else if (this.boosterLogic is CookieJarFillUpBoosterLogic)
			{
				levelSession.EndlessChallengeStats.CookieJarFillUpReceived();
			}
			for (int i = 0; i < this.amount; i++)
			{
				yield return BoosterLogic.ResolveBooster(this.boosterLogic, levelSession, UIViewManager.Instance.FindView<GameView>());
			}
			yield break;
		}

		private RewardItem rewardItem;

		private BoosterLogic boosterLogic;

		private int amount;
	}
}
