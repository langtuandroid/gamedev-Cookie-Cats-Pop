using System;
using Tactile.GameCore.Settings.Buttons;
using Tactile.GardenGame.Story.Assets;
using TactileModules.GameCore.ButtonArea;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Story
{
	public class TasksButtonHandler : ButtonHandler
	{
		public TasksButtonHandler(IButtonAreaModel buttonAreaModel, IStoryManager storyManager, IAssetModel assets, BrowseTasksFactory browseTasksFactory, FlowStack flowStack) : base(buttonAreaModel, assets.TasksButton)
		{
			this.storyManager = storyManager;
			this.browseTasksFactory = browseTasksFactory;
			this.flowStack = flowStack;
		}

		protected override void Clicked()
		{
			BrowseTasksFlow c = this.browseTasksFactory.CreateBrowseTasksFlow(new Action(this.storyManager.NotEnoughStarsPlayedWasClicked));
			this.flowStack.Push(c);
			this.storyManager.BrowseTaskStart();
		}

		private readonly IStoryManager storyManager;

		private readonly BrowseTasksFactory browseTasksFactory;

		private readonly FlowStack flowStack;
	}
}
