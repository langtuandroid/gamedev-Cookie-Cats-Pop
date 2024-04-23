using System;
using System.Collections;

namespace Tactile.GardenGame.Story.Dialog
{
	public interface IMapScriptDialogPresenter
	{
		IEnumerator PlayDialog(DialogActorSlot[] slots, int speakerIndex, string text, string imagePath, string splitScreenImage, DialogOverlayResult result, DialogOverlayView viewPrefabToUse = null);

		IEnumerator CloseDialog();

		event Action SkipClicked;

		void Destroy();
	}
}
