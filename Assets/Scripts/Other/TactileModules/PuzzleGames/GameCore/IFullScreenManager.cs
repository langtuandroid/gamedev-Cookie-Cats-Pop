using System;
using System.Collections;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IFullScreenManager
	{
		IHookList<ChangeInfo> PreChange { get; }

		IHookList<ChangeInfo> PostChange { get; }

		IHookList<ChangeInfo> MidChange { get; }

		IFullScreenOwner Top { get; }

		IEnumerator Push(IFullScreenOwner c);

		IEnumerator Pop();

		void PushInstantly(IFullScreenOwner c);

		void PopInstantly();

		IEnumerator ChangeToSameScreen();
	}
}
