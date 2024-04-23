using System;
using TactileModules.FeatureManager;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Popups
{
    public class SlidesAndLaddersEndPopup : MapFeatureEndPopup
    {
        public SlidesAndLaddersEndPopup(TactileModules.FeatureManager.FeatureManager featureManager, SlidesAndLaddersHandler featureHandler) : base(featureManager, featureHandler)
        {
            this.featureHandler = featureHandler;
        }

        protected override bool ShouldShowPopup()
        {
            return true;
        }

        private readonly SlidesAndLaddersHandler featureHandler;
    }
}
