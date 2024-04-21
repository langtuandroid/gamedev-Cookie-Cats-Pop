using System;
using System.Collections;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IFullScreenTransition
	{
		IEnumerator TransitionOut();

		IEnumerator TransitionIn();

		void FullyOut();
	}
}
