using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.UserSupport.Cloud;
using TactileModules.UserSupport.Cloud.Requests;
using TactileModules.UserSupport.Model;
using TactileModules.UserSupport.View;

namespace TactileModules.UserSupport.ViewControllers
{
	public class ConversationViewController : IUIViewController
	{
		public ConversationViewController(IUser user, IConversations conversations, IPushNotificationHandler pushNotificationHandler, IConversationsViewMediator viewMediator, IConversationRequests requests, IUserSupportViewControllerFactory controllerFactory)
		{
			this.user = user;
			this.conversations = conversations;
			this.pushNotificationHandler = pushNotificationHandler;
			this.viewMediator = viewMediator;
			this.requests = requests;
			this.controllerFactory = controllerFactory;
			this.conversation = this.conversations.Conversation;
			this.RegisterEventHandlers();
			this.LoadMessages();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ViewClosed;



		public bool IsShowing { get; private set; }

		private void RegisterEventHandlers()
		{
			this.requests.RequestsFailed += this.RequestsOnRequestFailed;
			this.pushNotificationHandler.NotificationReceived += this.UserSupportOnNotificationReceived;
			this.conversations.Loaded += this.OnConversationUpdated;
		}

		private void UserSupportOnNotificationReceived(PushNotificationPayload obj)
		{
			if (this.view == null)
			{
				return;
			}
			this.LoadMessages();
			this.SetRead();
		}

		public void ShowView()
		{
			this.view = this.viewMediator.ShowConversationView();
			this.view.Init();
			this.view.SetUser(this.user);
			this.RegisterViewEvents();
			this.RenderView();
			this.SetRead();
		}

		private void CheckForPendingBackup()
		{
			bool flag = this.conversations.HasPendingBackup();
			if (flag)
			{
				IUIViewController iuiviewController = this.controllerFactory.CreateBackupDialogueViewController();
				iuiviewController.ShowView();
			}
		}

		private void RenderView()
		{
			this.view.RenderMessages(this.conversation.Messages);
		}

		private void RegisterViewEvents()
		{
			this.view.Closed += this.ViewOnClosed;
			this.view.SubmitMessage += this.ViewOnSubmitMessage;
			this.view.AttachmentClicked += this.ViewOnAttachmentClicked;
		}

		private void ViewOnAttachmentClicked(Message message)
		{
			this.CheckAttachments();
		}

		private void ViewOnClosed()
		{
			this.view.Close(0);
			this.UnregisterEventHandlers();
			this.ViewClosed();
		}

		private void ViewOnSubmitMessage(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
			string name = this.user.Name;
			if (string.IsNullOrEmpty(name))
			{
				this.user.StoredMessage = input;
				this.ShowUserInformationInputView();
				return;
			}
			this.SubmitMessage(input.Trim());
		}

		private void ShowUserInformationInputView()
		{
			this.view.HideKeyboard();
			IUIViewController iuiviewController = this.controllerFactory.CreateUserInformationInputViewController();
			iuiviewController.ShowView();
			iuiviewController.ViewClosed += this.UserInformationInputControllerOnViewClosed;
		}

		private void UserInformationInputControllerOnViewClosed()
		{
			this.SubmitStoredMessage();
		}

		private void SubmitStoredMessage()
		{
			if (string.IsNullOrEmpty(this.user.Name))
			{
				return;
			}
			this.SubmitMessage(this.user.StoredMessage);
		}

		private void OnConversationUpdated()
		{
			this.conversation = this.GetConversation();
			this.CheckAttachments();
			this.CheckForPendingBackup();
			this.RenderView();
		}

		private void CheckAttachments()
		{
			if (this.conversation.GetUnclaimedAttachments().Count == 0)
			{
				return;
			}
			this.claimDialogue = this.viewMediator.ShowClaimView();
			this.claimDialogue.Closed += this.ClaimDialogueOnClosed;
			this.claimDialogue.Claimed += this.ClaimDialogueOnClaimed;
		}

		private void ClaimDialogueOnClaimed()
		{
			this.CliamAllAttachments();
		}

		private void ClaimDialogueOnClosed()
		{
			this.claimDialogue.Closed -= this.ClaimDialogueOnClosed;
			this.claimDialogue.Claimed -= this.ClaimDialogueOnClaimed;
		}

		private IConversation GetConversation()
		{
			return this.conversations.Conversation;
		}

		private void LoadMessages()
		{
			FiberCtrl.Pool.Run(this.ShowProgressViewAndExecute(this.requests.LoadMessages()), false);
		}

		private void SubmitMessage(string message)
		{
			FiberCtrl.Pool.Run(this.ShowProgressViewAndExecute(this.requests.SubmitMessage(message)), false);
		}

		protected virtual void SetRead()
		{
			FiberCtrl.Pool.Run(this.ShowProgressViewAndExecute(this.requests.SetRead()), false);
		}

		private void CliamAllAttachments()
		{
			FiberCtrl.Pool.Run(this.ShowProgressViewAndExecute(this.requests.Claim()), false);
		}

		private IEnumerator ShowProgressViewAndExecute(IEnumerator request)
		{
			this.view.ShowLoadingIndicator();
			yield return new Fiber.OnExit(delegate()
			{
				if (this.view == null)
				{
					return;
				}
				this.view.HideLoadingIndicator();
			});
			yield return request;
			yield break;
		}

		private void RequestsOnRequestFailed(CloudRequestsSequence requests)
		{
			RequestLog requestLog = requests.InvalidRequests[0];
			FiberCtrl.Pool.Run(this.ShowErrorView(requestLog.Response.ErrorInfo), false);
		}

		private IEnumerator ShowErrorView(string message)
		{
			UIViewManager.UIViewStateGeneric<AlertView> vs = UIViewManager.Instance.ShowView<AlertView>(new object[0]);
			AlertView alertView = vs.View;
			alertView.Show(message);
			yield return vs.WaitForClose();
			yield break;
		}

		private void UnregisterEventHandlers()
		{
			this.pushNotificationHandler.NotificationReceived -= this.UserSupportOnNotificationReceived;
			this.conversations.Loaded -= this.OnConversationUpdated;
		}

		private readonly IConversations conversations;

		private readonly IPushNotificationHandler pushNotificationHandler;

		private readonly IUser user;

		private readonly IConversationsViewMediator viewMediator;

		private readonly IConversationRequests requests;

		private readonly IUserSupportViewControllerFactory controllerFactory;

		private IConversationView view;

		private IClaimView claimDialogue;

		private IConversation conversation;
	}
}
