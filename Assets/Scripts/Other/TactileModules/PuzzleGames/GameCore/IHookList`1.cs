using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IHookList<T> : IEnumerable<Func<T, IEnumerator>>, IEnumerable
	{
		void Register(Func<T, IEnumerator> method);

		void Unregister(Func<T, IEnumerator> method);

		IEnumerator InvokeAll(T parameter);
	}
}
