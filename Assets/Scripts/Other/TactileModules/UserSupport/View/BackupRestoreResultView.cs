using System;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	public class BackupRestoreResultView : UIView, IBackupRestoreResultView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Confirmed;



		public void Show(string title, string message)
		{
			this.title.text = title;
			this.message.text = message;
		}

		[UsedImplicitly]
		private void Confirm(UIEvent e)
		{
			this.Confirmed();
		}

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UILabel message;
	}
}
