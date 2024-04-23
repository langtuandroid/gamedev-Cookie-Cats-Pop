using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapImage))]
	public class MapImageFadeAnimation : MapComponent, IMapPropBuildAnimatable
	{
		public MapImage MapImage
		{
			get
			{
				if (this.mapImage == null)
				{
					this.mapImage = base.GetComponent<MapImage>();
				}
				return this.mapImage;
			}
		}

		void IMapPropBuildAnimatable.BuildInitialize(bool isTeardown)
		{
			if (isTeardown)
			{
				base.gameObject.SetActive(true);
			}
			Color white = Color.white;
			white.a = ((!isTeardown) ? 0f : 1f);
			this.MapImage.SetTint(white);
		}

		IEnumerator IMapPropBuildAnimatable.Build(bool isTeardown)
		{
			AnimationCurve curve = GardenGameSetup.Get.mapImageFadeAnimationCurve;
			yield return FiberAnimation.Animate(GardenGameSetup.Get.mapImageFadeAnimationCurveDuration, delegate(float t)
			{
				Color white = Color.white;
				if (isTeardown)
				{
					white.a = curve.Evaluate(1f - t);
				}
				else
				{
					white.a = curve.Evaluate(t);
				}
				this.MapImage.SetTint(white);
			});
			this.MapImage.RemoveTint();
			if (isTeardown)
			{
				base.gameObject.SetActive(false);
			}
			yield break;
		}

		private float fadeDuration = 0.3f;

		private MapImage mapImage;

		private Material material;
	}
}
