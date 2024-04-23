using System;

namespace Tactile.GardenGame.Shop
{
	public class ShopSystem
	{
		public ShopSystem(ShopManager shopManager, IShopViewFlowFactory shopViewFlowFactory, IBuyShopItemFlowFactory buyShopItemFlowFactory, CoinButtonHandler coinButtonHandler)
		{
			this.ShopManager = shopManager;
			this.ShopViewFlowFactory = shopViewFlowFactory;
			this.CoinButtonHandler = coinButtonHandler;
			this.BuyShopItemFlowFactory = buyShopItemFlowFactory;
		}

		public IShopViewFlowFactory ShopViewFlowFactory { get; private set; }

		public IBuyShopItemFlowFactory BuyShopItemFlowFactory { get; private set; }

		public ShopManager ShopManager { get; private set; }

		public CoinButtonHandler CoinButtonHandler { get; private set; }
	}
}
