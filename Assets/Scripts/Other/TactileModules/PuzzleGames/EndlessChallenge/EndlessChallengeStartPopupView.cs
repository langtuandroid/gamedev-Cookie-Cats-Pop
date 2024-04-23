using System;
using JetBrains.Annotations;
using TactileModules.FeatureManager;

namespace TactileModules.PuzzleGames.EndlessChallenge
{
    public class EndlessChallengeStartPopupView : UIView
    {
        protected override void ViewLoad(object[] parameters)
        {
        }

        [UsedImplicitly]
        private void OnCloseClicked(UIEvent e)
        {
            base.Close(0);
        }

        [UsedImplicitly]
        private void OnOkClicked(UIEvent e)
        {
            EndlessChallengeHandler featureHandler = TactileModules.FeatureManager.FeatureManager.GetFeatureHandler<EndlessChallengeHandler>();
            featureHandler.StartFlow();
        }
    }
}
