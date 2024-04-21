using System;
using System.Collections;
using System.Collections.Generic;
using Tactile.GardenGame.Helpers;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionMove : MapAction
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

		public Location TargetLocation
		{
			get
			{
				return this.targetLocation;
			}
			set
			{
				this.targetLocation = value;
			}
		}

		public MapMoveable.MoveBehaviour MoveType
		{
			get
			{
				return this.moveType;
			}
			set
			{
				this.moveType = value;
			}
		}

		public bool ForceDirectPath
		{
			get
			{
				return this.forceDirectPath;
			}
			set
			{
				this.forceDirectPath = value;
			}
		}

		public float SpeedMultiplier
		{
			get
			{
				return this.speedMultiplier;
			}
			set
			{
				this.speedMultiplier = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			MapMoveable mapMoveable = map.GetMapComponent<MapMoveable>(this.moveableId);
			if (mapMoveable == null)
			{
				yield break;
			}
			MapMoveable.MoveBehaviour prevMoveType = mapMoveable.DesiredMoveBehaviour;
			mapMoveable.DesiredMoveBehaviour = this.MoveType;
			mapMoveable.MovementSpeedMultiplier = this.SpeedMultiplier;
			if (this.forceDirectPath)
			{
				if (GardenGameSetup.Get.pathFindingSetUp.onlyFollowIsometricDirections && mapMoveable.GetComponent<MapCamera>() == null)
				{
					List<Vector2> waypoints = IsometricUtils.GetCardinalOnlyPath(mapMoveable.transform.position, this.targetLocation.Position, GardenGameSetup.Get.pathFindingSetUp.CardinalDirectionThresholdDirectPaths);
					yield return mapMoveable.FollowPath(waypoints);
				}
				else
				{
					yield return mapMoveable.MoveToPos(this.targetLocation.Position, this.targetLocation.Direction, false);
				}
			}
			else
			{
				yield return mapMoveable.MoveToPos(this.targetLocation.Position, this.targetLocation.Direction, false);
			}
			mapMoveable.DesiredMoveBehaviour = prevMoveType;
			mapMoveable.MovementSpeedMultiplier = 1f;
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
		public Location targetLocation;

		[SerializeField]
		private MapMoveable.MoveBehaviour moveType;

		[SerializeField]
		private bool forceDirectPath;

		[SerializeField]
		private float speedMultiplier = 1f;
	}
}
