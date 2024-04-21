using System;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class BasicMissionEventBaseDecorator : IEventDecorator<BasicMissionEventBase>, IEventDecorator
	{
		public BasicMissionEventBaseDecorator(IMainProgressionForAnalytics progression)
		{
			this.progression = progression;
		}

		public void Decorate(BasicMissionEventBase eventObject)
		{
			eventObject.SetAvailableMainMapLevels(this.progression.MaxAvailableLevelHumanNumber);
		}

		private readonly IMainProgressionForAnalytics progression;
	}
}
