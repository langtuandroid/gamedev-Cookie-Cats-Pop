using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IHookList<T1, T2> : IEnumerable<Func<T1, T2, IEnumerator>>, IEnumerable
	{
		void Register(Func<T1, T2, IEnumerator> method);

		void Unregister(Func<T1, T2, IEnumerator> method);

		IEnumerator InvokeAll(T1 parameter1, T2 parameter2);
	}
}
