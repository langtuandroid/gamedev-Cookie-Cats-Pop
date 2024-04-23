using System;
using System.Collections;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGame.HotStreak.Popups
{
    public class HotStreakStartPopup : MapPopupManager.IMapPopup
    {
        public HotStreakStartPopup(HotStreakManager hotStreakManager, TactileModules.FeatureManager.FeatureManager featureManager)
        {
            this.manager = hotStreakManager;
            this.featureManager = featureManager;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.manager.IsHotStreakActive)
            {
                return;
            }
            if (this.manager.ShouldActiveLocalHotStreak())
            {
                popupFlow.AddPopup(this.ShowPopup(true));
                return;
            }
            if (this.manager.ShouldActivateHotStreak())
            {
                popupFlow.AddPopup(this.ShowPopup(false));
            }
        }

        private IEnumerator ShowPopup(bool localFeature)
        {
            this.manager.ActivateHotStreak(localFeature);
            yield return this.manager.ShowProgressViewAndWait();
            yield break;
        }

        private HotStreakManager manager;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;
    }
}
