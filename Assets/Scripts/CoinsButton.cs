using System;
using Tactile;
using UnityEngine;

public class CoinsButton : MonoBehaviour
{
	private void OnEnable()
	{
		this.RefreshCoins(null);
		InventoryManager.Instance.InventoryChanged += this.RefreshCoins;
		InventoryManager.Instance.InventoryReserveChanged += this.RefreshCoinsItem;
		this.isDirty = true;
	}

	private void OnDisable()
	{
		InventoryManager.Instance.InventoryChanged -= this.RefreshCoins;
		InventoryManager.Instance.InventoryReserveChanged -= this.RefreshCoinsItem;
	}

	private void RefreshCoins(InventoryManager.ItemChangeInfo info)
	{
		if ((info == null || info.Item == "Coin") && !this.pauseRefreshingCoins)
		{
			this.isDirty = true;
		}
	}

	private void RefreshCoinsItem(InventoryItem item)
	{
		if ((item == null || item == "Coin") && !this.pauseRefreshingCoins)
		{
			this.isDirty = true;
		}
	}

	public void PauseRefreshingCoins(bool value)
	{
		this.pauseRefreshingCoins = value;
		if (!this.pauseRefreshingCoins)
		{
			this.RefreshCoins(null);
		}
	}

	private void Update()
	{
		if (!this.isDirty)
		{
			return;
		}
		if (this.pauseRefreshingCoins)
		{
			return;
		}
		this.isDirty = false;
		this.coins.text = InventoryManager.Instance.Coins.ToString();
	}

	public UILabel coins;

	private bool pauseRefreshingCoins;

	private bool isDirty;
}
