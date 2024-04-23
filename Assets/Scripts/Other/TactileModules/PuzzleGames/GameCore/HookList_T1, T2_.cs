using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.GameCore
{
	public class HookList<T1, T2> : IHookList<T1, T2>, IEnumerable<Func<T1, T2, IEnumerator>>, IEnumerable
	{
		public void Register(Func<T1, T2, IEnumerator> method)
		{
			this.list.Add(method);
		}

		public void Unregister(Func<T1, T2, IEnumerator> method)
		{
			this.list.Remove(method);
		}

		public IEnumerator InvokeAll(T1 parameter1, T2 parameter2)
		{
			foreach (Func<T1, T2, IEnumerator> func in this.list)
			{
				yield return func(parameter1, parameter2);
			}
			yield break;
		}

		public IEnumerator<Func<T1, T2, IEnumerator>> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		private readonly List<Func<T1, T2, IEnumerator>> list = new List<Func<T1, T2, IEnumerator>>();
	}
}
