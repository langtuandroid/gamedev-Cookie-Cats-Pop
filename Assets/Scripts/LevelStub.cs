using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LevelStub : ILevelStubPrivateAccess
{
	LevelMetaData ILevelStubPrivateAccess.MetaData { get; set; }

	string ILevelStubPrivateAccess.MetaClassType
	{
		get
		{
			return this.metaClassType;
		}
		set
		{
			this.metaClassType = value;
		}
	}

	string ILevelStubPrivateAccess.MetaDataAsJson
	{
		get
		{
			return this.metaDataAsJson;
		}
		set
		{
			this.metaDataAsJson = value;
		}
	}

	string ILevelStubPrivateAccess.AssetResourcePath
	{
		get
		{
			return this.assetResourcePath;
		}
		set
		{
			this.assetResourcePath = value;
		}
	}

	public string AssetResourcePath
	{
		get
		{
			return this.assetResourcePath;
		}
	}

	public bool IsLoaded
	{
		get
		{
			return LevelStub.ListOfLoadedAssets.Exists((LevelStub x) => x.assetResourcePath == this.assetResourcePath);
		}
	}

	private static List<LevelStub> ListOfLoadedAssets
	{
		get
		{
			if (LevelStub.listOfLoadedAssets == null)
			{
				LevelStub.listOfLoadedAssets = new List<LevelStub>();
			}
			return LevelStub.listOfLoadedAssets;
		}
	}

	public static void UnloadAllLevels()
	{
		foreach (LevelStub levelStub in LevelStub.ListOfLoadedAssets)
		{
			levelStub.Unload();
		}
		LevelStub.ListOfLoadedAssets.Clear();
	}

	public LevelCollectionEntry GetLevelAsset()
	{
		if (this.loadedLevelAsset == null)
		{
			throw new InvalidOperationException("No level was loaded!");
		}
		return this.loadedLevelAsset;
	}

	public void Load(ILevelCollection levelCollection)
	{
		if (this.loadedLevelAsset != null)
		{
			return;
		}
		if (levelCollection == null)
		{
			throw new InvalidOperationException("Could not cast parent to level collection!");
		}
		if (levelCollection.AssetBundle == null)
		{
			this.loadedLevelAsset = this.LoadAssetFromResources();
		}
		else
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.assetResourcePath);
			this.loadedLevelAsset = levelCollection.AssetBundle.LoadAsset<LevelCollectionEntry>(fileNameWithoutExtension);
			if (this.loadedLevelAsset is LevelGroupAsset)
			{
				(this.loadedLevelAsset as LevelGroupAsset).AssetBundle = levelCollection.AssetBundle;
			}
		}
		if (this.loadedLevelAsset == null)
		{
			throw new InvalidOperationException("Failed loading level!");
		}
		if (!this.IsLoaded)
		{
			LevelStub.ListOfLoadedAssets.Add(this);
		}
		if (Application.isPlaying)
		{
		}
	}

	public void Unload()
	{
		if (this.loadedLevelAsset == null)
		{
			throw new InvalidOperationException("No level was loaded!");
		}
		if (Application.isPlaying)
		{
		}
		Resources.UnloadAsset(this.loadedLevelAsset);
		this.loadedLevelAsset = null;
	}

	private LevelCollectionEntry LoadAssetFromResources()
	{
		string text = "/Resources/";
		if (string.IsNullOrEmpty(this.assetResourcePath))
		{
			throw new Exception("AssetResourcePath is empty. Something went wrong during serialization of the related ILevelCollection.");
		}
		if (!this.assetResourcePath.Contains(text))
		{
			throw new Exception("Trying to resource load a level asset which is not located within a resource folder! path=" + this.assetResourcePath);
		}
		int count = this.assetResourcePath.IndexOf(text) + text.Length;
		string path = this.assetResourcePath.Remove(0, count);
		string path2 = Path.ChangeExtension(path, null);
		return Resources.Load<LevelCollectionEntry>(path2);
	}

	[SerializeField]
	private string metaClassType;

	[SerializeField]
	private string metaDataAsJson;

	[SerializeField]
	private string assetResourcePath;

	private LevelCollectionEntry loadedLevelAsset;

	private LevelMetaData metaData;

	public int humanNumber = -1;

	public int index = -1;

	public int[] starThresholds;

	private static List<LevelStub> listOfLoadedAssets;
}
