using System;
using System.Collections;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;



namespace TactileModules.PuzzleGames.StarTournament
{
    public class StarTournamentStartedPopup : MapPopupManager.IMapPopup
    {
        public StarTournamentStartedPopup([NotNull] StarTournamentManager starTournamentManager)
        {
            if (starTournamentManager == null)
            {
                throw new ArgumentNullException("starTournamentManager");
            }
            this.manager = starTournamentManager;
        }

        private bool ShouldShowPopup()
        {
            if (!this.manager.HasActiveFeature())
            {
                return false;
            }
            if (!this.manager.Config.FeatureEnabled)
            {
                return false;
            }
            if (PuzzleGameData.PlayerState.FarthestUnlockedLevelHumanNumber < this.manager.Config.LevelRequired)
            {
                return false;
            }
            if (TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager))
            {
                return false;
            }
            if (this.manager.CustomInstanceData.StartedViewPresented)
            {
                return false;
            }
            StarTournamentInstanceCustomData featureInstanceCustomData = this.manager.GetFeatureInstanceCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>();
            return this.HasJoined(featureInstanceCustomData) && this.HasJoinedCurrentlyActivatedFeatureInstance(featureInstanceCustomData);
        }

        private bool ShouldPerformSilentAction()
        {
            if (!this.manager.HasActiveFeature())
            {
                return false;
            }
            StarTournamentInstanceCustomData featureInstanceCustomData = this.manager.GetFeatureInstanceCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>();
            return this.HasJoined(featureInstanceCustomData) && !this.HasJoinedCurrentlyActivatedFeatureInstance(featureInstanceCustomData);
        }

        private bool HasJoined(StarTournamentInstanceCustomData featureInstanceCustomData)
        {
            return !string.IsNullOrEmpty(featureInstanceCustomData.JoinedInstanceId);
        }

        private bool HasJoinedCurrentlyActivatedFeatureInstance(StarTournamentInstanceCustomData featureInstanceCustomData)
        {
            ActivatedFeatureInstanceData activatedFeature = this.manager.GetActivatedFeature();
            return activatedFeature.Id == featureInstanceCustomData.JoinedInstanceId;
        }

        private IEnumerator ShowPopup()
        {
            yield return this.manager.ShowStarTournamentStartedView(false);
            this.manager.CustomInstanceData.StartedViewPresented = true;
            PuzzleGameData.UserSettings.SaveLocal();
            yield break;
        }

        private void SilentAction()
        {
            StarTournamentInstanceCustomData featureInstanceCustomData = this.manager.GetFeatureInstanceCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>();
            featureInstanceCustomData.JoinedInstanceId = string.Empty;
            featureInstanceCustomData.StartedViewPresented = false;
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
            else if (this.ShouldPerformSilentAction())
            {
                popupFlow.AddSilentAction(new Action(this.SilentAction));
            }
        }

        private readonly StarTournamentManager manager;
    }
}
