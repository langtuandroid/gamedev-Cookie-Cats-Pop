using System;

public class TournamentCompletedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		base.ViewLoad(parameters);
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public UILabel title;

	public UILabel description;

	public UISprite ticket;
}
