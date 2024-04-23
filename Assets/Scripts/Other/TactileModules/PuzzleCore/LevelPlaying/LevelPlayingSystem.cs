using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class LevelPlayingSystem
	{
		public LevelPlayingSystem(IPlayFlowFactory playFlowFactory, IPlayFlowEvents playFlowEvents)
		{
			this.PlayFlowFactory = playFlowFactory;
			this.PlayFlowEvents = playFlowEvents;
		}

		public IPlayFlowFactory PlayFlowFactory { get; set; }

		public IPlayFlowEvents PlayFlowEvents { get; set; }
	}
}
