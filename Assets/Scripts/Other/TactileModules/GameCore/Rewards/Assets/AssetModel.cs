using System;
using UnityEngine;

namespace TactileModules.GameCore.Rewards.Assets
{
	public class AssetModel : IAssetModel
	{
		public AddToInventoryView AddToInventoryView
		{
			get
			{
				return Resources.Load<AddToInventoryView>("Rewards/AddToInventoryView");
			}
		}

		public RewardItem RewardItem
		{
			get
			{
				return Resources.Load<RewardItem>("Rewards/RewardItem");
			}
		}

		public GiftRewardsView GiftRewardsView
		{
			get
			{
				return Resources.Load<GiftRewardsView>("Rewards/GiftRewardsView");
			}
		}

		public InventoryBoostersView InventoryBoostersView
		{
			get
			{
				return Resources.Load<InventoryBoostersView>("Rewards/InventoryBoostersView");
			}
		}
	}
}
