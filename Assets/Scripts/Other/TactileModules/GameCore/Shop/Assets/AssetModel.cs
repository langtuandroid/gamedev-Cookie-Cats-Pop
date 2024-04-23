using System;
using Tactile.GardenGame.Shop;
using TactileModules.GameCore.ButtonArea;
using UnityEngine;

namespace TactileModules.GameCore.Shop.Assets
{
	public class AssetModel : IAssetModel
	{
		public ButtonAreaButton CoinButton
		{
			get
			{
				return Resources.Load<ButtonAreaButton>("Shop/CoinButton");
			}
		}

		public ShopView ShopView
		{
			get
			{
				return Resources.Load<ShopView>("Shop/ShopView");
			}
		}

		public ShopViewItem ShopViewItem
		{
			get
			{
				return Resources.Load<ShopViewItem>("Shop/ShopViewItem");
			}
		}

		public Tactile.GardenGame.Shop.BuyShopItemView BuyShopItemView
		{
			get
			{
				return Resources.Load<Tactile.GardenGame.Shop.BuyShopItemView>("Shop/BuyShopItemView");
			}
		}
	}
}
