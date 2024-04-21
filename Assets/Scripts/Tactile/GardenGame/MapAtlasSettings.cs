using System;
using UnityEngine;

namespace Tactile.GardenGame
{
	[Serializable]
	public class MapAtlasSettings
	{
		public TextureFormat iOSTextureFormat = TextureFormat.PVRTC_RGBA4;

		public TextureFormat androidTextureFormat = TextureFormat.ASTC_6x6;
	}
}
