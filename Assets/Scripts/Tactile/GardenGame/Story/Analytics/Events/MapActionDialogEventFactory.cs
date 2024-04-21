using System;
using Tactile.GardenGame.Story.Dialog;

namespace Tactile.GardenGame.Story.Analytics.Events
{
	public class MapActionDialogEventFactory : ActionEventFactory<MapActionDialog, DialogOverlayResult>
	{
		protected override object CreateStartEvent(MapTask task, MapActionDialog action)
		{
			return null;
		}

		protected override object CreateEndEvent(MapTask task, MapActionDialog action, DialogOverlayResult result)
		{
			return new DialogClosedEvent(task.ID, action.Text, result.WasSkipped, (double)result.SecondsWaited, (double)result.TotalDuration, task.Title);
		}
	}
}
