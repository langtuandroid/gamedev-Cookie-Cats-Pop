using System;

namespace TactileModules.GameCore.Rewards.Assets
{
	public interface IAssetModel
	{
		AddToInventoryView AddToInventoryView { get; }

		RewardItem RewardItem { get; }

		GiftRewardsView GiftRewardsView { get; }

		InventoryBoostersView InventoryBoostersView { get; }
	}
}
