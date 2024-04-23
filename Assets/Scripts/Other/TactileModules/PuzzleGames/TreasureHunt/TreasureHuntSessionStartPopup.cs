using System;
using System.Collections;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntSessionStartPopup : MapPopupManager.IMapPopup
    {
        public TreasureHuntSessionStartPopup(TreasureHuntManager manager)
        {
            this.manager = manager;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        private bool ShouldShowPopup()
        {
            return this.manager.Config.FeatureEnabled && TactileModules.FeatureManager.FeatureManager.Instance.HasActiveFeature(this.manager) && TactileModules.FeatureManager.FeatureManager.Instance.GetStabilizedTimeLeftToFeatureDurationEnd(this.manager) > 0 && !TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager);
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.ShouldShowPopup())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
        }

        private IEnumerator ShowPopup()
        {
            yield return this.manager.ShowEventStartView(true);
            yield break;
        }

        private readonly TreasureHuntManager manager;
    }
}
