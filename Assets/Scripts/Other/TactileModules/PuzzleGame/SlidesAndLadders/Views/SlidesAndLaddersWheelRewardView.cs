using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersWheelRewardView : ExtensibleView<IRewardAnimation>
	{
		public void Initialize(List<ItemAmount> rewards)
		{
			base.Extension.Initialize(rewards);
			this.animationFiber.Start(this.Animate());
		}

		private IEnumerator Animate()
		{
			UICamera.DisableInput();
			if (base.Extension != null)
			{
				base.Extension.PlaySound();
				yield return base.Extension.Animate();
			}
			base.Close(1);
			UICamera.EnableInput();
			yield break;
		}

		private readonly Fiber animationFiber = new Fiber();
	}
}
