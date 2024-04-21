using System;
using System.Collections.Generic;
using UnityEngine;

[SingletonAssetPath("Assets/[TextureStreamerSettings]/Resources/TextureStreamerSettings.asset")]
public class TextureStreamerSettings : SingletonAsset<TextureStreamerSettings>
{
	[SerializeField]
	[HideInInspector]
	public List<TextureStreamerSettings.TextureStreamerElement> assetBundlesToCacheAtLevelUnlock = new List<TextureStreamerSettings.TextureStreamerElement>();

	[Serializable]
	public class TextureStreamerElement
	{
		public int cacheAtLevelNr;

		public string assetBundleName;
	}
}
