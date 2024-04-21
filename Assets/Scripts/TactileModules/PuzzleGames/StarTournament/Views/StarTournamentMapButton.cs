using System;
using TactileModules.FeatureManager;
using UnityEngine;

namespace TactileModules.PuzzleGames.StarTournament.Views
{
    public class StarTournamentMapButton : SideMapButton
    {
        private StarTournamentManager Manager
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<StarTournamentManager>();
            }
        }

        protected override void UpdateOncePerSecond()
        {
            if (!this.VisibilityChecker(null))
            {
                return;
            }
            this.timerLabel.text = ((!this.Manager.HasSentPresentRequest) ? this.Manager.GetTimeRemainingForStarTournamentAsString() : L.Get("Loading..."));
        }

        private void StarTournamentButtonClicked(UIEvent e)
        {
            if (!this.Manager.Config.FeatureEnabled || this.Manager.HasSentPresentRequest)
            {
                return;
            }
            this.Manager.StarTournamentCloud.SyncStatus();
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
            return this.Manager.Config.FeatureEnabled && this.Manager.HasActiveFeature() && this.Manager.CustomInstanceData.StartedViewPresented;
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
