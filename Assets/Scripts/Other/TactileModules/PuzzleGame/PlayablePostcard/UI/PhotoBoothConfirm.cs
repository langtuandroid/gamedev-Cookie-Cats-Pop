using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
	public class PhotoBoothConfirm : PhotoBoothVisualState
	{
		public void Initialize(AnimationCurve animationMoveCurve)
		{
			this.originalContinueTextPos = this.continueText.transform.localPosition;
			this.originalRetryButtonScale = this.retryButton.transform.localScale;
			this.originalConfirmButtonScale = this.confirmButton.transform.localScale;
			this.moveCurve = animationMoveCurve;
		}

		public override IEnumerator AnimateIn()
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.AnimateContinueTextIn(),
				this.AnimateRetryButtonIn(),
				this.AnimateConfirmButtonIn()
			});
			yield break;
		}

		public override IEnumerator AnimateOut()
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.AnimateContinueTextOut(),
				this.AnimateRetryButtonOut(),
				this.AnimateConfirmButtonOut()
			});
			yield break;
		}

		private IEnumerator AnimateContinueTextIn()
		{
			yield return FiberAnimation.MoveLocalTransform(this.continueText.transform, this.originalContinueTextPos + new Vector3(0f, 400f, 0f), this.originalContinueTextPos, this.moveCurve, 0.4f);
			yield break;
		}

		private IEnumerator AnimateContinueTextOut()
		{
			yield return FiberAnimation.MoveLocalTransform(this.continueText.transform, this.originalContinueTextPos, this.originalContinueTextPos + new Vector3(0f, 400f, 0f), null, 0.2f);
			yield break;
		}

		private IEnumerator AnimateRetryButtonIn()
		{
			yield return FiberAnimation.ScaleTransform(this.retryButton.transform, Vector3.zero, this.originalRetryButtonScale, null, 0.2f);
			yield break;
		}

		private IEnumerator AnimateRetryButtonOut()
		{
			yield return FiberAnimation.ScaleTransform(this.retryButton.transform, this.originalRetryButtonScale, Vector3.zero, null, 0.2f);
			yield break;
		}

		private IEnumerator AnimateConfirmButtonIn()
		{
			yield return FiberAnimation.ScaleTransform(this.confirmButton.transform, Vector3.zero, this.originalConfirmButtonScale, null, 0.2f);
			yield break;
		}

		private IEnumerator AnimateConfirmButtonOut()
		{
			yield return FiberAnimation.ScaleTransform(this.confirmButton.transform, this.originalConfirmButtonScale, Vector3.zero, null, 0.2f);
			yield break;
		}

		[SerializeField]
		private GameObject continueText;

		[SerializeField]
		private GameObject retryButton;

		[SerializeField]
		private GameObject confirmButton;

		private Vector3 originalContinueTextPos;

		private Vector3 originalRetryButtonScale;

		private Vector3 originalConfirmButtonScale;

		private AnimationCurve moveCurve;
	}
}
