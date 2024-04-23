using System;

namespace Tactile.GardenGame.Story.Analytics
{
	internal interface IActionEventFactory
	{
		object CreateEvent(MapTask task, MapAction mapAction, bool isStartEvent, object result);
	}
}
