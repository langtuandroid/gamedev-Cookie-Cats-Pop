using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.GameCore.ButtonArea
{
	public class ButtonAreaAnimator : UIViewAnimator
	{
		private void Awake()
		{
			this.positionSet = false;
		}

		private void SetPositions()
		{
			if (!this.positionSet)
			{
				this.startPosition = base.transform.position;
				this.invisiblePosition = this.InvisiblePosition.position;
				this.positionSet = true;
			}
		}

		public override IEnumerator AnimateIn()
		{
			this.SetPositions();
			yield return FiberAnimation.MoveTransform(base.transform, this.invisiblePosition, this.startPosition, AnimationCurve.Linear(0f, 0f, 1f, 1f), 0.15f);
			yield break;
		}

		public override IEnumerator AnimateOut()
		{
			this.SetPositions();
			yield return FiberAnimation.MoveTransform(base.transform, this.startPosition, this.invisiblePosition, AnimationCurve.Linear(0f, 0f, 1f, 1f), 0.15f);
			yield break;
		}

		[SerializeField]
		private Transform InvisiblePosition;

		private Vector3 startPosition;

		private Vector3 invisiblePosition;

		private bool positionSet;
	}
}
