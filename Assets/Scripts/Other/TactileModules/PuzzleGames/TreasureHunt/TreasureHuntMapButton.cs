using System;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntMapButton : SideMapButton
    {
        private TreasureHuntManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<TreasureHuntManager>();
            }
        }

        protected override void UpdateOncePerSecond()
        {
            if (!this.VisibilityChecker(null))
            {
                return;
            }
            this.timerLabel.text = this.Manager.TimeLeftAsText;
        }

        private new void Clicked(UIEvent e)
        {
            if (this.Manager.SecondsLeft > 0)
            {
                this.Manager.SwitchToTreasureHuntMapView();
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
            return this.Manager.HasActiveFeature() && this.Manager.Config.FeatureEnabled && !this.Manager.GetFeatureInstanceCustomData<TreasureHuntInstanceCustomData, TreasureHuntMetaData, FeatureTypeCustomData>().rewardClaimed && this.Manager.SecondsLeft > 0;
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

        [SerializeField]
        private UILabel timerLabel;
    }
}
