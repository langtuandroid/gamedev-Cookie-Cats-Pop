using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionFaceDirecton : MapAction
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

		public Vector2 Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapMoveable mapComponent = map.GetMapComponent<MapMoveable>(this.moveableId);
			if (mapComponent == null)
			{
				yield break;
			}
			mapComponent.Direction = this.Direction;
			MapSpineCharacter component = mapComponent.GetComponent<MapSpineCharacter>();
			if (component != null)
			{
				component.PlayIdleAnimation(this.Direction);
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
		public Vector2 direction;
	}
}
