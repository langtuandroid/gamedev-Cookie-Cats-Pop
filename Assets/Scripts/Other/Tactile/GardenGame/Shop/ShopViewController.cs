using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.Configuration;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewController : IFiberRunnable
	{
		public ShopViewController(ShopManager shopManager, InAppPurchaseManager inAppPurchaseManager, IUIController uiController, IAssetModel assets, ConfigGetter<ShopConfig> shopConfigProvider, ConfigGetter<LivesConfig> livesConfigProvider)
		{
			this.shopManager = shopManager;
			this.inAppPurchaseManager = inAppPurchaseManager;
			this.uiController = uiController;
			this.assets = assets;
			this.shopConfigProvider = shopConfigProvider;
			this.livesConfigProvider = livesConfigProvider;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ShopViewItem> ItemClickedForPurchase;



		public IEnumerator Run()
		{
			yield return this.uiController.ShowAndWaitForView<ShopView>(this.assets.ShopView, new Action<ShopView>(this.SetupView), null);
			yield break;
		}

		public void CloseView()
		{
			this.view.Close(0);
		}

		private void SetupView(ShopView view)
		{
			this.view = view;
			List<ShopViewItemData> list = new List<ShopViewItemData>();
			foreach (ShopItem shopItem in this.shopManager.ShopItemsWithIAPForItems("Coin"))
			{
				if (shopItem.Type.Contains("CoinPack"))
				{
					list.Add(this.CreateItemData(shopItem));
				}
			}
		}

		private ShopViewItemData CreateItemData(ShopItem shopItem)
		{
			ShopConfig shopConfig = this.shopConfigProvider.Get();
			ShopViewItemData shopViewItemData = new ShopViewItemData();
			shopViewItemData.shopItem = shopItem;
			ShopItemMetaData metaData = ShopManager.Instance.GetMetaData<ShopItemMetaData>(shopItem.Type);
			shopViewItemData.coinAmount = shopItem.CoinAmount;
			shopViewItemData.priceText = shopItem.FormattedPricePreferIAP(this.inAppPurchaseManager);
			shopViewItemData.saveAmountText = ((shopItem.CurrencyPrice <= 0) ? null : string.Format("{0}%", shopItem.CurrencyPrice));
			shopViewItemData.spriteName = metaData.ImageSpriteName;
			shopViewItemData.isBestOffer = false;
			shopViewItemData.showUnlimitedLives = (shopConfig.ShowInfiniteLifeRibbon && shopItem.CustomTag.Contains("unlimitedLives"));
			if (shopViewItemData.showUnlimitedLives)
			{
				LivesConfig livesConfig = this.livesConfigProvider.Get();
				shopViewItemData.unlimitedLivesAmount = TimeSpan.FromSeconds((double)livesConfig.InfiniteLivesDurationInSeconds).Hours;
			}
			return shopViewItemData;
		}

		private void HandleItemClickedForPurchase(ShopViewItem item)
		{
			this.ItemClickedForPurchase(item);
		}

		public void OnExit()
		{
		}

		private readonly ShopManager shopManager;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private readonly InAppPurchaseManager inAppPurchaseManager;

		private readonly ConfigGetter<ShopConfig> shopConfigProvider;

		private readonly ConfigGetter<LivesConfig> livesConfigProvider;

		private ShopView view;
	}
}
