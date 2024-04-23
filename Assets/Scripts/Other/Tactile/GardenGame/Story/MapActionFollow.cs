using System;
using System.Collections;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionFollow : MapAction
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

		public string FollowId
		{
			get
			{
				return this.followId;
			}
			set
			{
				this.followId = value;
			}
		}

		public MapMoveable.FollowBehaviour FollowBehaviour
		{
			get
			{
				return this.followBehaviour;
			}
			set
			{
				this.followBehaviour = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapMoveable mapComponent = map.GetMapComponent<MapMoveable>(this.moveableId);
			if (mapComponent == null)
			{
				yield break;
			}
			MapMoveable mapComponent2 = map.GetMapComponent<MapMoveable>(this.followId);
			if (mapComponent2 == null)
			{
				yield break;
			}
			mapComponent.Follow(mapComponent2, this.FollowBehaviour);
			yield break;
		}

		[SerializeField]
		private string moveableId;

		[SerializeField]
		private string followId;

		[SerializeField]
		private MapMoveable.FollowBehaviour followBehaviour;
	}
}
