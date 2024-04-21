using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	[TactileAnalytics.EventAttribute("levelAbandoned", true)]
	public class MissionAbandonedEvent : MissionEndEvent
	{
		public MissionAbandonedEvent(ILevelAttempt levelAttempt) : base(levelAttempt)
		{
		}
	}
}
