using System;
using System.Diagnostics;
using TactileModules.Analytics.Interfaces;
using TactileModules.UserSupport;
using TactileModules.UserSupport.Cloud;
using TactileModules.UserSupport.DataProviders;
using TactileModules.UserSupport.Model;
using TactileModules.UserSupport.View;
using TactileModules.UserSupport.ViewControllers;
using UnityEngine;

namespace Shared.UserSupport.Module.ViewControllers
{
	public class BackupDialogueViewController : IUIViewController
	{
		public BackupDialogueViewController(IConversations conversations, IConversationsViewMediator viewMediator, IConversationRequests conversationRequests, IUserSupportBackupRestorer backupRestorer, IUserSupportBackupDetailsProvider backupDetailsProvider, IAnalytics analytics)
		{
			this.conversations = conversations;
			this.viewMediator = viewMediator;
			this.conversationRequests = conversationRequests;
			this.backupRestorer = backupRestorer;
			this.backupDetailsProvider = backupDetailsProvider;
			this.analytics = analytics;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ViewClosed;



		public bool IsShowing { get; private set; }

		public void ShowView()
		{
			this.backupView = this.viewMediator.ShowBackupDialogueView();
			this.ShowBackupDetails();
			this.SetEventHandlers();
		}

		private void ShowBackupDetails()
		{
			Backup backup = this.conversations.GetPendingBackup().Backup;
			BackupSummary newBackupSummary = this.backupDetailsProvider.GetNewBackupSummary(backup.Data);
			string prettyFormattedDate = backup.GetPrettyFormattedDate();
			this.backupView.ShowBackupDetails(this.backupDetailsProvider.GetCurrentUserSettingsDetails(), newBackupSummary, prettyFormattedDate);
		}

		public void SetEventHandlers()
		{
			this.backupView.Apply += this.BackupViewOnApply;
			this.backupView.Dismiss += this.BackupViewOnDismiss;
		}

		private void BackupViewOnDismiss()
		{
			this.confirmationView = this.viewMediator.ShowBackupDismissConfirmation();
			this.confirmationView.Confirmed += this.ConfirmationViewOnConfirmed;
			this.confirmationView.Dismissed += this.ConfirmationViewOnDismissed;
		}

		private void ConfirmationViewOnDismissed()
		{
			this.confirmationView.Close();
		}

		private void ConfirmationViewOnConfirmed()
		{
			this.confirmationView.Close();
			this.backupView.Close();
			FiberCtrl.Pool.Run(this.conversationRequests.BackupDismissed(this.conversations.GetPendingBackup()), false);
		}

		private void BackupViewOnApply()
		{
			Message pendingBackup = this.conversations.GetPendingBackup();
			if (!this.IsBackupApplicable(pendingBackup))
			{
				return;
			}
			bool flag = this.backupRestorer.TryRestore(pendingBackup.Backup.Data);
			if (flag)
			{
				this.ShowRestoreSuccess(pendingBackup);
			}
			else
			{
				this.ShowRestoreFailed(pendingBackup);
				this.analytics.LogEvent(new ClientErrorEvent("UserSupport.ApplyBackupFailed", new StackTrace(false).ToString(), null, null, null, null, null, null, null), -1.0, null);
			}
		}

		private void ShowRestoreSuccess(Message message)
		{
			IBackupRestoreResultView backupRestoreResultView = this.viewMediator.ShowShutDownView();
			backupRestoreResultView.Confirmed += this.ViewOnConfirmed;
			backupRestoreResultView.Show(L.Get("Backup successfully restored!"), L.Get("The application will now close. Please relaunch the application"));
			FiberCtrl.Pool.Run(this.conversationRequests.BackupApplied(message), false);
		}

		private void ViewOnConfirmed()
		{
			Application.Quit();
		}

		private void ShowRestoreFailed(Message message)
		{
			IBackupRestoreResultView backupRestoreResultView = this.viewMediator.ShowBackupFailed();
			backupRestoreResultView.Show(L.Get("Restoring backup failed!"), string.Empty);
			FiberCtrl.Pool.Run(this.conversationRequests.BackupDismissed(message), false);
		}

		private bool IsBackupApplicable(Message messageWithBackup)
		{
			if (messageWithBackup == null)
			{
				return false;
			}
			string data = messageWithBackup.Backup.Data;
			return !string.IsNullOrEmpty(data);
		}

		private readonly IConversationRequests conversationRequests;

		private readonly IConversations conversations;

		private readonly IConversationsViewMediator viewMediator;

		private readonly IUserSupportBackupRestorer backupRestorer;

		private readonly IUserSupportBackupDetailsProvider backupDetailsProvider;

		private readonly IAnalytics analytics;

		private IBackupDialogueView backupView;

		private IBackupDismissConfirmationView confirmationView;
	}
}
