using System;
using Tactile;
using TactileModules.Foundation;

public class DeveloperView : UIView
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	private MapStreamerCollection MapStreamerCollection
	{
		get
		{
			return ManagerRepository.Get<MapStreamerCollection>();
		}
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void UseOneLifeClicked(UIEvent e)
	{
		if (InventoryManager.Instance.Lives <= 0)
		{
			InventoryManager.Instance.SetAmount("Life", 2 * ConfigurationManager.Get<LivesConfig>().LifeRegenerationMaxCount, "cheat");
		}
		else
		{
			InventoryManager.Instance.Consume("Life", 1, "cheat");
		}
		UserSettingsManager.Instance.SyncUserSettings();
	}

	private void AddCoinsClicked(UIEvent e)
	{
		InventoryManager.Instance.AddCoins(100, "cheat");
	}

	private void AddFortuneCookieClicked(UIEvent e)
	{
		InventoryManager.Instance.Add("BonusDrop", 15, "cheat");
	}

	private void AddBoostersClicked(UIEvent e)
	{
		foreach (BoosterMetaData boosterMetaData in InventoryManager.Instance.GetAllMetaDataOf<BoosterMetaData>())
		{
			InventoryManager.Instance.Add(boosterMetaData.Id, 5, "cheat");
		}
	}
}
