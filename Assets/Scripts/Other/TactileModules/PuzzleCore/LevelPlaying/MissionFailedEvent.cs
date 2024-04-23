using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	[TactileAnalytics.EventAttribute("levelFailed", true)]
	public class MissionFailedEvent : MissionEndEvent
	{
		public MissionFailedEvent(ILevelAttempt levelAttempt) : base(levelAttempt)
		{
		}
	}
}
