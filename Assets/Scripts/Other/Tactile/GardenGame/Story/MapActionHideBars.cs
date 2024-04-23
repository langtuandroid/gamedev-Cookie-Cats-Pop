using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public class MapActionHideBars : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return map.HideBars();
			yield break;
		}
	}
}
