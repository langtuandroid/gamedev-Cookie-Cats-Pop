using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile.GardenGame.MapSystem;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionIdleBehaviour : MapAction
	{
		public MapActionIdleBehaviour.IdleMode IdleBehaviour
		{
			get
			{
				return this.idleBehaviour;
			}
			set
			{
				this.idleBehaviour = value;
			}
		}

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

		public string ParentMoveableId
		{
			get
			{
				return this.parentMoveableId;
			}
			set
			{
				this.parentMoveableId = value;
			}
		}

		public float DurationSeconds
		{
			get
			{
				return this.durationSeconds;
			}
			set
			{
				this.durationSeconds = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			yield return new Fiber.OnExit(delegate()
			{
				this.behaviourFiber.Terminate();
				if (this.mapMoveable != null)
				{
					this.mapMoveable.StopMoving();
				}
			});
			this.map = map;
			this.mapMoveable = map.GetMapComponent<MapMoveable>(this.moveableId);
			if (this.mapMoveable == null)
			{
				yield break;
			}
			this.behaviourFiber = new Fiber(FiberBucket.Manual);
			MapActionIdleBehaviour.IdleMode idleMode = this.IdleBehaviour;
			if (idleMode != MapActionIdleBehaviour.IdleMode.RandomIdlePoints)
			{
				if (idleMode == MapActionIdleBehaviour.IdleMode.StayNearOther)
				{
					this.behaviourFiber.Start(this.StayNearOtherLogic());
				}
			}
			else
			{
				this.behaviourFiber.Start(this.RandomIdlePointsLogic());
			}
			float timeRan = 0f;
			for (;;)
			{
				timeRan += Time.deltaTime;
				if (this.durationSeconds > 0f & timeRan > this.durationSeconds)
				{
					break;
				}
				this.behaviourFiber.Step();
				yield return null;
			}
			this.behaviourFiber.Terminate();
			yield break;
			yield break;
		}

		private IEnumerator RandomIdlePointsLogic()
		{
			MapIdlepoint currentIdlePoint = null;
			for (;;)
			{
				currentIdlePoint = this.FindRandomIdlePoint(currentIdlePoint);
				if (currentIdlePoint == null)
				{
					break;
				}
				Vector3 targetPos = currentIdlePoint.Location.Position;
				yield return this.mapMoveable.MoveToPos(targetPos, currentIdlePoint.Location.Direction, false);
				yield return FiberHelper.Wait(UnityEngine.Random.Range(this.idleWaitDurationMin, this.idleWaitDurationMax), (FiberHelper.WaitFlag)0);
			}
			yield break;
			yield break;
		}

		private IEnumerator StayNearOtherLogic()
		{
			MapMoveable owner = this.map.GetMapComponent<MapMoveable>(this.parentMoveableId);
			if (this.mapMoveable == null)
			{
				yield break;
			}
			for (;;)
			{
				Vector2 targetPos = this.FindIdlePointNearOther(owner);
				yield return this.mapMoveable.MoveToPos(targetPos, owner.Direction, false);
				while (base.transform)
				{
					if (!this.mapMoveable.IsMoving)
					{
						break;
					}
					if ((owner.transform.position - this.mapMoveable.transform.position).magnitude < this.radiusForNearPoints)
					{
						this.mapMoveable.StopMoving();
					}
				}
				float delay = UnityEngine.Random.Range(this.idleWaitDurationMin, this.idleWaitDurationMax);
				float timeSpentHere = 0f;
				while (delay > timeSpentHere)
				{
					timeSpentHere += Time.deltaTime;
					if ((owner.TargetPosition - targetPos).magnitude > this.stayNearRecalculateDistance && timeSpentHere > 2f)
					{
						timeSpentHere = delay;
					}
					yield return null;
				}
			}
			yield break;
		}

		private MapIdlepoint FindRandomIdlePoint(MapIdlepoint excludePoint)
		{
			List<MapIdlepoint> list = new List<MapIdlepoint>();
			foreach (MapIdlepoint mapIdlepoint in this.mapMoveable.OwnerMap.Areas.IterateComponents<MapIdlepoint>())
			{
				if (mapIdlepoint != excludePoint && mapIdlepoint.Enabled)
				{
					list.Add(mapIdlepoint);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list.GetRandom<MapIdlepoint>();
		}

		protected Vector2 FindIdlePointNearOther(MapMoveable other)
		{
			Vector2 targetPosition = other.TargetPosition;
			float num = UnityEngine.Random.Range(30f, 60f);
			float num2 = UnityEngine.Random.Range(30f, 60f);
			if (UnityEngine.Random.Range(0f, this.radiusForNearPoints) > 50f)
			{
				num *= -1f;
			}
			if (UnityEngine.Random.Range(0f, this.radiusForNearPoints) > 30f)
			{
				num2 *= -1f;
			}
			Vector2 b = new Vector3(num, num2);
			if (!MapCollision.RayCast(targetPosition + b, targetPosition))
			{
				return targetPosition + b;
			}
			MapIdlepoint mapIdlepoint = null;
			float maxValue = float.MaxValue;
			List<MapIdlepoint> list = new List<MapIdlepoint>();
			foreach (MapIdlepoint mapIdlepoint2 in this.mapMoveable.OwnerMap.Areas.IterateComponents<MapIdlepoint>())
			{
				if (other != null)
				{
					float sqrMagnitude = (mapIdlepoint2.transform.position - other.transform.position).sqrMagnitude;
					if (sqrMagnitude < maxValue)
					{
						mapIdlepoint = mapIdlepoint2;
					}
				}
				list.Add(mapIdlepoint2);
			}
			if (mapIdlepoint != null)
			{
				return mapIdlepoint.Location.Position;
			}
			if (list.Count <= 0)
			{
				return targetPosition;
			}
			return list.GetRandom<MapIdlepoint>().Location.Position;
		}

		[SerializeField]
		private string parentMoveableId;

		[SerializeField]
		private string moveableId;

		[SerializeField]
		private float durationSeconds;

		[SerializeField]
		private MapActionIdleBehaviour.IdleMode idleBehaviour;

		private MapMoveable mapMoveable;

		private Vector2 currentTargetPosition;

		private Fiber behaviourFiber;

		private IStoryMapController map;

		private float radiusForNearPoints = 100f;

		private float stayNearRecalculateDistance = 500f;

		private float idleWaitDurationMin = 2f;

		private float idleWaitDurationMax = 10f;

		public enum IdleMode
		{
			RandomIdlePoints,
			StayNearOther
		}
	}
}
