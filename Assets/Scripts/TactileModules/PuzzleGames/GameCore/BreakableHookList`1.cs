using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;

namespace TactileModules.PuzzleGames.GameCore
{
	public class BreakableHookList<T> : IEnumerable<Func<T, EnumeratorResult<bool>, IEnumerator>>, IBreakableHookList<T>, IEnumerable
	{
		public void Register(Func<T, EnumeratorResult<bool>, IEnumerator> method)
		{
			this.list.Add(method);
		}

		public void UnRegister(Func<T, EnumeratorResult<bool>, IEnumerator> method)
		{
			this.list.Remove(method);
		}

		public IEnumerator InvokeAll(T t, EnumeratorResult<bool> breakOut)
		{
			foreach (Func<T, EnumeratorResult<bool>, IEnumerator> func in this.list)
			{
				yield return func(t, breakOut);
				if (breakOut)
				{
					break;
				}
			}
			yield break;
		}

		public IEnumerator<Func<T, EnumeratorResult<bool>, IEnumerator>> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		private readonly List<Func<T, EnumeratorResult<bool>, IEnumerator>> list = new List<Func<T, EnumeratorResult<bool>, IEnumerator>>();
	}
}
