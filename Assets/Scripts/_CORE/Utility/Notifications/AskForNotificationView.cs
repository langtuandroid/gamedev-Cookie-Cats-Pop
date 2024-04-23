using System;

public class AskForNotificationView : UIView
{
	private void OnAccept(UIEvent e)
	{
		base.Close(1);
	}

	private void OnDismiss(UIEvent e)
	{
		base.Close(0);
	}
}
