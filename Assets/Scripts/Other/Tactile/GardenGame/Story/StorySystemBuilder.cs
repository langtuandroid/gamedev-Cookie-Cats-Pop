using System;
using Shared.UserSettings.Module;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Shop;
using Tactile.GardenGame.Story.Assets;
using Tactile.GardenGame.Story.Rewards;
using TactileModules.GameCore.ButtonArea;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Rewards;
using TactileModules.GameCore.Rewards.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Story
{
	public class StorySystemBuilder
	{
		public static StorySystem Build(FlowStack flowStack, IVisualInventory visualInventory, IUIController uiController, IButtonAreaModel buttonAreaModel, UserSettingsManager userSettingsManager, PropsManager propsManager, IConfigurationManager configurationManager, IShopViewFlowFactory shopViewFlowFactory, IStoryAudio storyAudio)
		{
			Tactile.GardenGame.Story.Assets.AssetModel assets = new Tactile.GardenGame.Story.Assets.AssetModel();
			UserSettingsGetter<StoryManager.PersistableState> state = new UserSettingsGetter<StoryManager.PersistableState>(userSettingsManager);
			ConfigGetter<StoryConfig> config = new ConfigGetter<StoryConfig>(configurationManager);
			TimedTaskModel timedTaskModel = new TimedTaskModel(new ConfigGetter<StoryConfig>(configurationManager));
			StoryManager storyManager = new StoryManager(flowStack, visualInventory, uiController, buttonAreaModel, userSettingsManager, assets, shopViewFlowFactory, timedTaskModel, propsManager);
			timedTaskModel.SetStoryManager(storyManager);
			RewardsFactory rewardsFactory = new RewardsFactory(visualInventory, uiController, new TactileModules.GameCore.Rewards.Assets.AssetModel());
			StoryRewardsFactory storyRewardsFactory = new StoryRewardsFactory(config, state, storyManager, rewardsFactory, userSettingsManager);
			StoryMapControllerFactory storyMapControllerFactory = new StoryMapControllerFactory(uiController, propsManager, storyAudio);
			StoryControllerFactory storyControllerFactory = new StoryControllerFactory(storyManager, storyMapControllerFactory, propsManager);
			BrowseTasksFactory browseTasksFactory = new BrowseTasksFactory(uiController, storyManager, visualInventory, storyRewardsFactory, assets, timedTaskModel, flowStack);
			return new StorySystem(storyManager, browseTasksFactory, storyControllerFactory);
		}
	}
}
