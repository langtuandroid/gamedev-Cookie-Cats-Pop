using System;
using Tactile;

public class GameStatsManager
{
	private void Save()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	private GameStatsManager.PersistableState GetState()
	{
		return UserSettingsManager.Instance.GetSettings<GameStatsManager.PersistableState>();
	}

	[SettingsProvider("gs", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<GameStatsManager.PersistableState>, IPersistableState
	{
		[JsonSerializable("fr", null)]
		public int NumFreebieUsed { get; set; }

		public void MergeFromOther(GameStatsManager.PersistableState newState, GameStatsManager.PersistableState lastCloudState)
		{
		}
	}
}
