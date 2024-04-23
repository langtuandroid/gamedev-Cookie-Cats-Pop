using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	[TactileAnalytics.EventAttribute("levelSessionCreated", true)]
	public class LevelSessionCreatedEvent : BasicMissionEventBase
	{
		public LevelSessionCreatedEvent(ILevelSessionRunner levelSession) : base(levelSession)
		{
		}
	}
}
