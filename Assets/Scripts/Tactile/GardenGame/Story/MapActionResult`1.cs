using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public abstract class MapActionResult<T> : MapAction, IMapActionResult where T : new()
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			this.result = Activator.CreateInstance<T>();
			yield return this.Logic(map, this.result);
			yield break;
		}

		protected abstract IEnumerator Logic(IStoryMapController map, T result);

		object IMapActionResult.GetResult()
		{
			return this.result;
		}

		private T result;
	}
}
