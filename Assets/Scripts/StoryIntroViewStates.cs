using System;

public class StoryIntroViewStates : StoryStateMachine
{
	protected override StoryState[] GetStates()
	{
		return new StoryState[]
		{
			this.alarm,
			this.catIntroduction,
			this.flag,
			this.shipApproaching,
			this.mamaOctopus,
			this.kittenInBubble,
			this.toTheRescue
		};
	}

	public StoryIntroView.AlarmState alarm;

	public StoryIntroView.Flag flag;

	public StoryIntroView.CatIntroductionState catIntroduction;

	public StoryIntroView.ShipApproaching shipApproaching;

	public StoryIntroView.MamaOctopus mamaOctopus;

	public StoryIntroView.KittenInBubble kittenInBubble;

	public StoryIntroView.ToTheRescue toTheRescue;
}
