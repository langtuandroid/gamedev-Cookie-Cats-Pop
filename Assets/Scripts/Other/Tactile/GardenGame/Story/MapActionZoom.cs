using System;
using System.Collections;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class MapActionZoom : MapAction
	{
		public Rect ZoomTarget
		{
			get
			{
				return this.zoomTarget;
			}
			set
			{
				this.zoomTarget = value;
			}
		}

		public float Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
			}
		}

		public override IEnumerator Logic(IStoryMapController map)
		{
			if (this.Direction == MapActionZoom.ZoomDirection.ZoomIn)
			{
				yield return map.Camera.ZoomIn(this.duration);
			}
			else if (this.Direction == MapActionZoom.ZoomDirection.ZoomOut)
			{
				yield return map.Camera.ZoomOut(this.duration);
			}
			else
			{
				yield return map.Camera.ZoomCustom(this.duration, this.zoomTarget);
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

		public MapActionZoom.ZoomDirection Direction;

		[SerializeField]
		private Rect zoomTarget;

		[SerializeField]
		private float duration;

		public enum ZoomDirection
		{
			ZoomIn,
			ZoomOut,
			Custom
		}
	}
}
