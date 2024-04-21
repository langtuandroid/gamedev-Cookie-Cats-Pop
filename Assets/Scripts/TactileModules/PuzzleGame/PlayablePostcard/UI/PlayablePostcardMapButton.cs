using System;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.MapFeature;
using TactileModules.PuzzleGame.PlayablePostcard.Model;

namespace TactileModules.PuzzleGame.PlayablePostcard.UI
{
    public class PlayablePostcardMapButton : MapFeatureSideButton
    {
        protected PlayablePostcardActivation PlayablePostcardActivation
        {
            get
            {
                return ManagerRepository.Get<PlayablePostcardSystem>().PostcardActivation;
            }
        }

        public override object Data
        {
            get
            {
                return null;
            }
        }

        protected override MapFeatureHandler MapFeatureHandler
        {
            get
            {
                return TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<PlayablePostcardHandler>();
            }
        }

        protected override bool VisibilityCheckerImplementor(object data)
        {
            return this.PlayablePostcardActivation.HasShownStartPopup() && this.PlayablePostcardActivation.CanShowElseStartLoading();
        }
    }
}
