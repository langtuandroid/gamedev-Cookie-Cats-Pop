using System;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;

public class MainProgressionProvider : MainProgressionManager.IDataProvider
{
	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	public MainProgressionManager.PersistableState GetPersistableState()
	{
		return UserSettingsManager.Instance.GetSettings<MainProgressionManager.PersistableState>();
	}

	public void HandleDeveloperCheated()
	{
		GameEventManager.Instance.Emit(23);
	}

	public void SetPublicFarthestLevelId(int farthestLevelId)
	{
		UserSettingsManager.Get<MainProgressionManager.PublicState>().LatestUnlockedIndex = farthestLevelId;
	}

	public void SaveUserSettings()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	public string GetPersistedKeyForLevelProxy(LevelProxy levelProxy)
	{
		GateMetaData gateMetaData = levelProxy.LevelMetaData as GateMetaData;
		if (gateMetaData != null)
		{
			return "G_" + gateMetaData.gateIndex;
		}
		return "0_" + (this.GetDatabase().GetHumanNumber(levelProxy) - 1);
	}

	public MainLevelDatabase GetDatabase()
	{
		return this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
	}

	public bool IsMetaDataNormal(LevelMetaData metaData)
	{
		return metaData is PuzzleLevelMetaData;
	}
}
