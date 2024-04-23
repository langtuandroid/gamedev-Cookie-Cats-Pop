using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public class MapActionHideImage : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return map.HideFullScreenImage();
			yield break;
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}
	}
}
