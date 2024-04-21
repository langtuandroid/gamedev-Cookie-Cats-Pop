using System;

namespace Tactile.GardenGame.Shop
{
	public interface IShopViewFlowFactory
	{
		ShopViewFlow CreateFlow(int coinsNeeded);
	}
}
