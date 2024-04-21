using System;
using System.Collections;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StarTournament
{
    public class StarTournamentOldEndedPopup : MapPopupManager.IMapPopup
    {
        public StarTournamentOldEndedPopup([NotNull] StarTournamentManager managerBase)
        {
            if (managerBase == null)
            {
                throw new ArgumentNullException("managerBase");
            }
            this.manager = managerBase;
        }

        private bool ShouldShowPopup()
        {
            StarTournamentTypeCustomData featureTypeCustomData = this.manager.GetFeatureTypeCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>();
            return this.manager.Config.FeatureEnabled && featureTypeCustomData.OldTournamentInfo != null && featureTypeCustomData.OldTournamentInfo.NeedToShowEndedViewOldTournament > 0;
        }

        private void SilentAction()
        {
            OldStartTournamentInfo oldTournamentInfo = this.manager.GetFeatureTypeCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>().OldTournamentInfo;
            if (oldTournamentInfo != null)
            {
                oldTournamentInfo.Reset();
            }
            PuzzleGameData.UserSettings.SaveLocal();
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            OldStartTournamentInfo oldTournamentInfo = this.manager.GetFeatureTypeCustomData<StarTournamentInstanceCustomData, FeatureMetaData, StarTournamentTypeCustomData>().OldTournamentInfo;
            if (!this.manager.Config.FeatureEnabled)
            {
                return;
            }
            if (this.ShouldShowPopup() && this.CanGiveReward(oldTournamentInfo))
            {
                popupFlow.AddPopup(this.ShowPopup(oldTournamentInfo));
            }
            if (this.ShouldShowPopup() && !this.CanGiveReward(oldTournamentInfo))
            {
                popupFlow.AddSilentAction(new Action(this.SilentAction));
            }
        }

        private bool CanGiveReward(OldStartTournamentInfo info)
        {
            return info != null && info.RewardForOldTournament != null && info.RewardForOldTournament.Items != null && info.RewardForOldTournament.Items != null && info.RewardForOldTournament.Items.Count > 0 && info.RewardForOldTournament.Items[0].Amount > 0;
        }

        private IEnumerator ShowPopup(OldStartTournamentInfo info)
        {
            yield return this.manager.ShowStarTournamentEndedViewForOld(info);
            info.Reset();
            PuzzleGameData.UserSettings.SaveLocal();
            yield break;
        }

        private readonly StarTournamentManager manager;
    }
}
