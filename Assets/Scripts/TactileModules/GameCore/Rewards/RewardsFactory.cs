using System;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Rewards.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.Rewards
{
	public class RewardsFactory : IRewardsFactory
	{
		public RewardsFactory(IVisualInventory visualInventory, IUIController uiController, IAssetModel assets)
		{
			this.visualInventory = visualInventory;
			this.uiController = uiController;
			this.assets = assets;
		}

		public IGiveAndAnimateRewards CreateGiveAndAnimateRewards()
		{
			return new GiveAndAnimateRewards(this.visualInventory, this.uiController, this, this.assets);
		}

		public IAddBoostersToInventory CreateAddBoostersToInventory()
		{
			return new AddBoostersToInventory(this.uiController, this.assets);
		}

		private readonly IVisualInventory visualInventory;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;
	}
}
