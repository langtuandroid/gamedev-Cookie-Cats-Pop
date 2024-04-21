using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public abstract class RewardIndicatorBase : MonoBehaviour
	{
		protected void OnClicked(UIEvent e)
		{
			if (this.fiber.IsTerminated && !this.tooltipOpen)
			{
				this.fiber.Start(this.ShowTooltip());
			}
		}

		private void Update()
		{
			if (this.fiber.IsTerminated && Input.anyKeyDown && this.tooltipOpen)
			{
				this.fiber.Start(this.HideTooltip());
			}
		}

		private void OnDestroy()
		{
			this.fiber.Terminate();
		}

		private IEnumerator ShowTooltip()
		{
			this.tooltipPivot.gameObject.SetActive(true);
			yield return FiberAnimation.ScaleTransform(this.tooltipPivot, Vector3.one * 0.01f, Vector3.one, this.tooltipCurve, this.tooltipTime);
			this.tooltipOpen = true;
			yield break;
		}

		private IEnumerator HideTooltip()
		{
			yield return FiberAnimation.ScaleTransform(this.tooltipPivot, Vector3.one, Vector3.one * 0.01f, this.tooltipCurve, this.tooltipTime);
			this.tooltipPivot.gameObject.SetActive(false);
			this.tooltipOpen = false;
			yield break;
		}

		[SerializeField]
		protected Transform tooltipPivot;

		[SerializeField]
		private AnimationCurve tooltipCurve;

		[SerializeField]
		private float tooltipTime = 0.18f;

		private Fiber fiber = new Fiber();

		private bool tooltipOpen;

		private bool closeOnClick;
	}
}
