using System;
using Tactile;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Controllers
{
	public class SlidesAndLaddersWheelTutorialController
	{
		public SlidesAndLaddersWheelTutorialController(IDataProvider<SlidesAndLaddersInstanceCustomData> data, SlidesAndLaddersWheelWidget wheelWidget, UIScrollablePanel scrollablePanel, SlidesAndLaddersMapButtonView slidesAndLaddersMapButtonView)
		{
			this.data = data;
			this.slidesAndLaddersMapButtonView = slidesAndLaddersMapButtonView;
			this.wheelWidget = wheelWidget;
			this.scrollablePanel = scrollablePanel;
		}

		public void StartTutorial()
		{
			if (!this.data.Get().HasShownTutorial)
			{
				this.ShowTutorial();
			}
		}

		private void ShowTutorial()
		{
			this.scrollablePanel.GetComponent<BoxCollider>().enabled = false;
			this.tutorialView = UIViewManager.Instance.ShowView<SlidesAndLaddersTutorialView>(new object[0]).View;
			this.tutorialView.ShowFirstTutorialStep();
			this.tutorialView.ShowMask(new MaskOverlay(), this.slidesAndLaddersMapButtonView);
			SlidesAndLaddersTutorialView slidesAndLaddersTutorialView = this.tutorialView;
			slidesAndLaddersTutorialView.OnButtonPressed = (Action)Delegate.Combine(slidesAndLaddersTutorialView.OnButtonPressed, new Action(this.TutorialButtonClicked));
		}

		private void TutorialButtonClicked()
		{
			this.tutorialView.ShowSecondTutorialStep();
			this.tutorialView.HighlightWheelInMask(this.wheelWidget.gameObject);
			this.data.Get().CanSpinWheel = true;
			this.wheelWidget.SetWheelState(this.data.Get().CanSpinWheel);
			SlidesAndLaddersWheelWidget slidesAndLaddersWheelWidget = this.wheelWidget;
			slidesAndLaddersWheelWidget.OnWheelClicked = (Action)Delegate.Combine(slidesAndLaddersWheelWidget.OnWheelClicked, new Action(this.TutorialWheelClicked));
			SlidesAndLaddersWheelWidget slidesAndLaddersWheelWidget2 = this.wheelWidget;
			slidesAndLaddersWheelWidget2.OnAnimatingIsDone = (Action)Delegate.Combine(slidesAndLaddersWheelWidget2.OnAnimatingIsDone, new Action(this.TutorialDone));
		}

		private void TutorialDone()
		{
			this.tutorialView.HideMask();
			this.data.Get().HasShownTutorial = true;
			this.scrollablePanel.GetComponent<BoxCollider>().enabled = true;
			SlidesAndLaddersWheelWidget slidesAndLaddersWheelWidget = this.wheelWidget;
			slidesAndLaddersWheelWidget.OnAnimatingIsDone = (Action)Delegate.Remove(slidesAndLaddersWheelWidget.OnAnimatingIsDone, new Action(this.TutorialDone));
		}

		private void TutorialWheelClicked()
		{
			this.tutorialView.Close(0);
			SlidesAndLaddersWheelWidget slidesAndLaddersWheelWidget = this.wheelWidget;
			slidesAndLaddersWheelWidget.OnWheelClicked = (Action)Delegate.Remove(slidesAndLaddersWheelWidget.OnWheelClicked, new Action(this.TutorialWheelClicked));
		}

		private readonly SlidesAndLaddersWheelWidget wheelWidget;

		private SlidesAndLaddersTutorialView tutorialView;

		private readonly IDataProvider<SlidesAndLaddersInstanceCustomData> data;

		private readonly SlidesAndLaddersMapButtonView slidesAndLaddersMapButtonView;

		private readonly UIScrollablePanel scrollablePanel;
	}
}
