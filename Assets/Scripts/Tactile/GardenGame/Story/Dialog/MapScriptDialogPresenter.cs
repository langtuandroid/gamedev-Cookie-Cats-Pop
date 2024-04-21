using System;
using System.Collections;
using System.Diagnostics;
using TactileModules.GameCore.UI;

namespace Tactile.GardenGame.Story.Dialog
{
	public class MapScriptDialogPresenter : IMapScriptDialogPresenter
	{
		public MapScriptDialogPresenter(IUIController uiController, DialogOverlayView dialogOverlayViewPrefab)
		{
			this.uiController = uiController;
			this.dialogOverlayViewPrefab = dialogOverlayViewPrefab;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action SkipClicked;

		public IEnumerator PlayDialog(DialogActorSlot[] slots, int speakerIndex, string text, string imagePath, string splitScreenImage, DialogOverlayResult result, DialogOverlayView viewPrefabToUse = null)
		{
			if (viewPrefabToUse == null)
			{
				viewPrefabToUse = this.dialogOverlayViewPrefab;
			}
			if (this.currentDialogOverlayViewPrefab != viewPrefabToUse)
			{
				if (this.dialogView != null)
				{
					yield return this.CloseDialog();
				}
				this.currentDialogOverlayViewPrefab = viewPrefabToUse;
			}
			if (this.dialogView == null)
			{
				this.dialogView = this.uiController.ShowView<DialogOverlayView>(this.currentDialogOverlayViewPrefab);
				UIViewManager.Instance.GetViewLayerWithView(this.dialogView).UICamera.cachedCamera.depth = 30f;
				this.dialogView.SkipClicked += this.DialogViewOnSkipClicked;
			}
			if (!string.IsNullOrEmpty(text))
			{
				yield return this.dialogView.PlayDialog(slots, speakerIndex, text, imagePath, splitScreenImage, result);
			}
			yield break;
		}

		public IEnumerator CloseDialog()
		{
			if (this.dialogView == null)
			{
				yield break;
			}
			yield return this.dialogView.CloseDialog();
			this.dialogView.Close(0);
			while (!this.dialogView.IsClosing)
			{
				yield return null;
			}
			this.dialogView = null;
			this.currentDialogOverlayViewPrefab = null;
			yield break;
		}

		public void Destroy()
		{
			if (this.dialogView != null)
			{
				this.dialogView.Close(0);
				this.dialogView = null;
			}
		}

		private void DialogViewOnSkipClicked()
		{
			if (this.SkipClicked != null)
			{
				this.SkipClicked();
			}
		}

		private readonly IUIController uiController;

		private readonly DialogOverlayView dialogOverlayViewPrefab;

		private DialogOverlayView dialogView;

		private DialogOverlayView currentDialogOverlayViewPrefab;
	}
}
