using System;

namespace TactileModules.UserSupport.DataProviders
{
	public interface IUserSupportBackupRestorer
	{
		bool TryRestore(string backupData);
	}
}
