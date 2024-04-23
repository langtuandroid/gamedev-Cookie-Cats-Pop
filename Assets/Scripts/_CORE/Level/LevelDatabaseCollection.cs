using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.TactileCloud.AssetBundles;
using UnityEngine;

public class LevelDatabaseCollection : SingleInstance<LevelDatabaseCollection>, AssetBundleManager.IManagedAssetBundleHandler
{
	public LevelDatabaseCollection(AssetBundleManager assetBundleManager, TactileAnalytics tactileAnalytics)
	{
		this.assetBundleManager = assetBundleManager;
		this.tactileAnalytics = tactileAnalytics;
		this.CacheAllMetaData();
		this.RegisterLevelDatabaseListener("Main");
	}

	protected void CacheAllMetaData()
	{
		string text = "Assets/[Database]/Resources/LevelDatabases/";
		int num = "Assets/[Database]/Resources/LevelDatabases/".IndexOf("Resources/");
		string path = text.Substring(num + 10);
		LevelDatabase[] array = Resources.LoadAll<LevelDatabase>(path);
		foreach (LevelDatabase levelDatabase in array)
		{
			this.cachedResourceLevelDatabases[levelDatabase.GetMapAndLevelsIdentifier()] = levelDatabase;
		}
	}

	public T GetLevelDatabase<T>(MapIdentifier mapIdentifier) where T : LevelDatabase
	{
		if (this.cachedBundledLevelDatabases.ContainsKey(mapIdentifier))
		{
			return this.cachedBundledLevelDatabases[mapIdentifier] as T;
		}
		if (this.managedLevelDatabaseBundles.ContainsKey(mapIdentifier))
		{
			ManagedAssetbundle managedAssetbundle = this.managedLevelDatabaseBundles[mapIdentifier];
			if (managedAssetbundle.CurrentState == ManagedAssetbundle.State.CONSUMED && managedAssetbundle.AssetBundle != null)
			{
				string assetName = LevelDatabase.GetAssetName(mapIdentifier);
				T t = managedAssetbundle.AssetBundle.LoadAsset<T>(assetName);
				if (t != null)
				{
					t.AssetBundleInfo = managedAssetbundle.AssetBundleInfo;
					t.AssetBundle = managedAssetbundle.AssetBundle;
					if (this.cachedBundledLevelDatabases.ContainsKey(mapIdentifier))
					{
						this.cachedBundledLevelDatabases.Remove(mapIdentifier);
					}
					this.cachedBundledLevelDatabases.Add(mapIdentifier, t);
				}
				else
				{
					string errorName = "[LevelDatabaseCollection] Cant find Level database in asset bundle ";
					StackTrace stackTrace = new StackTrace();
					this.tactileAnalytics.LogEvent(new ClientErrorEvent(errorName, stackTrace.ToString(), null, assetName, null, null, null, null, null), -1.0, null);
				}
			}
		}
		foreach (LevelDatabase levelDatabase in this.cachedResourceLevelDatabases.Values)
		{
			if (levelDatabase.GetMapAndLevelsIdentifier() == mapIdentifier)
			{
				return levelDatabase as T;
			}
		}
		return (T)((object)null);
	}

	public void RegisterLevelDatabaseListener(string pMapAndLevelsIdentifier)
	{
		if (!this.managedLevelDatabaseBundles.ContainsKey(pMapAndLevelsIdentifier))
		{
			ManagedAssetbundle value = this.assetBundleManager.RegisterManagedAssetBundleHandler(pMapAndLevelsIdentifier, this);
			this.managedLevelDatabaseBundles.Add(pMapAndLevelsIdentifier, value);
		}
	}

	public void UpdateLevelDatabasesIfAvailable()
	{
		foreach (KeyValuePair<MapIdentifier, ManagedAssetbundle> keyValuePair in this.managedLevelDatabaseBundles)
		{
			this.ConsumeLevelDatabaseBundleIfAvailable(keyValuePair.Key);
		}
	}

	public void ConsumeLevelDatabaseBundleIfAvailable(string pMapIdentifier)
	{
		if (this.managedLevelDatabaseBundles.ContainsKey(pMapIdentifier))
		{
			ManagedAssetbundle managedAssetbundle = this.managedLevelDatabaseBundles[pMapIdentifier];
			if (managedAssetbundle.CurrentState == ManagedAssetbundle.State.AWAITING_CONSUMPTION)
			{
				managedAssetbundle.ConsumeChanges(false);
			}
			else if (managedAssetbundle.CurrentState == ManagedAssetbundle.State.UNAVAILABLE && this.cachedBundledLevelDatabases.ContainsKey(managedAssetbundle.AssetBundleName))
			{
				this.cachedBundledLevelDatabases.Remove(managedAssetbundle.AssetBundleName);
			}
		}
	}

	public void OnStateChanged(ManagedAssetbundle managedAssetbundle, ManagedAssetbundle.State newState)
	{
		if (newState != ManagedAssetbundle.State.AWAITING_CONSUMPTION)
		{
			if (newState == ManagedAssetbundle.State.UNLOADED && this.cachedBundledLevelDatabases.ContainsKey(managedAssetbundle.AssetBundleName))
			{
				this.cachedBundledLevelDatabases.Remove(managedAssetbundle.AssetBundleName);
			}
		}
	}

	private Dictionary<MapIdentifier, LevelDatabase> cachedResourceLevelDatabases = new Dictionary<MapIdentifier, LevelDatabase>();

	private Dictionary<MapIdentifier, LevelDatabase> cachedBundledLevelDatabases = new Dictionary<MapIdentifier, LevelDatabase>();

	private Dictionary<MapIdentifier, ManagedAssetbundle> managedLevelDatabaseBundles = new Dictionary<MapIdentifier, ManagedAssetbundle>();

	public const string META_ASSET_FOLDER = "Assets/[Database]/Resources/LevelDatabases/";

	private AssetBundleManager assetBundleManager;

	private TactileAnalytics tactileAnalytics;
}
