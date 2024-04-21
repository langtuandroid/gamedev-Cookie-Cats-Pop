using System;
using TactileModules.FeatureManager;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.PlayablePostcard.Model;

namespace TactileModules.PuzzleGame.PlayablePostcard.Popups
{
    public class PlayablePostcardSessionStartPopup : MapFeatureSessionStartPopup
    {
        public PlayablePostcardSessionStartPopup(TactileModules.FeatureManager.FeatureManager featureManager, MapFeatureHandler featureHandler, PlayablePostcardActivation postcardActivation) : base(featureManager, featureHandler)
        {
            this.postcardActivation = postcardActivation;
        }

        protected override bool ShouldShowPopup()
        {
            return this.postcardActivation.HasShownStartPopup() && this.postcardActivation.CanShowElseStartLoading();
        }

        private readonly PlayablePostcardActivation postcardActivation;
    }
}
