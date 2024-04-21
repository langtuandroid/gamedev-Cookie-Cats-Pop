using System;
using TactileModules.RuntimeTools;
using UnityEngine;

[SingletonAssetPath("Assets/[Effects]/Resources/EffectPoolSettings.asset")]
public class EffectPoolSettings : SingletonAsset<EffectPoolSettings>
{
	public string EffectPrefabFolderAssetPath
	{
		get
		{
			return this.effectPrefabsFolderAssetPath;
		}
	}

	public string EffectPrefabFolderResourcePath
	{
		get
		{
			return AssetPathUtility.AssetPathToResourcePath(this.effectPrefabsFolderAssetPath);
		}
	}

	public bool DirectInstantiation
	{
		get
		{
			return false;
		}
	}

	[SerializeField]
	private string effectPrefabsFolderAssetPath = "Assets/Resources/Effects/";

	[SerializeField]
	[Tooltip("Enable if you need to tweak an effect while playing.")]
	private bool directInstantiation;

	public float placementInWorldZ = -10f;
}
