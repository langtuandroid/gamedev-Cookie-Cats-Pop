using System;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Views
{
    public class SlidesAndLaddersRewardInfoView : ExtensibleView<IReward>
    {
        private SlidesAndLaddersHandler SlidesAndLaddersHandler
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<SlidesAndLaddersHandler>();
            }
        }

        protected override void ViewLoad(object[] parameters)
        {
            base.Extension.Initialize(this.SlidesAndLaddersHandler.GetFeatureRewards());
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - this.lastUpdated >= 1f)
            {
                this.timeLeftLabel.text = this.SlidesAndLaddersHandler.GetTimeLeftAsText();
                this.lastUpdated = Time.realtimeSinceStartup;
            }
        }

        [UsedImplicitly]
        private void Dismiss(UIEvent e)
        {
            base.Close(0);
        }

        [UsedImplicitly]
        private void Continue(UIEvent e)
        {
            base.Close(1);
        }

        [SerializeField]
        private UILabel timeLeftLabel;

        private float lastUpdated;
    }
}
