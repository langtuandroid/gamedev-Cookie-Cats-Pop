using System;
using System.Collections;
using JetBrains.Annotations;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.StarTournament
{
    public class StarTournamentEndedPopup : MapPopupManager.IMapPopup
    {
        public StarTournamentEndedPopup([NotNull] StarTournamentManager starTournamentManager)
        {
            if (starTournamentManager == null)
            {
                throw new ArgumentNullException("starTournamentManager");
            }
            this.manager = starTournamentManager;
            this.isFirstPopupAfterNewSession = true;
            this.manager.Provider.OnNewSessionStarted += delegate ()
            {
                this.isFirstPopupAfterNewSession = true;
            };
        }

        private bool ShouldShowPopups()
        {
            return this.isFirstPopupAfterNewSession && this.manager.Config.FeatureEnabled && this.manager.LocalPersistedState.HasReceivedFinalStatus && TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager) && this.manager.CustomInstanceData.StartedViewPresented;
        }

        private bool ShouldEndSilently()
        {
            return this.manager.Config.FeatureEnabled && TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager) && !this.manager.CustomInstanceData.StartedViewPresented;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.ShouldShowPopups())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
            else if (this.ShouldEndSilently())
            {
                popupFlow.AddSilentAction(new Action(this.EndSilently));
            }
        }

        private void EndSilently()
        {
            this.manager.EndStarTournament();
        }

        private IEnumerator ShowPopup()
        {
            yield return this.manager.PerformEndFlow();
            yield break;
        }

        private readonly StarTournamentManager manager;

        private bool isFirstPopupAfterNewSession;
    }
}
