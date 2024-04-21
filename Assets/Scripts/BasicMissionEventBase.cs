using System;
using TactileModules.PuzzleCore.LevelPlaying;
using UnityEngine;

public class BasicMissionEventBase : BasicEvent
{
	protected BasicMissionEventBase(ILevelSessionRunner levelSession)
	{
		LevelInformation levelInformation = levelSession.LevelInformation;
		LevelProxy levelProxy = levelSession.LevelProxy;
		LevelAnalyticsData levelAnalyticsData = LevelAnalyticsData.Create(levelSession.LevelProxy);
		this.AssetBundleName = levelAnalyticsData.databaseAssetBundleName;
		this.AssetBundleUrl = levelAnalyticsData.databaseAssetBundleUrl;
		this.IsHard = this.IsLevelHard(levelProxy);
		this.IsTutorial = levelSession.LevelInformation.IsTutorial;
		this.LevelCollection = ((levelProxy.LevelCollection != null) ? ((ScriptableObject)levelProxy.LevelCollection).name : "null");
		this.LevelDatabaseHash = levelAnalyticsData.levelDatabaseHash;
		this.LevelDatabaseName = levelAnalyticsData.levelDatabaseName;
		this.LevelDatabaseRevision = levelAnalyticsData.levelDatabaseRevision;
		this.LevelFilename = levelAnalyticsData.levelFilename;
		this.LevelGoalPiecesTotal = levelInformation.TotalGoalPieces;
		this.LevelHash = levelAnalyticsData.levelHash;
		this.LevelNumber = levelAnalyticsData.levelNumber;
		this.LevelType = levelInformation.LevelType;
	}

	private TactileAnalytics.RequiredParam<string> AssetBundleName { get; set; }

	private TactileAnalytics.RequiredParam<string> AssetBundleUrl { get; set; }

	private TactileAnalytics.RequiredParam<int> AvailableMainMapLevels { get; set; }

	private TactileAnalytics.OptionalParam<bool> IsHard { get; set; }

	private TactileAnalytics.OptionalParam<bool> IsTutorial { get; set; }

	private TactileAnalytics.OptionalParam<string> LevelCollection { get; set; }

	private TactileAnalytics.RequiredParam<string> LevelDatabaseHash { get; set; }

	private TactileAnalytics.RequiredParam<string> LevelDatabaseName { get; set; }

	private TactileAnalytics.RequiredParam<int> LevelDatabaseRevision { get; set; }

	private TactileAnalytics.RequiredParam<string> LevelFilename { get; set; }

	private TactileAnalytics.OptionalParam<int> LevelGoalPiecesTotal { get; set; }

	private TactileAnalytics.RequiredParam<string> LevelHash { get; set; }

	private TactileAnalytics.RequiredParam<int> LevelNumber { get; set; }

	private TactileAnalytics.OptionalParam<string> LevelType { get; set; }

	public void SetAvailableMainMapLevels(int availableLevels)
	{
		this.AvailableMainMapLevels = availableLevels;
	}

	private bool IsLevelHard(LevelProxy levelProxy)
	{
		return levelProxy.IsValid && levelProxy.LevelDifficulty == LevelDifficulty.Hard;
	}

	private TactileAnalytics.OptionalParam<int> GateNumber { get; set; }

	public void SetGateNumber(int gateNumber)
	{
		this.GateNumber = gateNumber;
	}
}
