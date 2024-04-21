using System;
using Shared.UserSupport.Module.ViewControllers;
using TactileModules.Analytics.Interfaces;
using TactileModules.UserSupport.Cloud;
using TactileModules.UserSupport.DataProviders;
using TactileModules.UserSupport.View;
using TactileModules.UserSupport.ViewControllers;

namespace TactileModules.UserSupport
{
	public class UserSupportViewControllerFactory : IUserSupportViewControllerFactory
	{
		public UserSupportViewControllerFactory(IUser user, IConversations conversations, IPushNotificationHandler pushNotificationHandler, IConversationRequests conversationRequests, IConversationsViewMediator viewMediator, IUserSupportBackupRestorer backupRestorer, IUserSupportBackupDetailsProvider userSettingsDetailsProvider, IAnalytics analytics)
		{
			this.user = user;
			this.conversations = conversations;
			this.pushNotificationHandler = pushNotificationHandler;
			this.conversationRequests = conversationRequests;
			this.viewMediator = viewMediator;
			this.backupRestorer = backupRestorer;
			this.userSettingsDetailsProvider = userSettingsDetailsProvider;
			this.analytics = analytics;
		}

		public IUIViewController CreateConversationViewController()
		{
			return new ConversationViewController(this.user, this.conversations, this.pushNotificationHandler, this.viewMediator, this.conversationRequests, this);
		}

		public IUIViewController CreateUserInformationInputViewController()
		{
			return new UserInformationInputViewController(this.user, this.viewMediator);
		}

		public IUIViewController CreateNewMessagesPopupViewController()
		{
			return new AvailableMessagesViewController(this, this.viewMediator);
		}

		public IUIViewController CreateBackupDialogueViewController()
		{
			return new BackupDialogueViewController(this.conversations, this.viewMediator, this.conversationRequests, this.backupRestorer, this.userSettingsDetailsProvider, this.analytics);
		}

		private readonly IConversations conversations;

		private readonly IPushNotificationHandler pushNotificationHandler;

		private readonly IUser user;

		private readonly IConversationRequests conversationRequests;

		private readonly IConversationsViewMediator viewMediator;

		private readonly IUserSupportBackupRestorer backupRestorer;

		private readonly IUserSupportBackupDetailsProvider userSettingsDetailsProvider;

		private readonly IAnalytics analytics;
	}
}
