using System;

public class OSNotificationAction
{
	public string actionID;

	public OSNotificationAction.ActionType type;

	public enum ActionType
	{
		Opened,
		ActionTaken
	}
}
