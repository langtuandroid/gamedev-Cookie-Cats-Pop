using System;
using System.Collections;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Popups
{
	public class PiggyBankTutorialPopup : MapPopupManager.IMapPopup
	{
		public PiggyBankTutorialPopup(PiggyBankControllerFactory controllerFactory, PiggyBankMapPlugin piggyBankMapPlugin, IPiggyBankProgression progression)
		{
			this.controllerFactory = controllerFactory;
			this.piggyBankMapPlugin = piggyBankMapPlugin;
			this.progression = progression;
			this.stateController = controllerFactory.CreateStateController();
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (this.ShouldShow())
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		private bool ShouldShow()
		{
			return this.stateController.GetCurrentState() == PiggyBankStateController.PiggyBankState.Tutorial;
		}

		private IEnumerator ShowPopup()
		{
			yield return this.controllerFactory.CreateTutorialController();
			yield return this.PanCameraAndDropLevelDotIndicator();
			yield break;
		}

		private IEnumerator PanCameraAndDropLevelDotIndicator()
		{
			int level = this.progression.GetNextFreeOpenLevelHumanNumber();
			yield return this.piggyBankMapPlugin.PanCameraAndDropLevelDotIndicator(level);
			yield break;
		}

		private readonly PiggyBankControllerFactory controllerFactory;

		private readonly PiggyBankMapPlugin piggyBankMapPlugin;

		private readonly IPiggyBankProgression progression;

		private readonly PiggyBankStateController stateController;
	}
}
