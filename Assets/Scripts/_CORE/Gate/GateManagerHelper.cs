using System;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;

public class GateManagerHelper : GateManager.IGateDataProvider
{
	public LevelDatabase GetMainLevelDatabase()
	{
		return MainProgressionManager.Instance.GetDatabase();
	}

	public GateManager.PersistableState GetPersistableState()
	{
		return UserSettingsManager.Instance.GetSettings<GateManager.PersistableState>();
	}

	public GateConfig GetConfig()
	{
		return ConfigurationManager.Get<GateConfig>();
	}

	public void SaveUsersettings()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}
}
