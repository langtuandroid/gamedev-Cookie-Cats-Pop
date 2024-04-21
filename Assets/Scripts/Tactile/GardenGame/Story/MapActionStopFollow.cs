using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionStopFollow : MapAction
	{
		public string MoveableId
		{
			get
			{
				return this.moveableId;
			}
			set
			{
				this.moveableId = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapMoveable mapComponent = map.GetMapComponent<MapMoveable>(this.moveableId);
			if (mapComponent == null)
			{
				yield break;
			}
			mapComponent.StopFollow();
			yield break;
		}

		[SerializeField]
		private string moveableId;
	}
}
