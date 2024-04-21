using System;
using System.Collections;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IFullScreenOwner
	{
		IEnumerator ScreenAcquired();

		void ScreenLost();

		void ScreenReady();
	}
}
