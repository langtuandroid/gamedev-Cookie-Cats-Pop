using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapImage))]
	public class MapTransformSelectionBounceAnimation : MapComponent, IMapPropSelectionAnimatable
	{
		IEnumerator IMapPropSelectionAnimatable.PlaySelectionAnimation()
		{
			MapTransformSelectionBounceAnimation.Config config = GardenGameSetup.Get.selectionBounceAnimation;
			Vector3 startScale = base.transform.localScale;
			Vector3 startPosition = base.transform.localPosition;
			yield return new Fiber.OnTerminate(delegate()
			{
				this.transform.localScale = startScale;
				this.transform.localPosition = startPosition;
			});
			yield return new Fiber.OnExit(delegate()
			{
				this.transform.localScale = startScale;
				this.transform.localPosition = startPosition;
			});
			yield return FiberAnimation.Animate(config.duration, delegate(float t)
			{
				Vector3 localPosition = startPosition + new Vector3(0f, config.bounceCurve.Evaluate(t) * config.bounceDistance, 0f);
				Vector3 localScale = startScale + new Vector3(config.scaleCurveX.Evaluate(t) * startScale.x, config.scaleCurveY.Evaluate(t) * startScale.y, 0f);
				this.transform.localScale = localScale;
				this.transform.localPosition = localPosition;
			});
			yield break;
		}

		[Serializable]
		public class Config
		{
			[SerializeField]
			public AnimationCurve bounceCurve;

			[SerializeField]
			public AnimationCurve scaleCurveX;

			[SerializeField]
			public AnimationCurve scaleCurveY;

			[SerializeField]
			public float bounceDistance = 100f;

			[SerializeField]
			public float duration = 0.5f;
		}
	}
}
