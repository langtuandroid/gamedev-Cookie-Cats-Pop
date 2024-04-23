using System;
using System.Collections;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.LevelDash.Popups
{
    public class LevelDashEndPopup : MapPopupManager.IMapPopup
    {
        public LevelDashEndPopup(LevelDashManager manager, LevelDashViewController levelDashViewController, GameSessionManager gameSessionManager)
        {
            this.manager = manager;
            this.levelDashViewController = levelDashViewController;
            this.isFirstsPopupAfterNewSession = true;
            gameSessionManager.NewSessionStarted += delegate ()
            {
                this.isFirstsPopupAfterNewSession = true;
            };
        }

        private bool ShouldShow()
        {
            return this.manager.HasActiveFeature() && this.manager.CustomInstanceData.HasReceivedFinalStatus && TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager) && this.isFirstsPopupAfterNewSession;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.ShouldShow())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
        }

        private IEnumerator ShowPopup()
        {
            this.isFirstsPopupAfterNewSession = false;
            yield return this.levelDashViewController.PerformEndFlow();
            yield break;
        }

        private LevelDashManager manager;

        private LevelDashViewController levelDashViewController;

        private bool isFirstsPopupAfterNewSession;
    }
}
