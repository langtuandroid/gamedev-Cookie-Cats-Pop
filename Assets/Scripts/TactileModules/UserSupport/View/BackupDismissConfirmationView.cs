using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace TactileModules.UserSupport.View
{
	public class BackupDismissConfirmationView : UIView, IBackupDismissConfirmationView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Confirmed;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Dismissed;



		void IBackupDismissConfirmationView.Close()
		{
			base.Close(0);
		}

		[UsedImplicitly]
		public void DismissBackup(UIEvent e)
		{
			this.Confirmed();
		}

		[UsedImplicitly]
		public void AbortDismissal(UIEvent e)
		{
			this.Dismissed();
		}
	}
}
