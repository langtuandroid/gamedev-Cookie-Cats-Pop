using System;
using Tactile.GardenGame.Story.Dialog;
using Tactile.GardenGame.Story.Monologue;
using Tactile.GardenGame.Story.Rewards;
using Tactile.GardenGame.Story.Views;
using TactileModules.GameCore.ButtonArea;

namespace Tactile.GardenGame.Story.Assets
{
	public interface IAssetModel
	{
		BrowseTasksView BrowseTasksView { get; }

		NotEnoughStarsView NotEnoughStarsView { get; }

		TaskCompletableButton TaskCompletableButton { get; }

		ButtonAreaButton TasksButton { get; }

		TasksCheatView TasksCheatView { get; }

		DialogOverlayView DialogOverlayView { get; }

		BarsView BarsView { get; }

		StoryImageView StoryImageView { get; }

		StoryMessageView StoryMessageView { get; }

		MonologueDatabase MonologueDatabase { get; }

		ButtonAreaButton NewTaskBubble { get; }

		StoryCelebrationView StoryCelebrationView { get; }

		TaskCompletedVisuals TaskCompletedVisuals { get; }

		StoryRewardIndicator StoryRewardIndicator { get; }

		TimedTaskView TimedTaskView { get; }
	}
}
