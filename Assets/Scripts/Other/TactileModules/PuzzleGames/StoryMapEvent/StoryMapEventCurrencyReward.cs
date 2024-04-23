using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.ComponentLifecycle;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventCurrencyReward : LifecycleBroadcaster
	{
		public IEnumerator PlayCurrencyCollect()
		{
			List<ItemAmount> itemAmounts = new List<ItemAmount>
			{
				new ItemAmount
				{
					ItemId = "Star",
					Amount = 1
				}
			};
			this.rewardGrid.Initialize(itemAmounts, true);
			yield return this.rewardGrid.Animate(false, false);
			yield break;
		}

		[SerializeField]
		private RewardGrid rewardGrid;
	}
}
