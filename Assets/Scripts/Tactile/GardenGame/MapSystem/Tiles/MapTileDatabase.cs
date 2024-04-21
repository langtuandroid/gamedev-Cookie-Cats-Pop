using System;
using TactileModules.GameCore.StreamingAssets;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Tiles
{
	public class MapTileDatabase : ScriptableObject
	{
		public StreamingAsset GetTexture(int x, int y)
		{
			if (x < 0)
			{
				return null;
			}
			if (y < 0)
			{
				return null;
			}
			if (x >= this.numX)
			{
				return null;
			}
			if (y >= this.numY)
			{
				return null;
			}
			return this.tiles[x + this.numX * y].Texture;
		}

		public void Resize(int numX, int numY)
		{
			this.numX = numX;
			this.numY = numY;
			this.tiles = new MapTileDatabase.Tile[numX * numY];
			for (int i = 0; i < this.tiles.Length; i++)
			{
				this.tiles[i] = new MapTileDatabase.Tile();
			}
		}

		public int numX;

		public int numY;

		public MapTileDatabase.Tile[] tiles;

		[Serializable]
		public class Tile
		{
			[StreamingAssetGroup("MapTiles")]
			public StreamingAsset Texture;
		}
	}
}
