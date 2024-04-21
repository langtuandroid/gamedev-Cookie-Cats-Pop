using System;
using JetBrains.Annotations;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
    public class EndlessChallengeMapButton : SideMapButton
    {
        private EndlessChallengeHandler Handler
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
            }
        }

        protected override void UpdateOncePerSecond()
        {
            if (!this.VisibilityChecker(null))
            {
                return;
            }
            this.timerLabel.text = this.Handler.GetTimeRemainingForEndlessChallengeAsString();
        }

        [UsedImplicitly]
        private new void Clicked(UIEvent e)
        {
            this.Handler.MapButtonClicked();
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
            return this.Handler.HasActiveFeature() && this.Handler.Config.IsActive;
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
