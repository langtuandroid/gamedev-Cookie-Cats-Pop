using System;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.UserSupport.DataProviders;
using TactileModules.UserSupport.Model;

namespace CookieCatsPop.UserSupport
{
	public class UserSettingsBackupSummaryProvider : IUserSupportBackupDetailsProvider
	{
		public UserSettingsBackupSummaryProvider(IUserSettings userSettings, InventoryManager inventoryManager)
		{
			this.userSettings = userSettings;
			this.inventoryManager = inventoryManager;
		}

		public BackupSummary GetCurrentUserSettingsDetails()
		{
			MainProgressionManager.PersistableState settings = this.userSettings.GetSettings<MainProgressionManager.PersistableState>();
			return new BackupSummary
			{
				Coins = this.inventoryManager.Coins,
				MaxReachedLevel = settings.FarthestCompletedLevelId
			};
		}

		public BackupSummary GetNewBackupSummary(string rawBackupData)
		{
			MainProgressionManager.PersistableState settingsFromRawData = this.userSettings.GetSettingsFromRawData<MainProgressionManager.PersistableState>(rawBackupData);
			InventoryManager.PersistableState settingsFromRawData2 = this.userSettings.GetSettingsFromRawData<InventoryManager.PersistableState>(rawBackupData);
			int coins;
			settingsFromRawData2.items.TryGetValue("Coin", out coins);
			return new BackupSummary
			{
				MaxReachedLevel = settingsFromRawData.FarthestCompletedLevelId,
				Coins = coins
			};
		}

		private readonly IUserSettings userSettings;

		private readonly InventoryManager inventoryManager;
	}
}
