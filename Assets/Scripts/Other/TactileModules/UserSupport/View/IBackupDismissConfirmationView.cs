using System;

namespace TactileModules.UserSupport.View
{
	public interface IBackupDismissConfirmationView
	{
		event Action Confirmed;

		event Action Dismissed;

		void Close();
	}
}
