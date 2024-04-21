using System;
using TactileModules.PuzzleGames.Configuration;

namespace Tactile.GardenGame.Story
{
	public class TimedTaskModel
	{
		public TimedTaskModel(IConfigGetter<StoryConfig> configGetter)
		{
			this.configGetter = configGetter;
		}

		public void SetStoryManager(IStoryManager storyManager)
		{
			this.storyManager = storyManager;
		}

		private StoryConfig Config
		{
			get
			{
				return this.configGetter.Get();
			}
		}

		public ITimedTask GetTimedTask(MapTask mapTask)
		{
			bool timerEnabled = false;
			int coinSkipCost = 0;
			int waitTimeInSeconds = 0;
			TimedTaskParameters component = mapTask.GetComponent<TimedTaskParameters>();
			if (component != null)
			{
				timerEnabled = component.timerEnabled;
				coinSkipCost = component.coinSkipCost;
				waitTimeInSeconds = component.waitTimeSeconds;
			}
			foreach (StoryConfig.TimedTask timedTask in this.Config.TimedTasks)
			{
				if (timedTask.TaskObjectName == mapTask.name)
				{
					timerEnabled = timedTask.TimerEnabled;
					coinSkipCost = timedTask.CoinSkipCost;
					waitTimeInSeconds = timedTask.WaitTimeInSeconds;
					break;
				}
			}
			return new TimedTaskData(mapTask, this.storyManager, timerEnabled, coinSkipCost, waitTimeInSeconds);
		}

		private readonly IConfigGetter<StoryConfig> configGetter;

		private IStoryManager storyManager;
	}
}
