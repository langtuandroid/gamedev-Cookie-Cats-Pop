using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public class MapActionStopMusic : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			map.StopMusic();
			yield break;
		}
	}
}
