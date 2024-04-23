using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Tiles
{
	public class MapTileCamera : MonoBehaviour
	{
		private void LateUpdate()
		{
			this.tileRenderer.Render(this.camera);
		}

		public void Initialize(Camera camera, MapTileRenderer tileRenderer)
		{
			this.camera = camera;
			this.tileRenderer = tileRenderer;
		}

		private Camera camera;

		private MapTileRenderer tileRenderer;
	}
}
