using System;
using System.Collections;
using System.Collections.Generic;
using Tactile.GardenGame.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace Tactile.GardenGame.MapSystem.Navigation
{
    public class MapNavigationMesh
    {
        public bool IsBuildingMesh()
        {
            return this.updateMeshOperation == null || this.updateMeshOperation.isDone;
        }

        public void Initialize()
        {
            this.navMesh = new NavMeshData();
            this.navMeshInstance = NavMesh.AddNavMeshData(this.navMesh);
        }

        public void Destroy()
        {
            this.navMeshInstance.Remove();
        }

        public void UpdateMesh(MapAreas areas, Bounds groundBounds)
        {
            NavMeshBuildSettings buildSettings = default(NavMeshBuildSettings);
            buildSettings.voxelSize = 4f;
            buildSettings.agentClimb = 0f;
            buildSettings.agentHeight = 1f;
            buildSettings.agentRadius = 10f;
            buildSettings.agentTypeID = 0;
            buildSettings.minRegionArea = 100f;
            List<NavMeshBuildSource> list = new List<NavMeshBuildSource>();
            Bounds localBounds = groundBounds;
            localBounds.center = this.SwapYZ(localBounds.center);
            localBounds.extents = this.SwapYZ(localBounds.extents);
            localBounds.extents += new Vector3(0f, 100f, 0f);
            list.Add(new NavMeshBuildSource
            {
                area = 0,
                shape = NavMeshBuildSourceShape.Box,
                size = new Vector3(localBounds.size.x * 2f, 0f, localBounds.size.z * 2f),
                transform = Matrix4x4.identity
            });
            foreach (MapCollision mapCollision in areas.IterateComponents<MapCollision>())
            {
                Mesh extrudedOutlineMesh = mapCollision.GetExtrudedOutlineMesh(10f, mapCollision.transform.localScale);
                if (!(extrudedOutlineMesh == null))
                {
                    Vector3 position = mapCollision.transform.position;
                    position.z = position.y;
                    position.y = 0f;
                    list.Add(new NavMeshBuildSource
                    {
                        area = 0,
                        shape = NavMeshBuildSourceShape.Mesh,
                        sourceObject = extrudedOutlineMesh,
                        size = extrudedOutlineMesh.bounds.size,
                        transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one)
                    });
                }
            }
            this.updateMeshOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.navMesh, buildSettings, list, localBounds);
        }

        private Vector3 SwapYZ(Vector3 v)
        {
            return new Vector3(v.x, v.z, v.y);
        }

        private float CalulateTotalLength(Vector3[] corners)
        {
            float num = 0f;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Vector3 vector = corners[i];
                Vector3 vector2 = corners[i + 1];
                Vector2 a = new Vector2(vector.x, vector.z);
                Vector2 b = new Vector2(vector2.x, vector2.z);
                num += Vector2.Distance(a, b);
            }
            return num;
        }

        public List<Vector2> GetPath(Vector2 start, Vector2 destination)
        {
            List<Vector2> list = new List<Vector2>();
            Vector3 sourcePosition = new Vector3(start.x, 0f, start.y);
            Vector3 targetPosition = new Vector3(destination.x, 0f, destination.y);
            NavMeshPath navMeshPath = new NavMeshPath();
            if (!NavMesh.CalculatePath(sourcePosition, targetPosition, -1, navMeshPath))
            {
                return null;
            }
            Vector3[] corners = navMeshPath.corners;
            float num = 30f;
            if (GardenGameSetup.Get.pathFindingSetUp.onlyFollowIsometricDirections)
            {
                int i = 0;
                while (i < corners.Length - 1)
                {
                    if (i >= corners.Length - 1)
                    {
                        goto IL_C4;
                    }
                    float magnitude = (corners[i] - corners[i + 1]).magnitude;
                    if (magnitude >= num)
                    {
                        goto IL_C4;
                    }
                    IL_1AE:
                    i++;
                    continue;
                    IL_C4:
                    Vector3 v = new Vector2(corners[i].x, corners[i].z);
                    Vector3 v2 = new Vector2(corners[i + 1].x, corners[i + 1].z);
                    List<Vector2> list2 = new List<Vector2>();
                    this.CalculateIsometricWaypoints(v, v2, list2);
                    for (int j = 0; j < list2.Count; j++)
                    {
                        if (list.Count > 0)
                        {
                            Vector2 b = list[list.Count - 1];
                            float magnitude2 = (list2[j] - b).magnitude;
                            if (magnitude2 < num)
                            {
                                list.RemoveAt(list.Count - 1);
                            }
                        }
                        list.Add(list2[j]);
                    }
                    goto IL_1AE;
                }
            }
            else
            {
                for (int k = 0; k < corners.Length; k++)
                {
                    list.Add(new Vector2(corners[k].x, corners[k].z));
                }
            }
            return list;
        }

        private void CalculateIsometricWaypoints(Vector2 start, Vector2 target, List<Vector2> steps)
        {
            if (steps == null)
            {
                steps = new List<Vector2>();
            }
            if (steps.Count == 0 && MapCollision.RayCast(start, target))
            {
                steps.Add(target);
                return;
            }
            List<Vector2> cardinalOnlyPath = IsometricUtils.GetCardinalOnlyPath(start, target, GardenGameSetup.Get.pathFindingSetUp.GetAICardinalDirectionThreshold((start - target).magnitude));
            if (MapCollision.IsPathValid(cardinalOnlyPath))
            {
                for (int i = 0; i < cardinalOnlyPath.Count; i++)
                {
                    steps.Add(cardinalOnlyPath[i]);
                }
            }
            else
            {
                float num = 0.9f;
                Vector2 vector;
                for (; ; )
                {
                    vector = Vector2.Lerp(start, target, num);
                    cardinalOnlyPath = IsometricUtils.GetCardinalOnlyPath(start, vector, GardenGameSetup.Get.pathFindingSetUp.GetAICardinalDirectionThreshold((start - vector).magnitude));
                    float magnitude = (vector - start).magnitude;
                    if (magnitude < 50f || MapCollision.IsPathValid(cardinalOnlyPath))
                    {
                        break;
                    }
                    num *= 0.9f;
                }
                for (int j = 0; j < cardinalOnlyPath.Count; j++)
                {
                    steps.Add(cardinalOnlyPath[j]);
                }
                this.CalculateIsometricWaypoints(vector, target, steps);
            }
        }

        public IEnumerator PathFind(Vector2 start, Vector2 destination, Vector2 targetDirection, bool ignoreTargetDirection, float speed, Action<Vector2, Vector2> moved, Action<float, float> location)
        {

            Vector3 sourcePosition = new Vector3(start.x, 0f, start.y);
            Vector3 destPosition = new Vector3(destination.x, 0f, destination.y);
            NavMeshPath path = new NavMeshPath();
            Vector3[] corners;
            if (!NavMesh.CalculatePath(sourcePosition, destPosition, -1, path))
            {
                corners = new Vector3[]
                {
                    sourcePosition,
                    destPosition
                };
            }
            else
            {
                corners = path.corners;
            }
            float totalLength = this.CalulateTotalLength(corners);
            float currentLength = 0f;
            for (int i = 0; i < corners.Length - 1; i++)
            {

                Vector3 p = corners[i];
                Vector3 p2 = corners[i + 1];
                Vector2 pos1 = new Vector2(p.x, p.z);
                Vector2 pos2 = new Vector2(p2.x, p2.z);
                Vector2 direction = pos2 - pos1;
                float distance = direction.magnitude;

                direction.x = direction.x / distance;

                direction.y = direction.y / distance;
                float prevLength = currentLength;
                currentLength += distance;
                yield return FiberAnimation.Animate(distance / speed, delegate (float t)
                {
                    Vector2 arg = Vector2.Lerp(pos1, pos2, t);
                    moved(arg, direction);
                    location(totalLength, Mathf.Lerp(prevLength, currentLength, t));
                });
            }
            if (!ignoreTargetDirection)
            {
                Vector3 vector = corners[corners.Length - 1];
                moved(new Vector2(vector.x, vector.z), targetDirection);
            }
            yield break;
        }

        private NavMeshData navMesh;

        private NavMeshDataInstance navMeshInstance;

        private AsyncOperation updateMeshOperation;
    }
}
