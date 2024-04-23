using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapQuadRenderer))]
	public class MapImage : MapComponent, IMapRenderable
	{
		public bool DisableImage { get; set; }

		public MapQuadRenderer MapQuadRenderer
		{
			get
			{
				if (this.mapQuadRenderer == null)
				{
					this.mapQuadRenderer = base.GetComponent<MapQuadRenderer>();
				}
				return this.mapQuadRenderer;
			}
		}

		public void SetTint(Color tint)
		{
			if (!this.tintEnabled || tint != this.tintColour)
			{
				this.tintEnabled = true;
				this.tintColour = tint;
				this.MapQuadRenderer.Render();
			}
		}

		public void RemoveTint()
		{
			if (this.tintEnabled)
			{
				this.tintEnabled = false;
				this.MapQuadRenderer.Render();
			}
		}

		public Color GetTintColour()
		{
			return this.tintColour;
		}

		public GameObject RenderObject(MapQuadRenderer quadRenderer)
		{
			if (this.DisableImage)
			{
				return null;
			}
			Material material = GardenGameSetup.Get.propMaterial;
			if (this.tintEnabled)
			{
				material = GardenGameSetup.Get.propTintedMaterial;
			}
			if (this.IsLightMask)
			{
				material = GardenGameSetup.Get.lightMaskMaterial;
			}
			GameObject result = quadRenderer.CreateQuad(base.transform, Vector2.zero, this.Size, this.Sprite, this.FlipHorizontally, this.tintEnabled, this.tintColour, material);
			if (this.IsLightMask && Application.isPlaying)
			{
				base.gameObject.SetLayerRecursively(GardenGameSetup.Get.lightingLayer);
			}
			return result;
		}

		public Vector2 Size;

		public bool FlipHorizontally;

		public bool IsLightMask;

		public Sprite Sprite;

		private Color tintColour;

		private bool tintEnabled;

		private MapQuadRenderer mapQuadRenderer;
	}
}
