using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapClickable : MapComponent
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapClickable, MapProp> Clicked;

		public MapProp Prop { get; set; }

		public List<Renderer> Renderers
		{
			get
			{
				return this.renderers;
			}
		}

		public Action<MapClickable.VisibilityData> Shown { get; set; }

		public Action<MapClickable.VisibilityData> Hidden { get; set; }

		protected override void Initialized()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.ownerProp = base.GetComponent<MapProp>();
			this.ownerProp.VariationChanged += this.MapPropVariationChanged;
			this.MapPropVariationChanged(this.ownerProp);
		}

		private void MapPropVariationChanged(MapProp mapPropBase)
		{
			MapProp.Variation currentVariation = this.ownerProp.CurrentVariation;
			if (currentVariation == null || !currentVariation.IsPickable)
			{
				return;
			}
			MapClickable.rendererList.Clear();
			base.gameObject.GetComponentsInChildren<Renderer>(MapClickable.rendererList);
			this.renderers.Clear();
			for (int i = 0; i < MapClickable.rendererList.Count; i++)
			{
				this.CreateClickColliderOnRenderer(MapClickable.rendererList[i]);
				this.renderers.Add(MapClickable.rendererList[i]);
			}
		}

		private void CreateClickColliderOnRenderer(Renderer renderer)
		{
			GameObject gameObject = renderer.gameObject;
			if (gameObject.GetComponent<MapClickableVisibilityDelegator>() != null)
			{
				return;
			}
			Mesh mesh = null;
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			if (component != null)
			{
				if (component.sharedMesh == null)
				{
					return;
				}
				mesh = component.sharedMesh;
			}
			MapClickableVisibilityDelegator mapClickableVisibilityDelegator = gameObject.AddComponent<MapClickableVisibilityDelegator>();
			MapZSorter componentInParent = gameObject.GetComponentInParent<MapZSorter>();
			MapClickable.VisibilityData data = new MapClickable.VisibilityData
			{
				Clickable = this,
				Renderer = renderer,
				Bounds = renderer.bounds,
				Mesh = mesh,
				SpriteRenderer = gameObject.GetComponent<SpriteRenderer>(),
				Order = ((!(componentInParent != null)) ? renderer.sortingOrder : componentInParent.Order)
			};
			mapClickableVisibilityDelegator.OnShown = delegate()
			{
				this.Shown(data);
			};
			mapClickableVisibilityDelegator.OnHidden = delegate()
			{
				this.Hidden(data);
			};
		}

		public override int IntializationOrder
		{
			get
			{
				return 1;
			}
		}

		private readonly List<Renderer> renderers = new List<Renderer>();

		private MapProp ownerProp;

		private static readonly List<Renderer> rendererList = new List<Renderer>();

		public class VisibilityData
		{
			public MapClickable Clickable { get; set; }

			public Renderer Renderer { get; set; }

			public Bounds Bounds { get; set; }

			public Mesh Mesh { get; set; }

			public SpriteRenderer SpriteRenderer { get; set; }

			public int Order { get; set; }
		}
	}
}
