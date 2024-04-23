using System;

namespace TactileModules.GameCore.Rewards
{
	public interface IRewardsFactory
	{
		IGiveAndAnimateRewards CreateGiveAndAnimateRewards();

		IAddBoostersToInventory CreateAddBoostersToInventory();
	}
}
