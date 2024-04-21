using System;
using System.Collections.Generic;

namespace Tactile.GardenGame.Story.Analytics
{
	public class ActionEventFactoryCollection
	{
		public ActionEventFactoryCollection()
		{
			this.CollectAllFactories();
		}

		private void CollectAllFactories()
		{
			Type typeFromHandle = typeof(IActionEventFactory);
			Type[] types = typeFromHandle.Assembly.GetTypes();
			foreach (Type type in types)
			{
				if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract)
				{
					this.factories.Add((IActionEventFactory)Activator.CreateInstance(type));
				}
			}
		}

		public object TryCreateEventFromAction(MapTask task, MapAction action, bool isStartEvent, object result)
		{
			for (int i = 0; i < this.factories.Count; i++)
			{
				object obj = this.factories[i].CreateEvent(task, action, isStartEvent, result);
				if (obj != null)
				{
					return obj;
				}
			}
			return null;
		}

		private readonly List<IActionEventFactory> factories = new List<IActionEventFactory>();
	}
}
