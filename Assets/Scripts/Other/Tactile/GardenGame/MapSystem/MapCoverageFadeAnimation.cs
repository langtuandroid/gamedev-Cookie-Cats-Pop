using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapCoverage))]
	public class MapCoverageFadeAnimation : MapComponent, IMapPropBuildAnimatable
	{
		public AnimationCurve Curve
		{
			get
			{
				return this.curve;
			}
			set
			{
				this.curve = value;
			}
		}

		public MapCoverage MapCoverage
		{
			get
			{
				if (this.mapCoverage == null)
				{
					this.mapCoverage = base.GetComponent<MapCoverage>();
				}
				return this.mapCoverage;
			}
		}

		protected override void Initialized()
		{
			this.initialMaxHeight = this.MapCoverage.MaxHeight;
		}

		public void BuildInitialize(bool isTeardown)
		{
		}

		public IEnumerator Build(bool isTeardown)
		{
			if (isTeardown)
			{
				base.gameObject.SetActive(true);
			}
			float start = (!isTeardown) ? 1f : this.initialMaxHeight;
			float end = (!isTeardown) ? this.initialMaxHeight : 1f;
			yield return new Fiber.OnExit(delegate()
			{
				this.MapCoverage.MaxHeight = end;
				if (isTeardown)
				{
					this.gameObject.SetActive(false);
				}
			});
			yield return FiberAnimation.Animate(0f, this.curve, delegate(float t)
			{
				this.MapCoverage.MaxHeight = Mathf.Lerp(start, end, t);
			}, false);
			yield break;
		}

		[SerializeField]
		private AnimationCurve curve;

		private MapCoverage mapCoverage;

		private float initialMaxHeight;
	}
}
