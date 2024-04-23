using System;
using Tactile.GardenGame.Shop;
using TactileModules.GameCore.ButtonArea;

namespace TactileModules.GameCore.Shop.Assets
{
	public interface IAssetModel
	{
		ButtonAreaButton CoinButton { get; }

		ShopView ShopView { get; }

		ShopViewItem ShopViewItem { get; }

		Tactile.GardenGame.Shop.BuyShopItemView BuyShopItemView { get; }
	}
}
