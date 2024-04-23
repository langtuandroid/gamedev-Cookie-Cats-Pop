using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public class InventoryBoostersView : ExtensibleView<IInventoryBoostersViewExtension>
	{
		public RewardGrid RewardGrid
		{
			get
			{
				return this.rewardGrid;
			}
			set
			{
				this.rewardGrid = value;
			}
		}

		public void HideRewardGrid()
		{
			if (base.Extension != null)
			{
				base.Extension.HideRewardGrid(this.rewardGridContainer);
			}
		}

		public void HideBoosterContainer()
		{
			if (base.Extension != null)
			{
				base.Extension.HideBoosterContainer();
			}
		}

		public IEnumerator AnimateRewardGridIn()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateRewardGridIn(this.rewardGridContainer);
			}
			yield break;
		}

		public IEnumerator AnimateRewardGridOut()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateRewardGridOut(this.rewardGridContainer);
			}
			yield break;
		}

		[SerializeField]
		private RewardGrid rewardGrid;

		[SerializeField]
		private GameObject rewardGridContainer;
	}
}
