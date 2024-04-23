using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.MapSystem.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
    public class MapMoveable : MapComponent
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapMoveable, Vector2> DirectionChanged;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapMoveable> MovementStarted;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapMoveable> MovementStopped;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapMoveable, float, float> Moved;

        public float PanDuration
        {
            get
            {
                return this.panDuration;
            }
            set
            {
                this.panDuration = value;
            }
        }

        public Vector2 TargetPosition
        {
            get
            {
                if (this.followTarget != null && this.followFiber != null && !this.followFiber.IsTerminated)
                {
                    return this.followTarget.TargetPosition;
                }
                if (this.moveFiber != null && !this.moveFiber.IsTerminated)
                {
                    return this.moveTarget;
                }
                return base.transform.position;
            }
        }

        public MapMoveable.MoveBehaviour DesiredMoveBehaviour
        {
            get
            {
                return this.desiredMoveBehaviour;
            }
            set
            {
                this.desiredMoveBehaviour = value;
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
                if (this.DirectionChanged != null)
                {
                    this.DirectionChanged(this, this.direction);
                }
            }
        }

        public bool DisablePathfinding
        {
            get
            {
                return this.disablePathfinding;
            }
            set
            {
                this.disablePathfinding = value;
            }
        }

        public float MovementSpeed
        {
            get
            {
                return this.movementSpeed * this.MovementSpeedMultiplier;
            }
            set
            {
                this.movementSpeed = value;
            }
        }

        public bool IsMoving
        {
            get
            {
                return this.moveFiber != null && !this.moveFiber.IsTerminated;
            }
        }

        public Vector2 Target { get; set; }

        public Vector2 TargetDirection { get; set; }

        public float MovementSpeedMultiplier { get; set; }

        protected override void Initialized()
        {
            this.MovementSpeedMultiplier = 1f;
        }

        public void Follow(MapMoveable followTarget, MapMoveable.FollowBehaviour followMode = MapMoveable.FollowBehaviour.LerpThenLock)
        {
            if (followTarget == null)
            {
                this.StopFollow();
                return;
            }
            if (this.followFiber == null)
            {
                this.followFiber = new Fiber(FiberBucket.Manual);
            }
            this.CalculateFollowOffset();
            followTarget.CalculateFollowOffset();
            if (this.disablePathfinding)
            {
                switch (followMode)
                {
                    case MapMoveable.FollowBehaviour.LerpThenLock:
                        this.followFiber.Start(this.FollowLogicNoPathFindingLerpThenLock(followTarget));
                        break;
                    case MapMoveable.FollowBehaviour.Radius:
                        this.followFiber.Start(this.FollowLogicNoPathFindingMaintainRadius(followTarget));
                        break;
                    case MapMoveable.FollowBehaviour.RadiusThenCentre:
                        this.followFiber.Start(this.FollowLogicNoPathFindingRadiusCentre(followTarget));
                        break;
                    case MapMoveable.FollowBehaviour.SmoothAccelerate:
                        this.followFiber.Start(this.FollowLogicSmoothAccelerate(followTarget));
                        break;
                }
            }
            else
            {
                this.followFiber.Start(this.FollowLogic(followTarget));
            }
        }

        public void StopFollow()
        {
            if (this.followFiber != null)
            {
                this.followFiber.Terminate();
                this.followFiber = null;
            }
        }

        public IEnumerator WaitForStop()
        {
            while (this.IsMoving)
            {
                yield return null;
            }
            yield break;
        }

        public IEnumerator MoveToPos(Vector2 target, Vector2 targetDirection, bool ignoreTargetDirection = false)
        {
            this.moveTarget = target;
            this.mapSpineRootMotion = base.GetComponentInChildren<MapSpineRootMotion>();
            this.useRootMotion = (this.mapSpineRootMotion != null);
            if ((GardenGameSetup.Get.pathFindingSetUp.onlyFollowIsometricDirections || this.useRootMotion) && !this.disablePathfinding)
            {
                if (base.OwnerMap.Navigation.IsBuildingMesh())
                {
                    yield return null;
                }
                List<Vector2> waypoints = base.OwnerMap.Navigation.GetPath(base.transform.position, target);
                yield return this.FollowPath(waypoints);
            }
            else
            {
                this.Target = target;
                this.TargetDirection = targetDirection;
                yield return new Fiber.OnExit(new Fiber.OnExitHandler(this.StopMoving));
                if (this.moveFiber == null)
                {
                    this.moveFiber = new Fiber(FiberBucket.Manual);
                }
                this.StopFollow();
                this.isStartingMovement = true;
                this.moveFiber.Start(this.MoveToPosWithEvents(target, targetDirection, ignoreTargetDirection));
                this.isStartingMovement = false;
                this.moveFiber.Step();
                yield return this.WaitForStop();
            }
            yield break;
        }

        public void StopMoving()
        {
            if (this.moveFiber == null)
            {
                return;
            }
            this.moveFiber.Terminate();
            this.moveFiber = null;
        }

        private IEnumerator MoveToPosWithEvents(Vector2 target, Vector2 targetDirection, bool ignoreTargetDirection)
        {
            if (this.MovementStarted != null)
            {
                this.MovementStarted(this);
            }
            yield return new Fiber.OnExit(delegate ()
            {
                if (this.MovementStopped != null)
                {
                    this.MovementStopped(this);
                }
            });
            if (Vector2.Distance(target, base.transform.position) < 1f)
            {
                yield break;
            }
            yield return this.MoveToPosInternal(target, targetDirection, ignoreTargetDirection);
            yield break;
        }

        private IEnumerator MoveToPosInternal(Vector2 target, Vector2 targetDirection, bool ignoreTargetDirection)
        {
            if (!this.disablePathfinding)
            {
                yield return new Fiber.OnExit(delegate ()
                {
                    if (this.isStartingMovement)
                    {
                        return;
                    }
                    this.transform.position = new Vector3(target.x, target.y, 0f);
                    this.Direction = targetDirection;
                });
                yield return base.OwnerMap.Navigation.PathFind(base.transform.position, target, targetDirection, ignoreTargetDirection, this.MovementSpeed, delegate (Vector2 p, Vector2 dir)
                {
                    this.transform.position = new Vector3(p.x, p.y, 0f);
                    this.Direction = dir;
                }, delegate (float totalLength, float currentPosition)
                {
                    if (this.Moved != null)
                    {
                        this.Moved(this, totalLength, currentPosition);
                    }
                });
                yield return new Fiber.OnExit(null);
            }
            else
            {
                yield return new Fiber.OnExit(delegate ()
                {
                    if (this.isStartingMovement)
                    {
                        return;
                    }
                    this.transform.position = new Vector3(target.x, target.y, 0f);
                });
                IMapMoveableMovement moveableMovement = base.GetComponent<IMapMoveableMovement>();
                if (moveableMovement != null)
                {
                    yield return moveableMovement.Move(base.transform.position, target, this.Direction, this.TargetDirection, this.MovementSpeed);
                }
                yield return new Fiber.OnExit(null);
            }
            yield break;
        }

        public IEnumerator FollowPath(List<Vector2> waypoints)
        {
            this.mapSpineRootMotion = base.GetComponentInChildren<MapSpineRootMotion>();
            this.useRootMotion = (this.mapSpineRootMotion != null);
            if (waypoints == null || waypoints.Count <= 0)
            {
                yield break;
            }
            this.Target = waypoints[waypoints.Count - 1];
            this.TargetDirection = this.Target - new Vector2(base.transform.position.x, base.transform.position.y);
            yield return new Fiber.OnExit(new Fiber.OnExitHandler(this.StopMoving));
            if (this.moveFiber == null)
            {
                this.moveFiber = new Fiber(FiberBucket.Manual);
            }
            this.StopFollow();
            this.moveFiber.Start(this.FollowPathInternal(waypoints));
            this.moveFiber.Step();
            yield return this.WaitForStop();
            yield break;
        }

        private IEnumerator FollowPathInternal(List<Vector2> waypoints)
        {
            if (waypoints.Count <= 0)
            {
                yield break;
            }
            Vector2 target = waypoints[waypoints.Count - 1];
            if (this.MovementStarted != null)
            {
                this.MovementStarted(this);
            }
            yield return new Fiber.OnExit(delegate ()
            {
                if (this.MovementStopped != null)
                {
                    this.MovementStopped(this);
                }
            });
            if (Vector2.Distance(target, base.transform.position) < 1f)
            {
                yield break;
            }
            for (int i = 0; i < waypoints.Count; i++)
            {
                target = waypoints[i];
                float speed = this.MovementSpeed;
                for (; ; )
                {
                    float movementThisFrame = speed * Time.deltaTime;
                    if (this.useRootMotion)
                    {
                        movementThisFrame = this.mapSpineRootMotion.GetDistanceTravelledThisFrame();
                        if (movementThisFrame > 10f)
                        {
                            movementThisFrame = 0f;
                        }
                    }
                    Vector3 delta = waypoints[i] - new Vector2(base.transform.position.x, base.transform.position.y);
                    this.Direction = delta.normalized;
                    float distLeft = delta.magnitude;
                    if (distLeft <= movementThisFrame)
                    {
                        break;
                    }
                    base.transform.position += new Vector3(this.Direction.x * movementThisFrame, this.Direction.y * movementThisFrame, 0f);
                    yield return null;
                }
                base.transform.position = target;
            }
            yield break;
        }

        private void Update()
        {
            if (this.moveFiber != null)
            {
                this.moveFiber.Step();
            }
            if (this.followFiber != null)
            {
                this.followFiber.Step();
            }
        }

        private IEnumerator FollowLogicNoPathFindingMaintainRadius(MapMoveable target)
        {
            float targetDistance = GardenGameSetup.Get.camRadiusFollowRadius;
            float speedMultiplier = GardenGameSetup.Get.camRadiusFollowSpeed;
            float maxSpeed = GardenGameSetup.Get.camMaxOrthoSize;
            for (; ; )
            {
                Vector3 delta = this.GetFollowTarget(target) - base.transform.position;
                float distanceLeft = delta.magnitude - targetDistance;
                if (distanceLeft > 0f)
                {
                    float d = Mathf.Min(speedMultiplier * distanceLeft, maxSpeed);
                    Vector3 a = delta.normalized * d;
                    base.transform.position = base.transform.position + a * Time.deltaTime;
                }
                yield return null;
            }
            yield break;
        }

        private IEnumerator FollowLogicNoPathFindingRadiusCentre(MapMoveable target)
        {
            float targetDistance = GardenGameSetup.Get.camRadiusFollowRadius;
            float speedMultiplier = GardenGameSetup.Get.camRadiusFollowSpeed;
            float maxSpeed = GardenGameSetup.Get.camMaxOrthoSize;
            bool leftRadius = false;
            for (; ; )
            {
                Vector3 delta = this.GetFollowTarget(target) - base.transform.position;
                float distanceLeft = delta.magnitude;
                if (distanceLeft > targetDistance || leftRadius)
                {
                    leftRadius = true;
                    if (distanceLeft > 0f)
                    {
                        float d = Mathf.Min(speedMultiplier * distanceLeft, maxSpeed);
                        Vector3 b = delta.normalized * d * Time.deltaTime;
                        if (b.magnitude > distanceLeft)
                        {
                            base.transform.position = this.GetFollowTarget(target);
                        }
                        else
                        {
                            base.transform.position = base.transform.position + b;
                        }
                    }
                }
                yield return null;
            }
            yield break;
        }

        private IEnumerator FollowLogicSmoothAccelerate(MapMoveable target)
        {
            float maxRadius = 1000f;
            float maxSpeed = 1000f;
            for (; ; )
            {
                Vector3 currentPos = base.transform.position;
                Vector3 targetPos = target.transform.position;
                Vector3 delta = targetPos - currentPos;
                float dist = delta.magnitude;
                float speed = 0f;
                if (dist > 0f)
                {
                    if (dist > maxRadius)
                    {
                        speed = maxSpeed;
                    }
                    else
                    {
                        float num = dist / maxRadius;
                        speed = maxSpeed * num;
                    }
                }
                float distThisFrame = speed * Time.deltaTime;
                if (distThisFrame > dist)
                {
                    currentPos = targetPos;
                }
                else
                {
                    currentPos += delta.normalized * distThisFrame;
                }
                base.transform.position = currentPos;
                yield return null;
            }
            yield break;
        }

        private IEnumerator FollowLogicNoPathFindingLerpThenLock(MapMoveable target)
        {
            Vector3 start = base.transform.position;
            yield return FiberAnimation.Animate(GardenGameSetup.Get.followDuration, GardenGameSetup.Get.followCurve, delegate (float t)
            {
                this.transform.position = Vector3.Lerp(start, this.GetFollowTarget(target), t);
            }, false);
            for (; ; )
            {
                base.transform.position = this.GetFollowTarget(target);
                yield return null;
            }
            yield break;
        }

        private IEnumerator FollowLogic(MapMoveable target)
        {
            for (; ; )
            {
                float distanceToTarget = (this.GetFollowTarget(target) - base.transform.position).magnitude;
                if (distanceToTarget > 100f)
                {
                    yield return this.MoveToPosWithEvents((Vector3)target.Target + (base.transform.position - target.transform.position).normalized * 100f, target.TargetDirection, false);
                }
                yield return null;
            }
            yield break;
        }

        private void CalculateFollowOffset()
        {
            Bounds bounds;
            if (!BoundsHelper.TryGetRendererBounds(base.gameObject, out bounds))
            {
                this.followOffset = Vector3.zero;
            }
            else
            {
                this.followOffset = bounds.center - base.transform.position;
            }
            this.followOffset.z = 0f;
        }

        private Vector3 GetFollowTarget(MapMoveable target)
        {
            Vector3 result = target.transform.position + target.followOffset;
            result.z = 0f;
            return result;
        }

        [SerializeField]
        private float movementSpeed = 10f;

        [SerializeField]
        private bool disablePathfinding;

        private Vector2 direction;

        private Fiber followFiber;

        private Fiber moveFiber;

        private float panDuration = 1f;

        private MapMoveable.MoveBehaviour desiredMoveBehaviour;

        private MapSpineRootMotion mapSpineRootMotion;

        private bool useRootMotion;

        private MapMoveable followTarget;

        private Vector2 moveTarget;

        private bool isStartingMovement;

        private Vector3 followOffset;

        public enum FollowBehaviour
        {
            LerpThenLock,
            Radius,
            RadiusThenCentre,
            SmoothAccelerate
        }

        public enum MoveBehaviour
        {
            Walk,
            Run
        }
    }
}
