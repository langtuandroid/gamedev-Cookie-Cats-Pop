using System;

public class StoryOutroViewStates : StoryStateMachine
{
	protected override StoryState[] GetStates()
	{
		return new StoryState[]
		{
			this.showMap
		};
	}

	public StoryOutroView.ShowMap showMap;
}
