using System;
using Tactile;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.Lives;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Controllers
{
	public class SlidesAndLaddersControllerFactory : ISlidesAndLaddersControllerFactory
	{
		public SlidesAndLaddersControllerFactory(ISlidesAndLaddersHandler featureHandler, ISlidesAndLaddersLevelDatabase levelDatabase, ISlidesAndLaddersInventory inventory, ISlidesAndLaddersSave save, ILivesManager lives, ConfigurationManager configurationManager, IPlayFlowFactory playLevelFacade, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack)
		{
			this.featureHandler = featureHandler;
			this.levelDatabase = levelDatabase;
			this.inventory = inventory;
			this.save = save;
			this.lives = lives;
			this.configurationManager = configurationManager;
			this.playLevelFacade = playLevelFacade;
			this.mapFacade = mapFacade;
			this.fullScreenManager = fullScreenManager;
			this.flowStack = flowStack;
		}

		public SlidesAndLaddersRewardController CreateRewardController()
		{
			return new SlidesAndLaddersRewardController(this.GetRewards(), this.inventory, this.GetConfig<SlidesAndLaddersConfig>());
		}

		public SlidesAndLaddersPlayFlow CreatePlayFlow(LevelProxy levelToPlay)
		{
			return new SlidesAndLaddersPlayFlow(this.playLevelFacade, levelToPlay, this.GetFeatureProgression());
		}

		public SlidesAndLaddersMapFlow CreateAndPushMapFlow()
		{
			SlidesAndLaddersMapFlow slidesAndLaddersMapFlow = new SlidesAndLaddersMapFlow("SlidesAndLadders", this.mapFacade, this.fullScreenManager, this.flowStack, this.GetRewards(), this.lives, this.GetFeatureProgression(), this, this.levelDatabase);
			this.flowStack.Push(slidesAndLaddersMapFlow);
			return slidesAndLaddersMapFlow;
		}

		public SlidesAndLaddersWheelTutorialController CreateTutorialController(SlidesAndLaddersWheelWidget wheelWidget, UIScrollablePanel scrollablePanel, SlidesAndLaddersMapButtonView slidesAndLaddersMapButtonView)
		{
			return new SlidesAndLaddersWheelTutorialController(this.GetFeatureDataProvider<SlidesAndLaddersInstanceCustomData>(), wheelWidget, scrollablePanel, slidesAndLaddersMapButtonView);
		}

		public SlidesAndLaddersWheelWidgetController CreateWheelController(SlidesAndLaddersWheelWidget wheelWidget)
		{
			return new SlidesAndLaddersWheelWidgetController(this.GetFeatureDataProvider<SlidesAndLaddersInstanceCustomData>(), wheelWidget);
		}

		public ISlidesAndLaddersFeatureProgression GetFeatureProgression()
		{
			return new SlidesAndLaddersFeatureProgression(this.GetFeatureDataProvider<SlidesAndLaddersInstanceCustomData>(), this.levelDatabase);
		}

		public ISlidesAndLaddersRewards GetRewards()
		{
			return new SlidesAndLaddersRewards(this.GetFeatureDataProvider<SlidesAndLaddersInstanceCustomData>(), this.GetFeatureDataProvider<SlidesAndLaddersMetaData>(), this.GetConfig<SlidesAndLaddersConfig>(), this.levelDatabase);
		}

		private IDataProvider<T> GetFeatureDataProvider<T>()
		{
			return new FeatureDataProvider<T>(this.featureHandler);
		}

		private ConfigProvider<T> GetConfig<T>()
		{
			return new ConfigProvider<T>(this.configurationManager);
		}

		private readonly ISlidesAndLaddersHandler featureHandler;

		private readonly ISlidesAndLaddersLevelDatabase levelDatabase;

		private readonly ISlidesAndLaddersInventory inventory;

		private readonly ISlidesAndLaddersSave save;

		private readonly ILivesManager lives;

		private readonly ConfigurationManager configurationManager;

		private readonly IPlayFlowFactory playLevelFacade;

		private readonly MapFacade mapFacade;

		private readonly IFullScreenManager fullScreenManager;

		private readonly IFlowStack flowStack;
	}
}
