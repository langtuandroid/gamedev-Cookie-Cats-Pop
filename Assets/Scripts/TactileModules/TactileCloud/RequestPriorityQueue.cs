using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.TactileCloud
{
	public class RequestPriorityQueue
	{
		public RequestPriorityQueue()
		{
			this.possiblePriorities = Enum.GetValues(typeof(RequestPriority));
			this.priorityQueue = new Dictionary<RequestPriority, List<string>>();
			IEnumerator enumerator = this.possiblePriorities.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					this.priorityQueue.Add((RequestPriority)obj, new List<string>());
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void Enqueue(RequestPriority requestPriority, string requestId)
		{
			this.priorityQueue[requestPriority].Add(requestId);
		}

		public bool Remove(RequestPriority requestPriority, string requestId)
		{
			return this.priorityQueue[requestPriority].Remove(requestId);
		}

		public bool IsNextRequest(string requestId)
		{
			foreach (RequestPriority key in this.IteratePriorities())
			{
				List<string> list = this.priorityQueue[key];
				if (list.Count > 0)
				{
					return list[0] == requestId;
				}
			}
			return false;
		}

		private IEnumerable<RequestPriority> IteratePriorities()
		{
			for (int i = this.possiblePriorities.Length - 1; i >= 0; i--)
			{
				yield return (RequestPriority)this.possiblePriorities.GetValue(i);
			}
			yield break;
		}

		private readonly Dictionary<RequestPriority, List<string>> priorityQueue;

		private readonly Array possiblePriorities;
	}
}
