using System;
using System.Collections;
using TactileModules.Foundation.CloudSynchronization;
using TactileModules.UserSupport.Cloud.Requests;

namespace TactileModules.UserSupport.Cloud
{
	public class UserSupportSynchronizer : ICloudSynchronizable
	{
		public UserSupportSynchronizer(ICloudRequestFactory requestFactory, IPushNotificationHandler pushNotificationHandler)
		{
			this.requestFactory = requestFactory;
			this.pushNotificationHandler = pushNotificationHandler;
			this.pushNotificationHandler.NotificationReceived += this.PushNotificationHandlerOnNotificationReceived;
		}

		private void PushNotificationHandlerOnNotificationReceived(PushNotificationPayload pushNotificationPayload)
		{
			FiberCtrl.Pool.Run(this.Synchronize(), false);
		}

		public IEnumerator Synchronize()
		{
			CheckUnreadCloudRequest cloudRequest = this.requestFactory.Create<CheckUnreadCloudRequest>();
			yield return cloudRequest.Execute();
			yield break;
		}

		private ICloudRequestFactory requestFactory;

		private IPushNotificationHandler pushNotificationHandler;
	}
}
