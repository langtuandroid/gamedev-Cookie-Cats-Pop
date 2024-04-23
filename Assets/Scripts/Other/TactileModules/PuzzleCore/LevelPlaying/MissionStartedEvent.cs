using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	[TactileAnalytics.EventAttribute("levelStarted", true)]
	public class MissionStartedEvent : BasicMissionEvent
	{
		public MissionStartedEvent(ILevelAttempt levelAttempt) : base(levelAttempt)
		{
		}
	}
}
