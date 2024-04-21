using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public class MapActionShowBars : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return map.ShowBars();
			yield break;
		}
	}
}
