using System;
using System.Collections;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntEndPopup : MapPopupManager.IMapPopup
    {
        public TreasureHuntEndPopup(TreasureHuntManager manager)
        {
            this.manager = manager;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        private bool ShouldEnd()
        {
            return TactileModules.FeatureManager.FeatureManager.Instance.ShouldDeactivateFeature(this.manager);
        }

        private bool ShouldShowPopup()
        {
            return this.ShouldEnd() && !this.manager.CustomData.rewardClaimed;
        }

        private bool ShouldRunSilentAction()
        {
            return this.ShouldEnd() && this.manager.CustomData.rewardClaimed;
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.ShouldShowPopup())
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
            else if (this.ShouldRunSilentAction())
            {
                popupFlow.AddSilentAction(delegate
                {
                    TactileModules.FeatureManager.FeatureManager.Instance.DeactivateFeature(this.manager);
                });
            }
        }

        private IEnumerator ShowPopup()
        {
            TactileModules.FeatureManager.FeatureManager.Instance.DeactivateFeature(this.manager);
            yield return this.manager.ShowEventEndedView();
            yield break;
        }

        private readonly TreasureHuntManager manager;
    }
}
