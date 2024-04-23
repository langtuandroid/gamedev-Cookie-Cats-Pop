using System;
using System.Diagnostics;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapInteraction : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Vector2> MapClicked;

		public void SetCamera(MapCamera mapCamera)
		{
			this.mapCamera = mapCamera;
			this.unityCam = mapCamera.CachedCamera;
		}

		private void OnDrag(Vector2 delta)
		{
			if (!base.enabled)
			{
				return;
			}
			this.draggedDistance += delta.magnitude;
			if (UnityEngine.Input.touchCount != 2 && !this.blockPan)
			{
				Vector3 b = this.unityCam.ScreenToWorldPoint(Vector3.zero);
				Vector3 a = this.unityCam.ScreenToWorldPoint(delta);
				Vector3 a2 = a - b;
				this.mapCamera.velocity = -a2 / Time.deltaTime;
			}
		}

		private void Update()
		{
			if (UnityEngine.Input.touchCount == 2)
			{
				this.blockPan = true;
				this.mapCamera.velocity = Vector2.zero;
				Touch touch = UnityEngine.Input.GetTouch(0);
				float magnitude = (UnityEngine.Input.GetTouch(1).position - touch.position).magnitude;
				if (this.lastPinchDistance == null)
				{
					this.lastPinchDistance = new float?(magnitude);
					this.lastPinchOrthoSize = this.unityCam.orthographicSize;
				}
				float num = this.lastPinchDistance.Value / this.lastPinchOrthoSize;
				float num2 = magnitude / this.unityCam.orthographicSize;
				this.unityCam.orthographicSize *= num / num2;
				this.mapCamera.ClampZoom();
				this.lastPinchDistance = new float?(magnitude);
				this.lastPinchOrthoSize = this.unityCam.orthographicSize;
			}
			else if (UnityEngine.Input.touchCount == 0)
			{
				this.blockPan = false;
				this.lastPinchDistance = null;
			}
			else
			{
				this.lastPinchDistance = null;
			}
		}

		private void OnScroll(float delta)
		{
			if (!base.enabled)
			{
				return;
			}
			this.mapCamera.NormalizedZoom += delta * 0.25f;
		}

		private void OnReleaseAtPos(Vector2 pos)
		{
			if (!base.enabled)
			{
				return;
			}
			if (this.draggedDistance < 10f)
			{
				UICamera uicamera = UICamera.FindCameraForLayer(base.gameObject.layer);
				Ray ray = this.unityCam.ScreenPointToRay(pos);
				RaycastHit raycastHit;
				if (uicamera.RaycastPastObject(ray, base.gameObject, out raycastHit))
				{
					raycastHit.collider.gameObject.SendMessage("OnClick", null, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					float distance = -ray.origin.z / ray.direction.z;
					if (this.MapClicked != null)
					{
						this.MapClicked(ray.GetPoint(distance));
					}
				}
			}
			this.draggedDistance = 0f;
		}

		private MapCamera mapCamera;

		private Camera unityCam;

		private float? lastPinchDistance;

		private float lastPinchOrthoSize;

		private bool blockPan;

		private float draggedDistance;
	}
}
