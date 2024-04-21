using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntStartPopup : MapPopupManager.IMapPopup
    {
        public TreasureHuntStartPopup(TreasureHuntManager treasureHuntManager, TactileModules.FeatureManager.FeatureManager featureManager)
        {
            this.treasureHuntManager = treasureHuntManager;
            this.featureManager = featureManager;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        private bool ShouldShowPopup()
        {
            return this.treasureHuntManager.Config.FeatureEnabled && TactileModules.FeatureManager.FeatureManager.Instance.CanActivateFeature(this.treasureHuntManager) && PuzzleGameData.PlayerState.FarthestUnlockedLevelHumanNumber >= this.treasureHuntManager.Config.LevelRequired;
        }

        private IEnumerator ShowPopup(FeatureData featureData)
        {
            TactileModules.FeatureManager.FeatureManager.Instance.ActivateFeature(this.treasureHuntManager, featureData);
            yield return this.treasureHuntManager.ShowEventStartView(false);
            yield break;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.ShouldShowPopup())
            {
                FeatureData feature = this.featureManager.GetFeature(this.treasureHuntManager);
                popupFlow.AddPopup(this.ShowPopup(feature));
            }
        }

        private readonly TreasureHuntManager treasureHuntManager;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;
    }
}
