using System;
using System.Collections;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushLocalStartedPopup : MapPopupManager.IMapPopup
	{
		public LevelRushLocalStartedPopup(ILevelRushActivation levelRushActivation, LevelRushControllerFactory controllerFactory)
		{
			this.levelRushActivation = levelRushActivation;
			this.controllerFactory = controllerFactory;
		}

		private IEnumerator ShowPopup()
		{
			yield return this.controllerFactory.CreateStartLevelRushRunner();
			yield break;
		}

		public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
		{
			if (this.levelRushActivation.HasActivationTriggerForLevel(unlockedLevelIndex))
			{
				popupFlow.AddPopup(this.ShowPopup());
			}
		}

		private readonly ILevelRushActivation levelRushActivation;

		private readonly LevelRushControllerFactory controllerFactory;
	}
}
