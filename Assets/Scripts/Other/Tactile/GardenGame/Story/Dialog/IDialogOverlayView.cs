using System;
using System.Collections;

namespace Tactile.GardenGame.Story.Dialog
{
	public interface IDialogOverlayView
	{
		IEnumerator PlayDialog(DialogActorSlot[] poses, int speakerIndex, string text, string imagePath, string splitScreenImagePath, DialogOverlayResult result);

		IEnumerator CloseDialog();

		void Initialize();

		event Action SkipClicked;
	}
}
