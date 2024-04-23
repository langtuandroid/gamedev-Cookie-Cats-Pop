using System;
using System.Collections;
using UnityEngine;

namespace Tactile
{
	public static class MaskOverlayExtensions
	{
		public static Camera FindCameraFromGO(GameObject gameObject)
		{
			Camera[] allCameras = Camera.allCameras;
			int num = 1 << gameObject.layer;
			for (int i = 0; i < allCameras.Length; i++)
			{
				if (((allCameras[i].cullingMask & num) == allCameras[i].cullingMask || allCameras[i].cullingMask == -1) && allCameras[i].cullingMask != 0)
				{
					return allCameras[i];
				}
			}
			return null;
		}

		private static Vector3[] GetWorldSpaceVerticesFromMeshFilter(MeshFilter meshFilter)
		{
			Mesh sharedMesh = meshFilter.sharedMesh;
			Vector3[] array = new Vector3[sharedMesh.vertices.Length];
			Transform transform = meshFilter.transform;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = transform.TransformPoint(sharedMesh.vertices[i]);
			}
			return array;
		}

		private static void GetCircumscripedCircleFromPoints(Vector3[] points, out Vector3 center, out float radius)
		{
			center = Vector3.zero;
			radius = 0f;
			if (points == null || points.Length == 0)
			{
				return;
			}
			for (int i = 0; i < points.Length; i++)
			{
				center += points[i];
			}
			center *= 1f / (float)points.Length;
			for (int j = 0; j < points.Length; j++)
			{
				radius = Mathf.Max(radius, (points[j] - center).sqrMagnitude);
			}
			radius = Mathf.Sqrt(radius);
		}

		public static void AddUIElements(this MaskOverlay overlay, string layer, params UIElement[] elements)
		{
			foreach (UIElement uielement in elements)
			{
				MaskOverlayCutout maskOverlayCutout = overlay.AddCutout(layer);
				maskOverlayCutout.Camera = MaskOverlayExtensions.FindCameraFromGO(uielement.gameObject);
				maskOverlayCutout.WorldPosition = uielement.transform.position;
				maskOverlayCutout.WorldSize = uielement.Size;
			}
		}

		public static void SetOvalFromMesh(this MaskOverlayCutout cutout, MeshFilter meshFilter)
		{
			Vector3[] worldSpaceVerticesFromMeshFilter = MaskOverlayExtensions.GetWorldSpaceVerticesFromMeshFilter(meshFilter);
			if (worldSpaceVerticesFromMeshFilter == null || worldSpaceVerticesFromMeshFilter.Length == 0)
			{
				return;
			}
			Vector3 worldPosition;
			float num;
			MaskOverlayExtensions.GetCircumscripedCircleFromPoints(worldSpaceVerticesFromMeshFilter, out worldPosition, out num);
			cutout.Camera = MaskOverlayExtensions.FindCameraFromGO(meshFilter.gameObject);
			cutout.WorldPosition = worldPosition;
			cutout.WorldSize = new Vector2(num * 2f, num * 2f);
			cutout.Oval = true;
		}

		public static void SetOvalShapeFromMesh(this MaskOverlayCutout cutout, Component component)
		{
			cutout.SetOvalFromMesh(component.gameObject.GetComponent<MeshFilter>());
		}

		public static void SetOvalFromGameObject(this MaskOverlayCutout cutout, GameObject gameObject, Vector2 worldSize)
		{
			cutout.Camera = MaskOverlayExtensions.FindCameraFromGO(gameObject);
			cutout.WorldPosition = gameObject.transform.position;
			cutout.WorldSize = worldSize;
			cutout.Oval = true;
		}

		public static void SetRectFromGameobject(this MaskOverlayCutout cutout, GameObject gameObject, Vector2 worldSize)
		{
			cutout.Camera = MaskOverlayExtensions.FindCameraFromGO(gameObject);
			cutout.WorldPosition = gameObject.transform.position;
			cutout.WorldSize = worldSize;
			cutout.Oval = false;
		}

		public static void AddCapsuleCutout(this MaskOverlay overlay, string layer, UIWidget widget, Vector3 endPosition, int segments = 3)
		{
			Vector3[] worldSpaceVerticesFromMeshFilter = MaskOverlayExtensions.GetWorldSpaceVerticesFromMeshFilter(widget.GetComponent<MeshFilter>());
			if (worldSpaceVerticesFromMeshFilter == null || worldSpaceVerticesFromMeshFilter.Length == 0)
			{
				return;
			}
			Vector3 a;
			float num;
			MaskOverlayExtensions.GetCircumscripedCircleFromPoints(worldSpaceVerticesFromMeshFilter, out a, out num);
			float num2 = 1f / (float)(segments - 1);
			for (int i = 0; i < segments; i++)
			{
				MaskOverlayCutout maskOverlayCutout = overlay.AddCutout(layer);
				maskOverlayCutout.Camera = MaskOverlayExtensions.FindCameraFromGO(widget.gameObject);
				maskOverlayCutout.WorldPosition = Vector3.Lerp(a, endPosition, (float)i * num2);
				maskOverlayCutout.WorldSize = new Vector2(num * 2f, num * 2f);
				maskOverlayCutout.Oval = true;
			}
		}

		public static IEnumerator AnimateAlpha(this MaskOverlay overlay, float source, float dest, float duration, AnimationCurve curve = null)
		{
			yield return FiberAnimation.Animate(duration, curve, delegate(float p)
			{
				overlay.Alpha = source + (dest - source) * p;
			}, false);
			yield break;
		}

		public static IEnumerator AnimateHighlightUIElement(this MaskOverlayCutout cutout, UIElement element, float duration, AnimationCurve curve = null)
		{
			cutout.Camera = MaskOverlayExtensions.FindCameraFromGO(element.gameObject);
			cutout.Oval = true;
			cutout.WorldSize = element.Size;
			cutout.WorldPosition = element.transform.position;
			Vector2 startSize = new Vector2(4f, 4f);
			Vector2 endSize = cutout.ViewportSize;
			Vector2 startPosition = new Vector2(0.5f, 0.5f);
			Vector2 endPosition = cutout.ViewportPosition;
			yield return FiberAnimation.Animate(duration, curve, delegate(float t)
			{
				cutout.ViewportPosition = Vector2.Lerp(startPosition, endPosition, t);
				cutout.ViewportSize = Vector2.Lerp(startSize, endSize, t);
			}, false);
			yield break;
		}

		public static void SetDepthAboveView(this MaskOverlay overlay, UIView view)
		{
			UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(view);
			if (viewLayerWithView == null)
			{
				return;
			}
			overlay.Depth = viewLayerWithView.ViewCamera.depth + 0.05f;
		}
	}
}
