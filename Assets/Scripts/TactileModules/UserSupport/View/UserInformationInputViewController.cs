using System;
using System.Diagnostics;
using TactileModules.UserSupport.ViewControllers;

namespace TactileModules.UserSupport.View
{
	public class UserInformationInputViewController : IUIViewController
	{
		public UserInformationInputViewController(IUser user, IConversationsViewMediator viewMediator)
		{
			this.user = user;
			this.viewMediator = viewMediator;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ViewClosed;



		public void ShowView()
		{
			this.view = this.viewMediator.ShowUserInformationInputView();
			this.RegisterViewEvents();
		}

		public bool IsShowing { get; private set; }

		private void RegisterViewEvents()
		{
			this.view.Closed += this.ViewOnClosed;
			this.view.Submit += this.ViewOnSubmit;
		}

		private void ViewOnSubmit(string name)
		{
			this.SetUser(name);
			this.ViewOnClosed();
		}

		private void SetUser(string name)
		{
			this.user.Name = name;
		}

		private void ViewOnClosed()
		{
			this.view.Close(0);
			this.ViewClosed();
		}

		private readonly IUser user;

		private readonly IConversationsViewMediator viewMediator;

		private IUserInformationInputView view;
	}
}
