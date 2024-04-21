using System;
using System.Collections.Generic;
using UnityEngine;

namespace TactileModules.Portraits
{
	[SingletonAssetPath("Assets/[ModuleAssets]/Resources/Portraits/PortraitsDatabase.asset")]
	public class PortraitsTextureDatabase : SingletonAsset<PortraitsTextureDatabase>
	{
		public List<Texture2D> portraitTextures;
	}
}
