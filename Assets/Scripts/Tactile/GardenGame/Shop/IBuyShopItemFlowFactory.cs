using System;

namespace Tactile.GardenGame.Shop
{
	public interface IBuyShopItemFlowFactory
	{
		BuyShopItemFlow CreateFlow(string shopItemIdentifier);
	}
}
