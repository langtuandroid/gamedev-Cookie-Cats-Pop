using System;

public class GetMoreLifeView : UIView
{
	private void ClosePressed(UIEvent e)
	{
		base.Close(0);
	}

	protected override void ViewWillAppear()
	{
		this.livesOverlay = base.ObtainOverlay<LivesOverlay>();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<LivesOverlay>();
	}

	protected override void ViewGotFocus()
	{
		this.livesOverlay.OnLifeButtonClicked = delegate()
		{
			base.Close(0);
		};
	}

	protected override void ViewLostFocus()
	{
		this.livesOverlay.OnLifeButtonClicked = null;
	}

	private LivesOverlay livesOverlay;
}
