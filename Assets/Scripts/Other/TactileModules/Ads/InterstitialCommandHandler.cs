using System;
using JetBrains.Annotations;
using TactileModules.TactilePrefs;

namespace TactileModules.Ads
{
    public class InterstitialCommandHandler : BaseCommandHandler
    {
        public static void InjectDependencies(IInterstitialPresenter injectedInterstitialPresenter, ILocalStorageObject<InterstitialProviderManagerData> storage)
        {
            InterstitialCommandHandler.interstitialPresenter = injectedInterstitialPresenter;
            InterstitialCommandHandler.interstitialProviderManagerStorage = storage;
        }

        [UsedImplicitly]

        private static void RequestRewardedVideo()
        {
            InterstitialCommandHandler.interstitialPresenter.RequestInterstitial();
        }

        [UsedImplicitly]

        private static void ShowRewardedVideo()
        {
            FiberCtrl.Pool.Run(InterstitialCommandHandler.interstitialPresenter.ShowInterstitial(), false);
        }

        private static IInterstitialPresenter interstitialPresenter;

        private static ILocalStorageObject<InterstitialProviderManagerData> interstitialProviderManagerStorage;
    }
}
