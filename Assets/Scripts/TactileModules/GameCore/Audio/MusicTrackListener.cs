using System;
using System.Diagnostics;

namespace TactileModules.GameCore.Audio
{
	public class MusicTrackListener
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnExit;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnExitImmediate;

		public void ExitMusicTrack()
		{
			if (this.OnExit != null)
			{
				this.OnExit();
			}
		}

		public void ExitMusicTrackImmediate()
		{
			if (this.OnExitImmediate != null)
			{
				this.OnExitImmediate();
			}
		}
	}
}
