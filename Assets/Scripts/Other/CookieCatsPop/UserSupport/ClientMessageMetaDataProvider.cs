using System;
using System.Collections;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.UserSupport.DataProviders;

namespace CookieCatsPop.UserSupport
{
	public class ClientMessageMetaDataProvider : IClientMessageMetaDataProvider
	{
		public ClientMessageMetaDataProvider(IUserSettings userSettings, MainProgressionManager mainProgressionManager)
		{
			this.userSettings = userSettings;
			this.mainProgressionManager = mainProgressionManager;
		}

		public void AddUserSettings(out Hashtable privateUserSettings, out Hashtable publicUserSettings)
		{
			this.userSettings.UserSettingsToHashTable(out privateUserSettings, out publicUserSettings);
		}

		public void AddCustomData(out Hashtable customData)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["FarthestCompletedLevelHumanNumber"] = this.mainProgressionManager.GetFarthestCompletedLevelHumanNumber();
			customData = hashtable;
		}

		private readonly IUserSettings userSettings;

		private readonly MainProgressionManager mainProgressionManager;
	}
}
