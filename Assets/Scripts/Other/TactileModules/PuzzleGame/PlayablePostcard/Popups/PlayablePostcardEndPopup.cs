using System;
using TactileModules.FeatureManager;
using TactileModules.MapFeature;

namespace TactileModules.PuzzleGame.PlayablePostcard.Popups
{
    public class PlayablePostcardEndPopup : MapFeatureEndPopup
    {
        public PlayablePostcardEndPopup(TactileModules.FeatureManager.FeatureManager featureManager, MapFeatureHandler featureHandler) : base(featureManager, featureHandler)
        {
        }

        protected override bool ShouldShowPopup()
        {
            return true;
        }
    }
}
