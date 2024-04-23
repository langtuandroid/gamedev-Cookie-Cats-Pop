using System;

namespace TactileModules.UserSupport.View
{
	public interface IBackupRestoreResultView
	{
		event Action Confirmed;

		void Show(string title, string message);
	}
}
