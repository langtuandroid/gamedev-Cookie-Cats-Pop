using System;

public class OSNotification
{
	public bool isAppInFocus;

	public bool shown;

	public bool silentNotification;

	public int androidNotificationId;

	public OSNotification.DisplayType displayType;

	public OSNotificationPayload payload;

	public enum DisplayType
	{
		Notification,
		InAppAlert,
		None
	}
}
