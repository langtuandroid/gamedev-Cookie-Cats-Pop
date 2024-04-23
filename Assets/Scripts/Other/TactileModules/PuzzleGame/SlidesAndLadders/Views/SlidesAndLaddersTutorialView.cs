using System;
using JetBrains.Annotations;
using Tactile;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
	public class SlidesAndLaddersTutorialView : UIView
	{
		protected new void ViewLoad(object[] parameters)
		{
			this.ShowFirstTutorialStep();
		}

		public void ShowFirstTutorialStep()
		{
			this.tutorialStepFirst.SetActive(true);
			this.tutorialStepSecond.SetActive(false);
			this.button.SetActive(true);
		}

		public void ShowSecondTutorialStep()
		{
			this.tutorialStepFirst.SetActive(false);
			this.tutorialStepSecond.SetActive(true);
			this.button.SetActive(false);
		}

		public void ShowMask(MaskOverlay maskOverlay, UIView buttonView)
		{
			this.maskOverlay = maskOverlay;
			maskOverlay.Initialize();
			maskOverlay.Alpha = 0.8f;
			maskOverlay.SetDepthAboveView(buttonView);
			maskOverlay.Enable(null);
		}

		public void HighlightWheelInMask(GameObject wheelWidget)
		{
			Camera camera = MaskOverlayExtensions.FindCameraFromGO(wheelWidget);
			MaskOverlayCutout maskOverlayCutout = this.maskOverlay.AddCutout(null);
			maskOverlayCutout.SetFromWorldSpace(camera, wheelWidget.transform.position, Vector2.one * 450f);
			maskOverlayCutout.Oval = true;
		}

		public void HideMask()
		{
			this.maskOverlay.Disable(null);
		}

		[UsedImplicitly]
		private void ButtonPressed(UIEvent e)
		{
			if (this.OnButtonPressed != null)
			{
				this.OnButtonPressed();
			}
		}

		[SerializeField]
		private GameObject tutorialStepFirst;

		[SerializeField]
		private GameObject tutorialStepSecond;

		[SerializeField]
		private GameObject button;

		public Action OnButtonPressed;

		private MaskOverlay maskOverlay;
	}
}
