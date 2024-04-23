using System;
using Tactile.GardenGame.Story.Dialog;
using Tactile.GardenGame.Story.Monologue;
using Tactile.GardenGame.Story.Rewards;
using Tactile.GardenGame.Story.Views;
using TactileModules.GameCore.ButtonArea;
using UnityEngine;

namespace Tactile.GardenGame.Story.Assets
{
	public class AssetModel : IAssetModel
	{
		public BrowseTasksView BrowseTasksView
		{
			get
			{
				return Resources.Load<BrowseTasksView>("Story/BrowseTasksView");
			}
		}

		public NotEnoughStarsView NotEnoughStarsView
		{
			get
			{
				return Resources.Load<NotEnoughStarsView>("Story/NotEnoughStarsView");
			}
		}

		public TaskCompletableButton TaskCompletableButton
		{
			get
			{
				return Resources.Load<TaskCompletableButton>("Story/TaskCompletableButton");
			}
		}

		public ButtonAreaButton TasksButton
		{
			get
			{
				return Resources.Load<ButtonAreaButton>("Story/TasksButton");
			}
		}

		public TasksCheatView TasksCheatView
		{
			get
			{
				return Resources.Load<TasksCheatView>("Story/TasksCheatView");
			}
		}

		public DialogOverlayView DialogOverlayView
		{
			get
			{
				return Resources.Load<DialogOverlayView>("Story/DialogOverlayView");
			}
		}

		public BarsView BarsView
		{
			get
			{
				return Resources.Load<BarsView>("Story/BarsView");
			}
		}

		public StoryImageView StoryImageView
		{
			get
			{
				return Resources.Load<StoryImageView>("Story/StoryImageView");
			}
		}

		public StoryMessageView StoryMessageView
		{
			get
			{
				return Resources.Load<StoryMessageView>("Story/StoryMessageView");
			}
		}

		public MonologueDatabase MonologueDatabase
		{
			get
			{
				return Resources.Load<MonologueDatabase>("Story/MonologueDatabase");
			}
		}

		public ButtonAreaButton NewTaskBubble
		{
			get
			{
				return Resources.Load<ButtonAreaButton>("Story/NewTaskBubble");
			}
		}

		public StoryCelebrationView StoryCelebrationView
		{
			get
			{
				return Resources.Load<StoryCelebrationView>("Story/StoryCelebrationView");
			}
		}

		public TaskCompletedVisuals TaskCompletedVisuals
		{
			get
			{
				return Resources.Load<TaskCompletedVisuals>("Story/TaskCompletedVisuals");
			}
		}

		public StoryRewardIndicator StoryRewardIndicator
		{
			get
			{
				return Resources.Load<StoryRewardIndicator>("Story/StoryRewardIndicator");
			}
		}

		public TimedTaskView TimedTaskView
		{
			get
			{
				return Resources.Load<TimedTaskView>("Story/TimedTaskView");
			}
		}
	}
}
