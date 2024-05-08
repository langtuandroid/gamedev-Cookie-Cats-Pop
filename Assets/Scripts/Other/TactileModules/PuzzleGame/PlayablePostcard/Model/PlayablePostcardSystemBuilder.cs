using System;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PlayablePostcard.Data;
using TactileModules.PuzzleGame.PlayablePostcard.Module.Controllers;
using TactileModules.PuzzleGame.PlayablePostcard.Popups;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;
using TactileModules.TactileCloud.AssetBundles;

namespace TactileModules.PuzzleGame.PlayablePostcard.Model
{
    public static class PlayablePostcardSystemBuilder
    {
        public static PlayablePostcardSystem Build(TactileModules.FeatureManager.FeatureManager featureManager, ConfigurationManager configurationManager, LevelDatabaseCollection levelDatabaseCollection, MainProgressionManager mainProgressionManager, AssetBundleManager assetBundleManager, IAssetBundleDownloader assetBundleDownloader, UIViewManager uiViewManager, IPlayablePostcardProvider provider, MapFacade mapFacade, IFlowStack flowStack, IFullScreenManager fullScreenManager, IPlayFlowFactory playLevelFacade)
        {
            PlayablePostcardLevelDatabase levelDatabase = levelDatabaseCollection.GetLevelDatabase<PlayablePostcardLevelDatabase>("PlayablePostcard");
            PlayablePostcardConfig config = configurationManager.GetConfig<PlayablePostcardConfig>();
            PlayablePostcardMapFeatureProvider playablePostcardMapFeatureProvider = new PlayablePostcardMapFeatureProvider(provider);
            PlayablePostcardHandler playablePostcardHandler = new PlayablePostcardHandler(featureManager, playablePostcardMapFeatureProvider, config, mainProgressionManager);
            FeatureDataProvider<PlayablePostcardInstanceCustomData> featureInstanceData = new FeatureDataProvider<PlayablePostcardInstanceCustomData>(playablePostcardHandler);
            FeatureDataProvider<PlayablePostcardMetaData> metaData = new FeatureDataProvider<PlayablePostcardMetaData>(playablePostcardHandler);
            PostcardAssetBundleDownloader postcardAssetBundleDownloader = new PostcardAssetBundleDownloader(assetBundleManager, assetBundleDownloader);
            PlayablePostcardActivation postcardActivation = new PlayablePostcardActivation(playablePostcardHandler, featureInstanceData, metaData, postcardAssetBundleDownloader);
            PlayablePostcardControllerFactory controllerFactory = new PlayablePostcardControllerFactory(levelDatabase, featureInstanceData, postcardActivation, uiViewManager, mapFacade, flowStack, fullScreenManager, playLevelFacade);
            playablePostcardMapFeatureProvider.OnSwitchToFeatureMapView += delegate ()
            {
                PlayablePostcardProgress playablePostcardProgress = new PlayablePostcardProgress(featureInstanceData.Get());
                if (playablePostcardProgress.HasCompletedPostcard())
                {
                    controllerFactory.CreateAndPushEndedFlow();
                }
                else
                {
                    controllerFactory.CreateAndPushMapFlow();
                }
            };
            new PlayablePostcardStartPopup(featureManager, playablePostcardHandler, postcardActivation);
            new PlayablePostcardSessionStartPopup(featureManager, playablePostcardHandler, postcardActivation);
            new PlayablePostcardEndPopup(featureManager, playablePostcardHandler);
            return new PlayablePostcardSystem(playablePostcardHandler, postcardActivation, provider);
        }
    }
}
