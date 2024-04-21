using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.UserSupport.Cloud;
using TactileModules.UserSupport.Cloud.Requests;
using TactileModules.UserSupport.DataProviders;
using TactileModules.UserSupport.ViewControllers;

namespace TactileModules.UserSupport
{
	public static class UserSupportSystemBuilder
	{
		public static UserSupportSystem Build(IAttachmentsListener attachmentsListener, FacebookClient facebookClient, OneSignalManager oneSignalManager, IClientMessageMetaDataProvider messageMessageMetaDataProvider, CloudClientBase cloudClient, InAppPurchaseManagerBase iapManager, MapPopupManager mapPopupManager, IUIViewManager viewManager, ISupportedAttachments clientSupportedAttachments, IUserSupportBackupRestorer backupRestorer, IUserSupportBackupDetailsProvider userSettingsDetailsProvider, IAnalytics analytics)
		{
			MessageMetaData messageMetaDataProvider = new MessageMetaData(iapManager, facebookClient, cloudClient, messageMessageMetaDataProvider);
			UserSupportCloud cloud = new UserSupportCloud(cloudClient, messageMetaDataProvider);
			UserStorageFactory storageFactory = new UserStorageFactory();
			User user = new User(storageFactory, facebookClient);
			DefaultSupportedAttachments supportedAttachments = new DefaultSupportedAttachments(clientSupportedAttachments);
			Conversations conversations = new Conversations(attachmentsListener, supportedAttachments);
			PushNoficationsHandler pushNotificationHandler = new PushNoficationsHandler(oneSignalManager);
			CloudRequestFactory requestFactory = new CloudRequestFactory(conversations, cloud);
			UserSupportSynchronizer cloudSynchronizable = new UserSupportSynchronizer(requestFactory, pushNotificationHandler);
			ConversationRequests conversationRequests = new ConversationRequests(requestFactory, conversations, user);
			ConversationsViewMediator viewMediator = new ConversationsViewMediator(viewManager);
			UserSupportViewControllerFactory controllerFactory = new UserSupportViewControllerFactory(user, conversations, pushNotificationHandler, conversationRequests, viewMediator, backupRestorer, userSettingsDetailsProvider, analytics);
			UserSupportSystem result = new UserSupportSystem(controllerFactory, conversations, requestFactory, user, cloudSynchronizable);
			mapPopupManager.RegisterPopupObject(new UserSupportNewMessagesPopup(controllerFactory, conversations));
			return result;
		}
	}
}
