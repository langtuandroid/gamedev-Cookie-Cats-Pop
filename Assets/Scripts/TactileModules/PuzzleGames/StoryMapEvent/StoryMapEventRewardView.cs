using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventRewardView : UIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedClaim;



		public void Initialize()
		{
			this.presentSpine.PlayAnimation(0, "locked", true, false);
		}

		public IEnumerator ClaimRewards(List<ItemAmount> items)
		{
			this.presentSpine.state.SetAnimation(0, "opening", false);
			yield return FiberHelper.Wait(0.65f, (FiberHelper.WaitFlag)0);
			this.presentSpine.state.AddAnimation(0, "open cycle", true, 0f);
			this.rewardGrid.Initialize(items, true);
			yield return this.rewardGrid.Animate(false, true);
			yield break;
		}

		[UsedImplicitly]
		private void Claim(UIEvent e)
		{
			this.ClickedClaim();
		}

		private const float CHEST_OPEN_ANIMATION_DELAY = 0.65f;

		[SerializeField]
		private RewardGrid rewardGrid;

		[SerializeField]
		private SkeletonAnimation presentSpine;
	}
}
