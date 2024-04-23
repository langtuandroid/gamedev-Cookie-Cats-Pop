using System;
using System.Collections;

namespace Tactile
{
	public interface IUserSettings
	{
		event Action<UserSettingsManager> SettingsSynced;

		event Action SettingsSaved;

		T GetSettings<T>() where T : IPersistableState;

		T GetFriendSettings<T>(CloudUser user);

		void SaveLocalSettings();

		void SyncUserSettings();

		void UserSettingsToHashTable(out Hashtable privateSettings, out Hashtable publicSettings);

		bool Restore(string userSettingsJson);

		bool Restore(Hashtable publicSettings, Hashtable privateSettings);

		T GetSettingsFromRawData<T>(string userSettingsJson) where T : IPersistableState;

		T GetSettingsFromRawData<T>(Hashtable publicSettings, Hashtable privateSettings) where T : IPersistableState;
	}
}
