using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.PlayablePostcard.Controllers;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGame.PlayablePostcard.Module.Controllers
{
	public class PlayablePostcardControllerFactory : IPlayablePostcardControllerFactory
	{
		public PlayablePostcardControllerFactory(PlayablePostcardLevelDatabase levelDatabase, FeatureDataProvider<PlayablePostcardInstanceCustomData> featureDataProvider, PlayablePostcardActivation postcardActivation, UIViewManager uiViewManager, MapFacade mapFacade, IFlowStack flowStack, IFullScreenManager fullScreenManager, IPlayFlowFactory playFlowFactory)
		{
			this.levelDatabase = levelDatabase;
			this.featureDataProvider = featureDataProvider;
			this.postcardActivation = postcardActivation;
			this.uiViewManager = uiViewManager;
			this.mapFacade = mapFacade;
			this.flowStack = flowStack;
			this.fullScreenManager = fullScreenManager;
			this.playFlowFactory = playFlowFactory;
		}

		public PlayablePostcardMapFlow CreateAndPushMapFlow()
		{
			PlayablePostcardProgress progress = new PlayablePostcardProgress(this.featureDataProvider.Get());
			PlayablePostcardMapFlow playablePostcardMapFlow = new PlayablePostcardMapFlow(this.levelDatabase, progress, this.mapFacade, this.fullScreenManager, this.flowStack, this, this.uiViewManager);
			this.flowStack.Push(playablePostcardMapFlow);
			return playablePostcardMapFlow;
		}

		public PlayablePostcardEndedFlow CreateAndPushEndedFlow()
		{
			PlayablePostcardEndedFlow playablePostcardEndedFlow = new PlayablePostcardEndedFlow(this, new PlayablePostcardProgress(this.featureDataProvider.Get()));
			this.flowStack.Push(playablePostcardEndedFlow);
			return playablePostcardEndedFlow;
		}

		public PlayablePostcardPlayFlow CreatePlayFlow(LevelProxy levelProxy)
		{
			PlayablePostcardProgress progress = new PlayablePostcardProgress(this.featureDataProvider.Get());
			return new PlayablePostcardPlayFlow(this.playFlowFactory, levelProxy, progress, this);
		}

		public PhotoBoothController CreatePhotoBoothController(PlayablePostcardProgress model)
		{
			return new PhotoBoothController(model, this.postcardActivation, this.uiViewManager);
		}

		private readonly PlayablePostcardLevelDatabase levelDatabase;

		private readonly FeatureDataProvider<PlayablePostcardInstanceCustomData> featureDataProvider;

		private readonly PlayablePostcardActivation postcardActivation;

		private readonly UIViewManager uiViewManager;

		private readonly MapFacade mapFacade;

		private readonly IFlowStack flowStack;

		private readonly IFullScreenManager fullScreenManager;

		private readonly IPlayFlowFactory playFlowFactory;
	}
}
