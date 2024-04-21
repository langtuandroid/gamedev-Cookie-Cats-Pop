using System;
using System.Collections;
using System.Collections.Generic;

public class NinjaUIDebugData
{
	public static Hashtable DebugData { get; private set; }

	public static void Init(int queueSize)
	{
		NinjaUIDebugData.DebugData = new Hashtable();
		NinjaUIDebugData.maxQueueSize = queueSize;
		NinjaUIDebugData.actionQueue = new Queue<NinjaUIDebugData.Action>(queueSize);
	}

	public static void LogAction(string actionName)
	{
		if (NinjaUIDebugData.DebugData == null)
		{
			return;
		}
		NinjaUIDebugData.actionQueue.Enqueue(new NinjaUIDebugData.Action(actionName));
		if (NinjaUIDebugData.actionQueue.Count > NinjaUIDebugData.maxQueueSize)
		{
			NinjaUIDebugData.actionQueue.Dequeue();
		}
		ArrayList arrayList = new ArrayList();
		NinjaUIDebugData.DebugData["actions"] = arrayList;
		foreach (NinjaUIDebugData.Action action in NinjaUIDebugData.actionQueue.ToArray())
		{
			Hashtable hashtable = new Hashtable();
			hashtable["action"] = action.action;
			ArrayList arrayList2 = new ArrayList();
			hashtable["stack"] = arrayList2;
			foreach (string value in action.viewStack)
			{
				arrayList2.Add(value);
			}
			arrayList.Add(hashtable);
		}
	}

	private static Queue<NinjaUIDebugData.Action> actionQueue;

	private static int maxQueueSize;

	public class Action
	{
		public Action(string action)
		{
			this.action = action;
			this.viewStack = UIViewManager.Instance.GetViewNameStack();
		}

		public string action { get; private set; }

		public string[] viewStack { get; private set; }
	}
}
