using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tactile;
using UnityEngine;

public class SpecialOutOfMovesView : UIView
{
	private ShopItem ShopItemContinueSpecial
	{
		get
		{
			return ShopManager.Instance.GetShopItem("ShopItemContinueSpecial");
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		ButtonPurchase instance = this.buyWithBoosterButton.GetInstance<ButtonPurchase>();
		instance.Title = string.Empty;
		instance.Price = this.ShopItemContinueSpecial.CurrencyPrice;
		if (parameters.Length > 1 && parameters[1] is List<InventoryItem>)
		{
			this.boosters = (List<InventoryItem>)parameters[1];
			int num = 0;
			foreach (SpecialOutOfMovesView.BoosterPivot boosterPivot in this.boosterPivots)
			{
				InventoryItemMetaData metaData = InventoryManager.Instance.GetMetaData<BoosterMetaData>(boosterPivot.booster);
				bool flag = this.boosters.Contains(boosterPivot.booster);
				boosterPivot.UpdateUI(flag);
				if (flag)
				{
					num++;
				}
			}
			this.gridLayout.numColums = num;
		}
	}

	protected override void ViewWillAppear()
	{
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
	}

	protected override void ViewGotFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = delegate()
		{
			UIViewManager.Instance.ShowView<ShopView>(new object[]
			{
				0
			});
		};
	}

	protected override void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
	}

	[UsedImplicitly]
	private void BuySpecialButton(UIEvent e)
	{
		if (base.IsClosing)
		{
			return;
		}
		UserCareManager.Instance.StartCare("Coin");
		ShopManager.Instance.TrySpendCoins(this.ShopItemContinueSpecial, this.buyWithBoosterButton, delegate(bool success)
		{
			UserCareManager.Instance.StopCare("Coin");
			if (success && this.boosters != null)
			{
				base.Close(this.boosters);
			}
		});
	}

	[UsedImplicitly]
	private void DismissButton(UIEvent e)
	{
		base.Close(0);
	}

	[Header("Buy With Booster")]
	[SerializeField]
	private UIInstantiator buyWithBoosterButton;

	[SerializeField]
	private UIGridLayout gridLayout;

	[SerializeField]
	private List<SpecialOutOfMovesView.BoosterPivot> boosterPivots;

	private CurrencyOverlay currencyOverlay;

	private List<InventoryItem> boosters;

	[Serializable]
	public class BoosterPivot
	{
		public void UpdateUI(bool active)
		{
			this.pivot.SetActive(active);
		}

		public InventoryItem booster;

		public GameObject pivot;
	}
}
