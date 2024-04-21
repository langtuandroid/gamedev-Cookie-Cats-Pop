using System;
using System.Diagnostics;
using Tactile.GardenGame.Story.Assets;
using Tactile.GardenGame.Story.Rewards;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Story
{
	public class BrowseTasksFactory
	{
		public BrowseTasksFactory(IUIController uiController, IStoryManager storyManager, IVisualInventory visualInventory, IStoryRewardsFactory storyRewardsFactory, IAssetModel assets, TimedTaskModel timedTaskModel, FlowStack flowStack)
		{
			this.uiController = uiController;
			this.storyManager = storyManager;
			this.visualInventory = visualInventory;
			this.storyRewardsFactory = storyRewardsFactory;
			this.assets = assets;
			this.timedTaskModel = timedTaskModel;
			this.flowStack = flowStack;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BrowseTasksFlow> OnCreated;



		public BrowseTasksFlow CreateBrowseTasksFlow(Action notEnoughStarsPlayClicked)
		{
			BrowseTasksFlow browseTasksFlow = new BrowseTasksFlow(this.uiController, this.storyManager, this.visualInventory, notEnoughStarsPlayClicked, this.storyRewardsFactory, this.assets, this.timedTaskModel, this.flowStack);
			if (this.OnCreated != null)
			{
				this.OnCreated(browseTasksFlow);
			}
			return browseTasksFlow;
		}

		private readonly IUIController uiController;

		private readonly IStoryManager storyManager;

		private readonly IVisualInventory visualInventory;

		private readonly IStoryRewardsFactory storyRewardsFactory;

		private readonly IAssetModel assets;

		private readonly TimedTaskModel timedTaskModel;

		private readonly FlowStack flowStack;
	}
}
