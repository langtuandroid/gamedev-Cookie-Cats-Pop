using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.Importing
{
	public class MapImagesImporterSettings : ScriptableObject
	{
		public bool TryGetMaxSize(string path, out int maxSize)
		{
			maxSize = 0;
			for (int i = 0; i < this.MaxSizes.Count; i++)
			{
				if (this.MaxSizes[i].Path == path)
				{
					maxSize = this.MaxSizes[i].MaxSize;
					return true;
				}
			}
			return false;
		}

		public List<MapImagesImporterSettings.MaxSizeInfo> MaxSizes = new List<MapImagesImporterSettings.MaxSizeInfo>();

		[Serializable]
		public class MaxSizeInfo
		{
			public string Path
			{
				get
				{
					return string.Empty;
				}
			}

			public Texture2D texture;

			public int MaxSize;
		}
	}
}
