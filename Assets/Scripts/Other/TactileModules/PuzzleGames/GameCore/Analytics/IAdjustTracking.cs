using System;

namespace TactileModules.PuzzleGames.GameCore.Analytics
{
	public interface IAdjustTracking
	{
		void TrackEvent(string adjustEventId);
	}
}
