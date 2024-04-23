using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionPlace : MapAction
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

		public Location PlacementLocation
		{
			get
			{
				return this.placementLocation;
			}
			set
			{
				this.placementLocation = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapMoveable mapComponent = map.GetMapComponent<MapMoveable>(this.moveableId);
			if (mapComponent == null)
			{
				yield break;
			}
			mapComponent.transform.position = this.placementLocation.Position;
			mapComponent.Direction = this.placementLocation.Direction;
			MapSpineCharacter component = mapComponent.GetComponent<MapSpineCharacter>();
			if (component != null)
			{
				component.PlayIdleAnimation(-this.placementLocation.Direction);
				yield break;
			}
			yield break;
		}

		public override bool IsAllowedWhenSkipping
		{
			get
			{
				return true;
			}
		}

		[SerializeField]
		private string moveableId;

		[SerializeField]
		public Location placementLocation;
	}
}
