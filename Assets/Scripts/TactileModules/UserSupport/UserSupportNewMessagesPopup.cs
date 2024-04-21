using System;
using System.Collections;
using TactileModules.UserSupport.ViewControllers;

namespace TactileModules.UserSupport
{
	public class UserSupportNewMessagesPopup : MapPopupManager.IMapPopup
	{
		public UserSupportNewMessagesPopup(IUserSupportViewControllerFactory controllerFactory, IConversations conversations)
		{
			this.controllerFactory = controllerFactory;
			this.conversations = conversations;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (!this.CanShowPopup())
			{
				return;
			}
			popupFlow.AddPopup(this.ShowPopup());
		}

		private IEnumerator ShowPopup()
		{
			this.viewController = this.controllerFactory.CreateNewMessagesPopupViewController();
			this.viewController.ShowView();
			while (this.viewController.IsShowing)
			{
				yield return null;
			}
			yield break;
		}

		public bool CanShowPopup()
		{
			return this.IsNewMessageAvailable();
		}

		private bool IsNewMessageAvailable()
		{
			int numberOfUnreadMessagesAvailableOnServer = this.conversations.GetNumberOfUnreadMessagesAvailableOnServer();
			return numberOfUnreadMessagesAvailableOnServer > 0;
		}

		private readonly IUserSupportViewControllerFactory controllerFactory;

		private readonly IConversations conversations;

		private IUIViewController viewController;
	}
}
