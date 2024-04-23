using System;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Rewards.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.Rewards
{
	public static class RewardSystemBuilder
	{
		public static RewardSystem Build(IUIController uiController, IVisualInventory visualInventory, IRewardAreaModel rewardAreaModel)
		{
			AssetModel assets = new AssetModel();
			RewardAreaController rewardAreaController = new RewardAreaController(uiController, rewardAreaModel);
			RewardsFactory rewardsFactory = new RewardsFactory(visualInventory, uiController, assets);
			return new RewardSystem(uiController, visualInventory, rewardAreaController, rewardAreaModel, assets, rewardsFactory);
		}
	}
}
