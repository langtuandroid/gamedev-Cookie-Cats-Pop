using System;

namespace TactileModules.UserSupport
{
	public interface IPushNotificationHandler
	{
		event Action<PushNotificationPayload> NotificationReceived;
	}
}
