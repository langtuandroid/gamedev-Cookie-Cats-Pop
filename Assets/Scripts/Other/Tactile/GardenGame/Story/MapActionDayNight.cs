using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionDayNight : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			if (this.Duration <= 0f)
			{
				map.Darkness = ((this.Action != MapActionDayNight.DayNightAction.FadeToDay) ? 1f : 0f);
				yield break;
			}
			float target = (this.Action != MapActionDayNight.DayNightAction.FadeToDay) ? 1f : 0f;
			yield return new Fiber.OnExit(delegate()
			{
				map.Darkness = target;
			});
			float initialDarkness = map.Darkness;
			yield return FiberAnimation.Animate(this.Duration, delegate(float t)
			{
				map.Darkness = Mathf.Lerp(initialDarkness, target, t);
			});
			yield break;
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}

		public MapActionDayNight.DayNightAction Action;

		public float Duration = 1f;

		public enum DayNightAction
		{
			FadeToDay,
			FadeToNight
		}
	}
}
