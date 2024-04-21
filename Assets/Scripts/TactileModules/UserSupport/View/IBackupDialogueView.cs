using System;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.View
{
	public interface IBackupDialogueView
	{
		event Action Dismiss;

		event Action Apply;

		void ShowBackupDetails(BackupSummary currentUserSetting, BackupSummary backedUpUserSetting, string backupDate);

		void Close();
	}
}
