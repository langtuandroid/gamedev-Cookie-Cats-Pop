using System;

public static class LocalNotificationAnalytics
{
	public static void LogLocalNotificationScheduled(LocalNotificationManager.Notification notification, string time, string notificationId, string featureInstanceId = "")
	{
		TactileAnalytics.Instance.LogEvent(new LocalNotificationAnalytics.LocalNotificationScheduledEvent(notification.AlertBody, time, notificationId, featureInstanceId), -1.0, null);
	}

	[TactileAnalytics.EventAttribute("localNotificationScheduled", true)]
	private class LocalNotificationScheduledEvent : BasicEvent
	{
		public LocalNotificationScheduledEvent(string notification, string time, string notificationId, string featureInstanceId = "")
		{
			this.Notification = notification;
			this.Time = time;
			this.NotificationId = notificationId;
			this.FeatureInstanceId = featureInstanceId;
		}

		private TactileAnalytics.RequiredParam<string> Notification { get; set; }

		private TactileAnalytics.RequiredParam<string> Time { get; set; }

		private TactileAnalytics.RequiredParam<string> NotificationId { get; set; }

		private TactileAnalytics.OptionalParam<string> FeatureInstanceId { get; set; }
	}
}
