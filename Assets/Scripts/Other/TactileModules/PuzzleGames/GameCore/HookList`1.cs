using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.PuzzleGames.GameCore
{
	public class HookList<T> : IHookList<T>, IEnumerable<Func<T, IEnumerator>>, IEnumerable
	{
		public void Register(Func<T, IEnumerator> method)
		{
			this.list.Add(method);
		}

		public void Unregister(Func<T, IEnumerator> method)
		{
			this.list.Remove(method);
		}

		public IEnumerator InvokeAll(T parameter)
		{
			foreach (Func<T, IEnumerator> func in this.list)
			{
				yield return func(parameter);
			}
			yield break;
		}

		public IEnumerator<Func<T, IEnumerator>> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		private readonly List<Func<T, IEnumerator>> list = new List<Func<T, IEnumerator>>();
	}
}
