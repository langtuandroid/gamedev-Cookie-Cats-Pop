using System;
using System.Collections;
using JetBrains.Annotations;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.StarTournament
{
    public class StarTournamentStartSessionPopup : MapPopupManager.IMapPopup
    {
        public StarTournamentStartSessionPopup([NotNull] StarTournamentManager starTournamentManager)
        {
            if (starTournamentManager == null)
            {
                throw new ArgumentNullException("starTournamentManager");
            }
            this.manager = starTournamentManager;
        }

        private bool ShouldShowPopup()
        {
            return this.manager.Config.FeatureEnabled && this.manager.HasActiveFeature() && !TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager) && this.manager.CustomInstanceData.StartedViewPresented;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (!this.manager.Config.FeatureEnabled)
            {
                return;
            }
            if (this.ShouldShowPopup())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
        }

        private IEnumerator ShowPopup()
        {
            yield return this.manager.ShowStarTournamentStartedView(true);
            yield break;
        }

        private readonly StarTournamentManager manager;
    }
}
