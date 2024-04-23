using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.GameCore.Rewards
{
	public interface IGiveAndAnimateRewards
	{
		IEnumerator AddToInventory(List<ItemAmount> rewards, string analyticsTag);

		IEnumerator Gifts(List<ItemAmount> rewards, int giftType, string analyticsTag);
	}
}
