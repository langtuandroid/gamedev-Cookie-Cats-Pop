using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IBreakableHookList<T>
	{
		void Register(Func<T, EnumeratorResult<bool>, IEnumerator> method);

		void UnRegister(Func<T, EnumeratorResult<bool>, IEnumerator> method);

		IEnumerator InvokeAll(T t, EnumeratorResult<bool> breakOut);
	}
}
