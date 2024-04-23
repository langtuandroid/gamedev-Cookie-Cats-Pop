using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public class MapActionWait : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return FiberHelper.Wait(this.Delay, (FiberHelper.WaitFlag)0);
			yield break;
		}

		public float Delay = 1f;
	}
}
