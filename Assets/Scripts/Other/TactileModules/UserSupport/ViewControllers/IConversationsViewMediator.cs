using System;
using TactileModules.UserSupport.View;

namespace TactileModules.UserSupport.ViewControllers
{
	public interface IConversationsViewMediator
	{
		IConversationView ShowConversationView();

		IUserInformationInputView ShowUserInformationInputView();

		IAlertView ShowAlertView();

		IAvailableMessagesPopupView ShowAvailableMessagesView();

		IClaimView ShowClaimView();

		IBackupDialogueView ShowBackupDialogueView();

		IBackupRestoreResultView ShowShutDownView();

		IBackupRestoreResultView ShowBackupFailed();

		IBackupDismissConfirmationView ShowBackupDismissConfirmation();
	}
}
