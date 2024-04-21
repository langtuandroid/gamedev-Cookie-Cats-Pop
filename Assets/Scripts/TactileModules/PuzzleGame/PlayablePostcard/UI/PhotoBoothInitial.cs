using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
	public class PhotoBoothInitial : PhotoBoothVisualState
	{
		public void Initialize(AnimationCurve animationMoveCurve)
		{
			this.originalLogoAngle = this.logo.transform.localEulerAngles;
			this.originalLogoScale = this.logo.transform.localScale;
			this.originalTopBarPos = this.topBar.transform.localPosition;
			this.moveCurve = animationMoveCurve;
		}

		public override IEnumerator AnimateIn()
		{
			this.topBar.SetActive(false);
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.AnimateLogoIn(),
				this.AnimateTopBarIn()
			});
			yield break;
		}

		public override IEnumerator AnimateOut()
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.AnimateLogoOut(),
				this.AnimateTopBarOut()
			});
			yield break;
		}

		private IEnumerator AnimateLogoIn()
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.ScaleTransform(this.logo.transform, this.originalLogoScale * 8f, this.originalLogoScale, this.logoScaleCurve, 0.2f),
				FiberAnimation.RotateTransform(this.logo.transform, this.originalLogoAngle - new Vector3(0f, 0f, 45f), this.originalLogoAngle, null, 0.2f)
			});
			yield return FiberAnimation.ShakeLocalPosition(UIViewManager.Instance.FindTopMostCameraWithViews().transform, UIViewManager.Instance.FindTopMostCameraWithViews().transform.localPosition, null, 0.3f, 10f, 20f, 0f);
			yield break;
		}

		private IEnumerator AnimateLogoOut()
		{
			yield return FiberAnimation.ScaleTransform(this.logo.transform, this.originalLogoScale, Vector3.zero, this.logoScaleCurve, 0.2f);
			yield break;
		}

		private IEnumerator AnimateTopBarIn()
		{
			this.topBar.SetActive(true);
			yield return FiberAnimation.MoveLocalTransform(this.topBar.transform, this.originalTopBarPos + new Vector3(0f, 300f, 0f), this.originalTopBarPos, this.moveCurve, 0.6f);
			yield break;
		}

		private IEnumerator AnimateTopBarOut()
		{
			this.topBar.SetActive(true);
			yield return FiberAnimation.MoveLocalTransform(this.topBar.transform, this.originalTopBarPos, this.originalTopBarPos + new Vector3(0f, 300f, 0f), this.moveCurve, 0.6f);
			yield break;
		}

		[SerializeField]
		private GameObject topBar;

		[SerializeField]
		private GameObject logo;

		[SerializeField]
		private AnimationCurve logoScaleCurve;

		private Vector3 originalLogoAngle;

		private Vector3 originalLogoScale;

		private Vector3 originalTopBarPos;

		private AnimationCurve moveCurve;
	}
}
