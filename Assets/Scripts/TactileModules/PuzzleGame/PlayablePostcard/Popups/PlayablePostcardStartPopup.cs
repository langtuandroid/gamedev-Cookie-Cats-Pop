using System;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.PlayablePostcard.Model;

namespace TactileModules.PuzzleGame.PlayablePostcard.Popups
{
    public class PlayablePostcardStartPopup : MapFeatureStartPopup
    {
        public PlayablePostcardStartPopup(TactileModules.FeatureManager.FeatureManager featureManager, MapFeatureHandler featureHandler, PlayablePostcardActivation postcardActivation) : base(featureManager, featureHandler)
        {
            this.postcardActivation = postcardActivation;
        }

        protected override bool CanActivateFeature(IFeatureTypeHandler featureHandler)
        {
            return !this.postcardActivation.HasShownStartPopup();
        }

        protected override void ActivateFeature(FeatureData featureData)
        {
            this.postcardActivation.StartPopupHasBeenShown();
        }

        protected override bool ShouldShowPopup()
        {
            return this.postcardActivation.CanShowElseStartLoading();
        }

        private readonly PlayablePostcardActivation postcardActivation;
    }
}
