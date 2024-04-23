using System;
using TactileModules.UserSupport.ViewControllers;

namespace TactileModules.UserSupport
{
	public interface IUserSupportViewControllerFactory
	{
		IUIViewController CreateConversationViewController();

		IUIViewController CreateUserInformationInputViewController();

		IUIViewController CreateNewMessagesPopupViewController();

		IUIViewController CreateBackupDialogueViewController();
	}
}
