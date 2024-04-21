using System;

public class TutorialMessageView : UIView
{
	private void ButtonDimiss(UIEvent e)
	{
		base.Close(0);
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.session = (parameters[0] as LevelSession);
	}

	protected override void ViewWillAppear()
	{
		TutorialStep tutorialStep = this.session.Tutorial.CurrentStep as TutorialStep;
		TutorialSpeechBubble instance = this.frame.GetInstance<TutorialSpeechBubble>();
		instance.Message = tutorialStep.Message;
	}

	public UIInstantiator frame;

	private LevelSession session;
}
