using System;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGame.HotStreak.UI
{
    public class HotStreakMapButton : SideMapButton
    {
        protected HotStreakManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<HotStreakManager>();
            }
        }

        public override SideMapButton.AreaSide Side
        {
            get
            {
                return SideMapButton.AreaSide.Left;
            }
        }

        public override bool VisibilityChecker(object data)
        {
            return this.Manager.IsHotStreakActive;
        }

        public override Vector2 Size
        {
            get
            {
                return this.GetElementSize();
            }
        }

        public override object Data
        {
            get
            {
                return null;
            }
        }

        protected override void UpdateOncePerSecond()
        {
            if (!this.VisibilityChecker(null))
            {
                return;
            }
            this.timerLabel.text = this.Manager.GetTimeRemainingForHotStreakAsString();
        }

        protected override void Setup()
        {
            int num = Mathf.Max(0, this.Manager.CurrentTierIndex);
            for (int i = 0; i < this.tierPivots.Length; i++)
            {
                this.tierPivots[i].SetActive(num == i);
            }
        }

        protected void HotStreakButtonClicked(UIEvent e)
        {
            if (!this.Manager.IsHotStreakActive)
            {
                return;
            }
            FiberCtrl.Pool.Run(this.Manager.ShowProgressViewAndWait(), false);
        }

        [SerializeField]
        private UILabel timerLabel;

        [SerializeField]
        private GameObject[] tierPivots;

        private Vector2 size;
    }
}
