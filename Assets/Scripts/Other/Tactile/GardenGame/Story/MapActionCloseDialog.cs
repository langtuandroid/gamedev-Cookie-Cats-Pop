using System;
using System.Collections;

namespace Tactile.GardenGame.Story
{
	public class MapActionCloseDialog : MapAction
	{
		public override IEnumerator Logic(IStoryMapController map)
		{
			while (MapActionDialog.IsAMapActionDialogRunning)
			{
				yield return null;
			}
			yield return map.Dialog.CloseDialog();
			yield break;
		}
	}
}
