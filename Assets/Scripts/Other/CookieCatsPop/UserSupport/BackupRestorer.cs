using System;
using Tactile;
using TactileModules.UserSupport.DataProviders;

namespace CookieCatsPop.UserSupport
{
	public class BackupRestorer : IUserSupportBackupRestorer
	{
		public BackupRestorer(IUserSettings userSettings)
		{
			this.userSettings = userSettings;
		}

		public bool TryRestore(string backupData)
		{
			return this.userSettings.Restore(backupData);
		}

		private readonly IUserSettings userSettings;
	}
}
