using System;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace TactileModules.PuzzleGames.TreasureHunt
{
    public class TreasureHuntRewardInfoView : ExtensibleView<TreasureHuntRewardInfoView.IExtension>
    {
        protected override void ViewWillAppear()
        {
            this.treasureHuntManager = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<TreasureHuntManager>();
            if (base.Extension != null)
            {
                base.Extension.InitializeRewardVisuals(this.treasureHuntManager.Config.Rewards);
            }
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - this.lastUpdated >= 1f)
            {
                this.timeLeftLabel.text = this.treasureHuntManager.TimeLeftAsText;
                this.lastUpdated = Time.realtimeSinceStartup;
            }
        }

        private void Dismiss(UIEvent e)
        {
            base.Close(0);
        }

        private void Continue(UIEvent e)
        {
            FlowStack flowStack = ManagerRepository.Get<FlowStack>();
            if (flowStack.Top is TreasureHuntMapFlow)
            {
                base.Close(0);
            }
            else
            {
                this.treasureHuntManager.SwitchToTreasureHuntMapView();
            }
        }

        [SerializeField]
        private UILabel timeLeftLabel;

        private float lastUpdated;

        private TreasureHuntManager treasureHuntManager;

        public interface IExtension
        {
            void InitializeRewardVisuals(List<ItemAmount> rewards);
        }
    }
}
