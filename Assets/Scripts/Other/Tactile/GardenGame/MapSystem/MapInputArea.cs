using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapInputArea : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapInputArea.MouseButtonEvent> TouchDown;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapInputArea.MouseButtonEvent> TouchUp;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float> Scrolled;

		public Vector2 GetTouchPosition(int index)
		{
			return this.touches[index].touchPosition;
		}

		private void OnScroll(float delta)
		{
			if (this.Scrolled != null)
			{
				this.Scrolled(delta);
			}
		}

		private void Update()
		{
			for (int i = 0; i < this.touches.Length; i++)
			{
				this.touches[i].isDown = false;
			}
			for (int j = 0; j < UnityEngine.Input.touchCount; j++)
			{
				Touch touch = UnityEngine.Input.GetTouch(j);
				int fingerId = touch.fingerId;
				if (fingerId < this.touches.Length)
				{
					this.touches[fingerId].touchPosition = touch.position;
					this.touches[fingerId].isDown = true;
				}
			}
			for (int k = 0; k < this.touches.Length; k++)
			{
				if (this.touches[k].isDown && !this.prevTouches[k].isDown && (k > 0 || this.IsTouchHittingLayer(this.touches[k].touchPosition)))
				{
					this.touches[k].wasDownSent = true;
					if (this.TouchDown != null)
					{
						this.TouchDown(new MapInputArea.MouseButtonEvent(this.touches[k].touchPosition, k));
					}
				}
			}
			for (int l = 0; l < this.touches.Length; l++)
			{
				if (!this.touches[l].isDown && this.prevTouches[l].isDown && this.touches[l].wasDownSent)
				{
					this.touches[l].wasDownSent = false;
					if (this.TouchUp != null)
					{
						this.TouchUp(new MapInputArea.MouseButtonEvent(this.touches[l].touchPosition, l));
					}
				}
			}
			for (int m = 0; m < this.touches.Length; m++)
			{
				this.prevTouches[m] = this.touches[m];
			}
		}

		private bool IsTouchHittingLayer(Vector2 touchPosition)
		{
			RaycastHit raycastHit = default(RaycastHit);
			return UICamera.Raycast(touchPosition, ref raycastHit) && raycastHit.collider.gameObject.layer == base.gameObject.layer;
		}

		private MapInputArea.InputTouch[] touches = new MapInputArea.InputTouch[3];

		private MapInputArea.InputTouch[] prevTouches = new MapInputArea.InputTouch[3];

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MouseButtonEvent
		{
			public MouseButtonEvent(Vector2 touchPosition, int index)
			{
				this = default(MapInputArea.MouseButtonEvent);
				this.TouchPosition = touchPosition;
				this.Index = index;
			}

			public Vector2 TouchPosition { get; set; }

			public int Index { get; private set; }

			public Vector3 GetWorldPosition(Camera camera)
			{
				return camera.ScreenToWorldPoint(this.TouchPosition);
			}
		}

		private struct InputTouch
		{
			public bool isDown;

			public Vector2 touchPosition;

			public bool wasDownSent;
		}
	}
}
