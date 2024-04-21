using System;
using TactileModules.GameCore.ButtonArea;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Shop
{
	public class CoinButtonHandler
	{
		public CoinButtonHandler(FlowStack flowStack, IButtonAreaModel buttonAreaModel, IShopViewFlowFactory shopViewFlowFactory, IAssetModel assets, IVisualInventory visualInventory)
		{
			this.flowStack = flowStack;
			this.shopViewFlowFactory = shopViewFlowFactory;
			buttonAreaModel.RegisterButton(assets.CoinButton, delegate(ButtonAreaButton b)
			{
				b.gameObject.GetComponent<InventoryLabel>().StartListen(visualInventory);
			}, delegate(ButtonAreaButton b)
			{
				b.gameObject.GetComponent<InventoryLabel>().StopListen();
			}, new Action(this.CoinButtonClicked));
		}

		private void CoinButtonClicked()
		{
			if (!this.flowStack.IsFlowInStack<ShopViewFlow>())
			{
				this.flowStack.Push(this.shopViewFlowFactory.CreateFlow(0));
			}
		}

		private readonly FlowStack flowStack;

		private readonly IShopViewFlowFactory shopViewFlowFactory;
	}
}
