using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionFade : MapAction
	{
		public MapActionFade.FadeType FadeBehaviour
		{
			get
			{
				return this.fadeBehaviour;
			}
			set
			{
				this.fadeBehaviour = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			switch (this.fadeBehaviour)
			{
			case MapActionFade.FadeType.FadeToBlack:
				yield return map.FadeToBlack();
				break;
			case MapActionFade.FadeType.FadeToMap:
				yield return map.FadeToMap();
				break;
			case MapActionFade.FadeType.InstantBlack:
				map.InstantBlack();
				break;
			case MapActionFade.FadeType.RemoveBlack:
				map.RemoveBlack();
				break;
			}
			yield break;
		}

		[SerializeField]
		private MapActionFade.FadeType fadeBehaviour;

		public enum FadeType
		{
			FadeToBlack,
			FadeToMap,
			InstantBlack,
			RemoveBlack
		}
	}
}
