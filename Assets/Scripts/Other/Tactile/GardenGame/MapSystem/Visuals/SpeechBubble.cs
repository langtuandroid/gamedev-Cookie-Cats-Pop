using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Visuals
{
	public class SpeechBubble : MonoBehaviour
	{
		public bool IsVisible
		{
			get
			{
				return this.scalePivot.gameObject.activeSelf;
			}
		}

		public IEnumerator Say(string text)
		{
			yield return new Fiber.OnExit(delegate()
			{
				this.RefreshText(text);
				this.scalePivot.localScale = Vector3.one;
			});
			if (!this.IsVisible)
			{
				this.label.text = string.Empty;
				this.label.typingLookahead = -1;
				this.label.typingProgress = 0f;
				yield return this.FadeIn();
			}
			this.RefreshText(text);
			this.label.typingProgress = 0f;
			this.label.typingLookahead = 0;
			while (this.label.typingProgress < 1f)
			{
				yield return null;
			}
			yield break;
		}

		private void RefreshText(string text)
		{
			this.label.text = text;
		}

		private IEnumerator FadeIn()
		{
			this.Show();
			yield return FiberAnimation.ScaleTransform(this.scalePivot, Vector3.zero, Vector3.one, this.scaleInCurve, 0f);
			yield break;
		}

		public IEnumerator FadeOut()
		{
			yield return FiberAnimation.ScaleTransform(this.scalePivot, Vector3.zero, Vector3.one, this.scaleOutCurve, 0f);
			this.Hide();
			yield break;
		}

		public void Hide()
		{
			this.scalePivot.gameObject.SetActive(false);
		}

		private void Show()
		{
			this.scalePivot.gameObject.SetActive(true);
		}

		public AnimationCurve scaleInCurve;

		public AnimationCurve scaleOutCurve;

		public Transform scalePivot;

		public UILabel label;
	}
}
