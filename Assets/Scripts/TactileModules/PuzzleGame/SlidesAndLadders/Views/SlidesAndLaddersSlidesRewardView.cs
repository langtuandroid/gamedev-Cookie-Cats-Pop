using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersSlidesRewardView : ExtensibleView<ISlidesRewardAnimation>
	{
		public Vector3 RewardsStartPosition
		{
			get
			{
				return this.spline.RewardPosition;
			}
		}

		public IEnumerator Animate(List<ItemAmount> rewards, MapStreamer mapStreamer, SlidesAndLaddersSpline spline, int preSlideIndex, int currentIndex, int chestIndex)
		{
			UICamera.DisableInput();
			this.spline = spline;
			if (base.Extension != null)
			{
				yield return base.Extension.Animate(rewards, mapStreamer, preSlideIndex, currentIndex, chestIndex);
				base.Extension.PlaySound();
			}
			base.Close(1);
			UICamera.EnableInput();
			yield break;
		}

		public void StartDimming(float duration, float sourceAlpha, float destAlpha)
		{
			this.dimmedBackgroundFiber = new Fiber(FiberBucket.Manual);
			this.dimmedBackgroundFiber.Start(FiberAnimation.Animate(duration, delegate(float t)
			{
				this.dimmedBackground.Alpha = sourceAlpha + (destAlpha - sourceAlpha) * t;
				this.dimmedBackground.gameObject.SetActive(this.dimmedBackground.Alpha > 0.0001f);
			}));
		}

		private void Update()
		{
			if (this.dimmedBackgroundFiber != null)
			{
				this.dimmedBackgroundFiber.Step();
			}
		}

		[SerializeField]
		private UISprite dimmedBackground;

		private Fiber dimmedBackgroundFiber;

		private SlidesAndLaddersSpline spline;
	}
}
