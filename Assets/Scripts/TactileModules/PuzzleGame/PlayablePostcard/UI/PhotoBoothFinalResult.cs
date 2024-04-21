using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
	public class PhotoBoothFinalResult : PhotoBoothVisualState
	{
		public void Initialize(GameObject postcard, float animationDuration, AnimationCurve animationMoveCurve)
		{
			this.currentPostcard = postcard;
			this.originalPostcardScale = this.currentPostcard.transform.localScale;
			this.originalTitlePos = this.title.transform.localPosition;
			this.originalExitButtonScale = this.exitButton.transform.localScale;
			this.originalShareButtonScale = this.shareButton.transform.localScale;
			this.originalBottomBarPos = this.bottomBar.transform.localPosition;
			this.duration = animationDuration;
			this.moveCurve = animationMoveCurve;
		}

		public override IEnumerator AnimateIn()
		{
			this.exitButton.gameObject.SetActive(false);
			this.shareButton.gameObject.SetActive(false);
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.AnimatePostcardIn(),
				this.AnimateTitleIn(),
				this.AnimateBottomBarIn()
			});
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			{
				yield return this.AnimateShareButtonIn();
			}
			yield return this.AnimateExitButtonIn();
			yield break;
		}

		public override IEnumerator AnimateOut()
		{
			yield break;
		}

		private IEnumerator AnimatePostcardIn()
		{
			this.currentPostcard.gameObject.SetActive(true);
			yield return FiberAnimation.ScaleTransform(this.currentPostcard.transform, Vector3.zero, this.originalPostcardScale, this.moveCurve, this.duration);
			yield break;
		}

		private IEnumerator AnimateExitButtonIn()
		{
			this.exitButton.gameObject.SetActive(true);
			yield return FiberAnimation.ScaleTransform(this.exitButton.transform, Vector3.zero, this.originalExitButtonScale, null, this.duration * 0.5f);
			yield break;
		}

		private IEnumerator AnimateShareButtonIn()
		{
			this.shareButton.gameObject.SetActive(true);
			yield return FiberAnimation.ScaleTransform(this.shareButton.transform, Vector3.zero, this.originalShareButtonScale, null, this.duration * 0.5f);
			yield break;
		}

		private IEnumerator AnimateTitleIn()
		{
			yield return FiberAnimation.MoveLocalTransform(this.title.transform, this.originalTitlePos + new Vector3(0f, 400f, 0f), this.originalTitlePos, this.moveCurve, this.duration);
			yield break;
		}

		private IEnumerator AnimateBottomBarIn()
		{
			yield return FiberAnimation.MoveLocalTransform(this.bottomBar.transform, this.originalBottomBarPos + new Vector3(0f, -300f, 0f), this.originalBottomBarPos, this.moveCurve, 0.6f);
			yield break;
		}

		[SerializeField]
		private GameObject title;

		[SerializeField]
		private GameObject exitButton;

		[SerializeField]
		private GameObject bottomBar;

		[SerializeField]
		private GameObject shareButton;

		private GameObject currentPostcard;

		private Vector3 originalPostcardScale;

		private Vector3 originalTitlePos;

		private Vector3 originalExitButtonScale;

		private Vector3 originalShareButtonScale;

		private Vector3 originalBottomBarPos;

		private float duration;

		private AnimationCurve moveCurve;
	}
}
