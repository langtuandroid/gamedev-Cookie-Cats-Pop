using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public interface IInventoryBoostersViewExtension
	{
		void HideBoosterContainer();

		void HideRewardGrid(GameObject rewardGridContainer);

		IEnumerator AnimateRewardGridIn(GameObject rewardGridContainer);

		IEnumerator AnimateRewardGridOut(GameObject rewardGridContainer);
	}
}
