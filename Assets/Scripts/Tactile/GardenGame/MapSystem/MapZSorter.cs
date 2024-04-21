using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[ExecuteInEditMode]
	public class MapZSorter : MapComponent
	{
		public int Order
		{
			get
			{
				return this.order;
			}
		}

		private void OnEnable()
		{
			this.nonBackgroundLayerId = SortingLayer.layers[1].id;
			this.MarkDirty();
		}

		private void LateUpdate()
		{
			if (!base.transform.hasChanged)
			{
				return;
			}
			base.transform.hasChanged = false;
			this.UpdateZOrder();
		}

		public Vector2 Origin()
		{
			return base.transform.TransformPoint(new Vector3(0f, this.GroundPosition, 0f));
		}

		protected virtual int CalculateZOrder()
		{
			return 10000 + Mathf.RoundToInt(-base.transform.TransformPoint(new Vector3(0f, this.GroundPosition, 0f)).y);
		}

		public void UpdateZOrder()
		{
			this.order = this.CalculateZOrder();
			MapZSorter.rendererResults.Clear();
			base.GetComponentsInChildren<Renderer>(MapZSorter.rendererResults);
			for (int i = 0; i < MapZSorter.rendererResults.Count; i++)
			{
				Renderer renderer = MapZSorter.rendererResults[i];
				renderer.sortingOrder = this.order;
				renderer.sortingLayerID = ((!this.isBackground) ? this.nonBackgroundLayerId : 0);
			}
		}

		public void MarkDirty()
		{
			base.transform.hasChanged = true;
		}

		public float GroundPosition;

		public bool isBackground;

		private static readonly List<Renderer> rendererResults = new List<Renderer>();

		private int nonBackgroundLayerId;

		private int order;
	}
}
