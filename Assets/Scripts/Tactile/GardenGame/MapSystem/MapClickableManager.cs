using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile.GardenGame.MapSystem.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapClickableManager
	{
		public MapClickableManager(MapInputArea inputArea, MapAreas areas, MapCamera camera)
		{
			this.inputArea = inputArea;
			this.areas = areas;
			this.camera = camera;
			this.visibleClickables = new HashSet<MapClickable.VisibilityData>();
			this.foundClickables = new List<MapClickable.VisibilityData>();
			this.SubscribeToInput();
			this.AddClickablesToProps();
			this.Enabled = true;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event MapClickableManager.ClickableEvent Down;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event MapClickableManager.ClickableEvent Up;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event MapClickableManager.ClickableEvent Clicked;

		public bool Enabled { get; set; }

		public MapInputArea InputArea
		{
			get
			{
				return this.inputArea;
			}
		}

		private void SubscribeToInput()
		{
			this.inputArea.TouchDown += this.InputAreaOnTouchDown;
			this.inputArea.TouchUp += this.InputAreaOnTouchUp;
		}

		private void InputAreaOnTouchDown(MapInputArea.MouseButtonEvent e)
		{
			if (!this.Enabled)
			{
				return;
			}
			if (e.Index != 0)
			{
				return;
			}
			this.currentClickable = this.FindTopClickableAtLocation(e.GetWorldPosition(this.camera.CachedCamera));
			if (this.currentClickable != null && this.Down != null)
			{
				this.Down(e.GetWorldPosition(this.camera.CachedCamera), this.currentClickable, this.currentClickable.Prop);
			}
			this.cameraMovedWhileTouchDown = 0f;
			this.camera.Moved += this.CameraOnMoved;
		}

		private void InputAreaOnTouchUp(MapInputArea.MouseButtonEvent e)
		{
			if (!this.Enabled)
			{
				return;
			}
			if (e.Index != 0)
			{
				return;
			}
			if (this.Clicked != null && this.cameraMovedWhileTouchDown < 10f)
			{
				MapClickable mapClickable = this.FindTopClickableAtLocation(e.GetWorldPosition(this.camera.CachedCamera));
				if (mapClickable == this.currentClickable)
				{
					this.Clicked(e.GetWorldPosition(this.camera.CachedCamera), this.currentClickable, (!(this.currentClickable != null)) ? null : mapClickable.Prop);
				}
			}
			if (this.currentClickable == null)
			{
				return;
			}
			if (this.Up != null)
			{
				this.Up(e.GetWorldPosition(this.camera.CachedCamera), this.currentClickable, this.currentClickable.Prop);
			}
		}

		private void AddClickablesToProps()
		{
			foreach (MapObjectID mapObjectID in this.areas.IterateComponents<MapObjectID>())
			{
				MapProp component = mapObjectID.gameObject.GetComponent<MapProp>();
				if (!(component == null))
				{
					MapClickable mapClickable = mapObjectID.gameObject.AddComponent<MapClickable>();
					mapClickable.Prop = component;
					mapClickable.Shown = new Action<MapClickable.VisibilityData>(this.MapClickableShown);
					mapClickable.Hidden = new Action<MapClickable.VisibilityData>(this.MapClickableHidden);
				}
			}
		}

		private void MapClickableShown(MapClickable.VisibilityData data)
		{
			this.visibleClickables.Add(data);
		}

		private void MapClickableHidden(MapClickable.VisibilityData data)
		{
			this.visibleClickables.Remove(data);
		}

		private MapClickable FindTopClickableAtLocation(Vector3 worldLocation)
		{
			List<MapClickable.VisibilityData> list = this.FindVisibleClickablesAtLocation(worldLocation);
			if (list.Count == 0)
			{
				return null;
			}
			list.Sort((MapClickable.VisibilityData a, MapClickable.VisibilityData b) => b.Order.CompareTo(a.Order));
			return list[0].Clickable;
		}

		private List<MapClickable.VisibilityData> FindVisibleClickablesAtLocation(Vector3 worldLocation)
		{
			this.foundClickables.Clear();
			Ray ray = new Ray(worldLocation, new Vector3(0f, 0f, 1f));
			foreach (MapClickable.VisibilityData visibilityData in this.visibleClickables)
			{
				if (this.IntersectVisibleClickable(visibilityData, ray))
				{
					this.foundClickables.Add(visibilityData);
				}
			}
			return this.foundClickables;
		}

		private bool IntersectVisibleClickable(MapClickable.VisibilityData data, Ray ray)
		{
			if (!data.Bounds.IntersectRay(ray))
			{
				return false;
			}
			if (data.Mesh != null)
			{
				return MeshHelper.MeshContainsWorldPoint(data.Mesh, data.Renderer.transform, ray.origin);
			}
			if (data.SpriteRenderer == null)
			{
				return false;
			}
			Sprite sprite = data.SpriteRenderer.sprite;
			return !(sprite == null) && MeshHelper.MeshContainsWorldPoint(sprite.vertices, sprite.triangles, data.Renderer.transform, ray.origin);
		}

		private void CameraOnMoved(float deltaDistance)
		{
			this.cameraMovedWhileTouchDown += deltaDistance;
		}

		private readonly MapInputArea inputArea;

		private readonly MapAreas areas;

		private readonly MapCamera camera;

		private readonly HashSet<MapClickable.VisibilityData> visibleClickables;

		private readonly List<MapClickable.VisibilityData> foundClickables;

		private MapClickable currentClickable;

		private float cameraMovedWhileTouchDown;

		public delegate void ClickableEvent(Vector3 worldPosition, MapClickable mapClickable, MapProp mapProp);
	}
}
