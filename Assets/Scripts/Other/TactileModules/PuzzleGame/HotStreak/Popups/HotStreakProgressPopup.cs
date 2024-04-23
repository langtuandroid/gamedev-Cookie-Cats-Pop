using System;
using System.Collections;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGame.HotStreak.Popups
{
    public class HotStreakProgressPopup : MapPopupManager.IMapPopup
    {
        public HotStreakProgressPopup(HotStreakManager hotStreakManager, TactileModules.FeatureManager.FeatureManager featureManager)
        {
            this.manager = hotStreakManager;
            this.featureManager = featureManager;
            MapPopupManager.Instance.RegisterPopupObject(this);
        }

        public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
        {
            if (this.manager.IsHotStreakActive && this.manager.ProgressChanged)
            {
                popupFlow.AddPopup(this.ShowPopup());
            }
        }

        private IEnumerator ShowPopup()
        {
            yield return this.manager.ShowProgressViewAndWait();
            yield break;
        }

        private HotStreakManager manager;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;
    }
}
