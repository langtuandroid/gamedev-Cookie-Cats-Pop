using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelDash.Popups
{
    public class LevelDashStartPopup : MapPopupManager.IMapPopup
    {
        public LevelDashStartPopup(LevelDashManager manager, LevelDashViewController levelDashViewController)
        {
            this.manager = manager;
            this.levelDashViewController = levelDashViewController;
        }

        private bool ShouldShow()
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
            if (this.manager.HasPresentedStartView())
            {
                return false;
            }
            LevelDashInstanceCustomData featureInstanceCustomData = this.manager.GetFeatureInstanceCustomData<LevelDashInstanceCustomData, FeatureMetaData, LevelDashTypeCustomData>();
            return this.HasJoined(featureInstanceCustomData) && this.HasJoinedCurrentlyActivatedFeatureInstance(featureInstanceCustomData);
        }

        private bool ShouldPerformSilentAction()
        {
            if (!this.manager.HasActiveFeature())
            {
                return false;
            }
            LevelDashInstanceCustomData featureInstanceCustomData = this.manager.GetFeatureInstanceCustomData<LevelDashInstanceCustomData, FeatureMetaData, LevelDashTypeCustomData>();
            return this.HasJoined(featureInstanceCustomData) && !this.HasJoinedCurrentlyActivatedFeatureInstance(featureInstanceCustomData);
        }

        private bool HasJoined(LevelDashInstanceCustomData featureInstanceCustomData)
        {
            return !string.IsNullOrEmpty(featureInstanceCustomData.JoinedInstanceId);
        }

        private bool HasJoinedCurrentlyActivatedFeatureInstance(LevelDashInstanceCustomData featureInstanceCustomData)
        {
            ActivatedFeatureInstanceData activatedFeature = this.manager.GetActivatedFeature();
            return activatedFeature.Id == featureInstanceCustomData.JoinedInstanceId;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (!this.manager.Config.FeatureEnabled)
            {
                return;
            }
            if (this.ShouldShow())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
            else if (this.ShouldPerformSilentAction())
            {
                popupFlow.AddSilentAction(new Action(this.SilentAction));
            }
        }

        private IEnumerator ShowPopup()
        {
            this.manager.StartFeature();
            yield return this.levelDashViewController.ShowStartPopup(false);
            yield break;
        }

        private void SilentAction()
        {
            LevelDashInstanceCustomData featureInstanceCustomData = this.manager.GetFeatureInstanceCustomData<LevelDashInstanceCustomData, FeatureMetaData, LevelDashTypeCustomData>();
            featureInstanceCustomData.JoinedInstanceId = string.Empty;
            featureInstanceCustomData.StartLevel = -1;
        }

        private LevelDashManager manager;

        private LevelDashViewController levelDashViewController;
    }
}
