using System;

public class LevelAnalyticsData
{
	public LevelAnalyticsData(LevelProxy levelProxy)
	{
		LevelAssetBase levelAssetBase = levelProxy.LevelAsset as LevelAssetBase;
		this.databaseAssetBundleName = levelProxy.RootDatabase.AssetBundleFilenameForAnalytics;
		this.databaseAssetBundleUrl = levelProxy.RootDatabase.AssetBundleURL;
		this.isHard = (levelProxy.LevelDifficulty == LevelDifficulty.Hard);
		this.levelCollectionPath = levelProxy.LevelCollectionPath;
		this.levelDatabaseName = levelProxy.RootDatabase.name;
		this.levelDatabaseHash = levelProxy.RootDatabase.GetContentHash();
		this.levelDatabaseRevision = levelProxy.RootDatabase.DatabaseRevision;
		this.levelFilename = levelProxy.LevelAsset.name;
		this.levelHash = ((!(levelAssetBase != null)) ? string.Empty : levelAssetBase.ContentHash);
		this.levelIndexInCollection = levelProxy.Index;
		this.levelMapPosition = levelProxy.RootDatabase.GetLevelProgress(levelProxy);
		this.levelNumber = levelProxy.RootDatabase.GetHumanNumber(levelProxy);
	}

	public static LevelAnalyticsData Create(LevelProxy levelProxy)
	{
		return new LevelAnalyticsData(levelProxy);
	}

	public readonly string databaseAssetBundleName;

	public readonly string databaseAssetBundleUrl;

	public readonly bool isHard;

	public readonly string levelCollectionPath;

	public readonly string levelDatabaseHash;

	public readonly string levelDatabaseName;

	public readonly int levelDatabaseRevision;

	public readonly string levelFilename;

	public readonly string levelHash;

	public readonly int levelIndexInCollection;

	public readonly double levelMapPosition;

	public readonly int levelNumber;
}
