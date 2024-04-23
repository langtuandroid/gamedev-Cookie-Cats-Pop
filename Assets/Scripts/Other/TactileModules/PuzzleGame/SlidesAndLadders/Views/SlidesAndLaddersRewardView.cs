using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersRewardView : ExtensibleView<IRewardAnimation>
	{
		public Action OnViewDidAppear { get; set; }

		public void Initialize(List<ItemAmount> rewards)
		{
			this.claimed = false;
			base.Extension.Initialize(rewards);
		}

		protected override void ViewDidAppear()
		{
			if (this.OnViewDidAppear != null)
			{
				this.OnViewDidAppear();
			}
		}

		private void SetClaimButtonEnabled(bool enabled)
		{
			this.ClaimButtonDisabled.gameObject.SetActive(!enabled);
			this.ClaimButton.gameObject.SetActive(enabled);
		}

		[UsedImplicitly]
		private void Claim(UIEvent e)
		{
			if (!this.claimed)
			{
				this.claimed = true;
				this.animationFiber.Start(this.Animate());
				if (base.Extension != null)
				{
					base.Extension.PlaySound();
				}
			}
		}

		private IEnumerator Animate()
		{
			UICamera.DisableInput();
			this.SetClaimButtonEnabled(false);
			if (base.Extension != null)
			{
				yield return base.Extension.Animate();
			}
			base.Close(1);
			UICamera.EnableInput();
			yield break;
		}

		[SerializeField]
		private UIInstantiator ClaimButton;

		[SerializeField]
		private UIInstantiator ClaimButtonDisabled;

		private bool claimed;

		private readonly Fiber animationFiber = new Fiber();
	}
}
