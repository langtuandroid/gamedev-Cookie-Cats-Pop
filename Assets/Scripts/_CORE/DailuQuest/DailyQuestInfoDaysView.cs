using System;

public class DailyQuestInfoDaysView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		int num = (int)parameters[0];
		if (num <= 1)
		{
			this.message.text = string.Format(L.Get("You missed {0} daily quest!"), num);
		}
		else
		{
			this.message.text = string.Format(L.Get("You missed {0} daily quests!"), num);
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public UILabel message;
}
