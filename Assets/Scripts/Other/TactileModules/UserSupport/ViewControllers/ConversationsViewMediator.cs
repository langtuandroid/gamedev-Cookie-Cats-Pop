using System;
using TactileModules.UserSupport.View;

namespace TactileModules.UserSupport.ViewControllers
{
	public class ConversationsViewMediator : IConversationsViewMediator
	{
		public ConversationsViewMediator(IUIViewManager viewManager)
		{
			this.viewManager = viewManager;
		}

		public IConversationView ShowConversationView()
		{
			UIViewManager.UIViewStateGeneric<ConversationView> uiviewStateGeneric = this.viewManager.ShowView<ConversationView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IUserInformationInputView ShowUserInformationInputView()
		{
			UIViewManager.UIViewStateGeneric<UserInformationInputView> uiviewStateGeneric = this.viewManager.ShowView<UserInformationInputView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IAlertView ShowAlertView()
		{
			UIViewManager.UIViewStateGeneric<AlertView> uiviewStateGeneric = this.viewManager.ShowView<AlertView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IAvailableMessagesPopupView ShowAvailableMessagesView()
		{
			UIViewManager.UIViewStateGeneric<AvailableMessagesPopupView> uiviewStateGeneric = this.viewManager.ShowView<AvailableMessagesPopupView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IClaimView ShowClaimView()
		{
			UIViewManager.UIViewStateGeneric<ClaimView> uiviewStateGeneric = this.viewManager.ShowView<ClaimView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IBackupDialogueView ShowBackupDialogueView()
		{
			UIViewManager.UIViewStateGeneric<BackupDialogueView> uiviewStateGeneric = this.viewManager.ShowView<BackupDialogueView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IBackupRestoreResultView ShowShutDownView()
		{
			UIViewManager.UIViewStateGeneric<BackupRestoreResultView> uiviewStateGeneric = this.viewManager.ShowView<BackupRestoreResultView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IBackupRestoreResultView ShowBackupFailed()
		{
			UIViewManager.UIViewStateGeneric<BackupRestoreResultView> uiviewStateGeneric = this.viewManager.ShowView<BackupRestoreResultView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		public IBackupDismissConfirmationView ShowBackupDismissConfirmation()
		{
			UIViewManager.UIViewStateGeneric<BackupDismissConfirmationView> uiviewStateGeneric = this.viewManager.ShowView<BackupDismissConfirmationView>(new object[0]);
			return uiviewStateGeneric.View;
		}

		private readonly IUIViewManager viewManager;
	}
}
