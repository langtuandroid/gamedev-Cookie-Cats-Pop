using System;

namespace Tactile.GardenGame.Story.Analytics
{
	public abstract class ActionEventFactory<TActionType, TResult> : IActionEventFactory where TActionType : MapAction
	{
		public object CreateEvent(MapTask task, MapAction action, bool isStartEvent, object result)
		{
			TActionType tactionType = action as TActionType;
			if (tactionType == null)
			{
				return null;
			}
			if (isStartEvent)
			{
				return this.CreateStartEvent(task, tactionType);
			}
			TResult tresult = (TResult)((object)result);
			if (tresult == null)
			{
				return null;
			}
			return this.CreateEndEvent(task, tactionType, tresult);
		}

		protected abstract object CreateStartEvent(MapTask task, TActionType action);

		protected abstract object CreateEndEvent(MapTask task, TActionType action, TResult result);
	}
}
