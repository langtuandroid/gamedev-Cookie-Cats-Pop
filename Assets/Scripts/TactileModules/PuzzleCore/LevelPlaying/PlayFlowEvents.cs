using System;
using System.Diagnostics;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class PlayFlowEvents : IPlayFlowEvents, IPlayFlowEventEmitter
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ICorePlayFlow> PlayFlowCreated;



		public void EmitPlayFlowCreated(ICorePlayFlow corePlayFlow)
		{
			this.PlayFlowCreated(corePlayFlow);
		}
	}
}
