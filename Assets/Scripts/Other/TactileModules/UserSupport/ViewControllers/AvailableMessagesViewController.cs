using System;
using System.Diagnostics;
using TactileModules.UserSupport.View;

namespace TactileModules.UserSupport.ViewControllers
{
	public class AvailableMessagesViewController : IUIViewController
	{
		public AvailableMessagesViewController(IUserSupportViewControllerFactory controllerFactory, IConversationsViewMediator viewMediator)
		{
			this.controllerFactory = controllerFactory;
			this.viewMediator = viewMediator;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ViewClosed;



		public bool IsShowing { get; private set; }

		public void ShowView()
		{
			this.view = this.viewMediator.ShowAvailableMessagesView();
			this.view.ConversationSelected += this.ShowConversation;
			this.view.ClosedSelected += this.ViewOnClosedSelected;
			this.IsShowing = true;
		}

		private void ViewOnClosedSelected()
		{
			this.Close();
		}

		private void ShowConversation()
		{
			IUIViewController iuiviewController = this.controllerFactory.CreateConversationViewController();
			iuiviewController.ViewClosed += this.ConversationClosed;
			iuiviewController.ShowView();
		}

		private void ConversationClosed()
		{
			this.Close();
		}

		private void Close()
		{
			this.IsShowing = false;
			this.view.Close(0);
			this.ViewClosed();
		}

		private readonly IUserSupportViewControllerFactory controllerFactory;

		private readonly IConversationsViewMediator viewMediator;

		private IAvailableMessagesPopupView view;
	}
}
