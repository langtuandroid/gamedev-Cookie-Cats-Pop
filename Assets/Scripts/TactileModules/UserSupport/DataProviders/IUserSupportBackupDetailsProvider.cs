using System;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.DataProviders
{
	public interface IUserSupportBackupDetailsProvider
	{
		BackupSummary GetCurrentUserSettingsDetails();

		BackupSummary GetNewBackupSummary(string rawBackupData);
	}
}
