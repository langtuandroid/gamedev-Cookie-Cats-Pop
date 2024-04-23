using System;
using System.Collections;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Popups
{
	public class PiggyBankFreeOpenPopup : MapPopupManager.IMapPopup
	{
		public PiggyBankFreeOpenPopup(PiggyBankControllerFactory controllerFactory, PiggyBankMapPlugin mapPlugin, IPiggyBankProgression progression)
		{
			this.controllerFactory = controllerFactory;
			this.mapPlugin = mapPlugin;
			this.progression = progression;
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
			PiggyBankStateController piggyBankStateController = this.controllerFactory.CreateStateController();
			return piggyBankStateController.GetCurrentState() == PiggyBankStateController.PiggyBankState.FreeOpening;
		}

		private IEnumerator ShowPopup()
		{
			PiggyBankStateController stateController = this.controllerFactory.CreateStateController();
			yield return stateController.RunPiggyBankViewFlow();
			yield return this.PanCameraAndDropLevelDotIndicator();
			yield return this.controllerFactory.CreateBankOfferController();
			yield break;
		}

		private IEnumerator PanCameraAndDropLevelDotIndicator()
		{
			int level = this.progression.GetNextFreeOpenLevelHumanNumber();
			yield return this.mapPlugin.PanCameraAndDropLevelDotIndicator(level);
			yield break;
		}

		private readonly PiggyBankControllerFactory controllerFactory;

		private readonly PiggyBankMapPlugin mapPlugin;

		private readonly IPiggyBankProgression progression;
	}
}
