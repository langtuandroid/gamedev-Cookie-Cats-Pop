using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Shared.UserSupport.Module.ViewComponents;
using TactileModules.UserSupport.Model;
using UnityEngine;

namespace TactileModules.UserSupport.View
{
	internal class BackupDialogueView : UIView, IBackupDialogueView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Dismiss;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Apply;



		public void ShowBackupDetails(BackupSummary currentUserSettings, BackupSummary backedUpUserSettings, string backupDate)
		{
			this.currentUserSettingsRenderer = this.currentUserSettingsInstanciator.GetInstance<BackupDetailsRenderer>();
			this.backedUpUserSettingsRenderer = this.backedUpUserSettingsInstantiator.GetInstance<BackupDetailsRenderer>();
			this.currentUserSettingsRenderer.Render(L.Get("Current"), currentUserSettings);
			this.backedUpUserSettingsRenderer.Render(L.Get("Cloud"), backedUpUserSettings);
			this.backupDateLabel.text = L.Get("Backed up: ") + backupDate;
		}

		void IBackupDialogueView.Close()
		{
			base.Close(1);
		}

		[UsedImplicitly]
		public void Applied(UIEvent e)
		{
			this.Apply();
			base.Close(1);
		}

		[UsedImplicitly]
		public void Dismissed(UIEvent e)
		{
			this.Dismiss();
		}

		[SerializeField]
		private UIInstantiator currentUserSettingsInstanciator;

		[SerializeField]
		private UIInstantiator backedUpUserSettingsInstantiator;

		[SerializeField]
		private UILabel backupDateLabel;

		private BackupDetailsRenderer currentUserSettingsRenderer;

		private BackupDetailsRenderer backedUpUserSettingsRenderer;
	}
}
