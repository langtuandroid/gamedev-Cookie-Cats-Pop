using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class StoryCelebrationView : UIView
	{
		public IEnumerator PlayEffect(string message)
		{
			this.messageLabel.text = message;
			yield return FiberAnimation.ScaleTransform(this.scalePivot, Vector3.zero, Vector3.one, this.scaleCurve, 0f);
			yield break;
		}

		[SerializeField]
		private UILabel messageLabel;

		[SerializeField]
		private Transform scalePivot;

		[SerializeField]
		private AnimationCurve scaleCurve;
	}
}
