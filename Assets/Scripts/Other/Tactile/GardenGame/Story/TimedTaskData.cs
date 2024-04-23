using System;

namespace Tactile.GardenGame.Story
{
	public class TimedTaskData : ITimedTask
	{
		public TimedTaskData(MapTask task, IStoryManager storyManager, bool timerEnabled, int coinSkipCost, int waitTimeInSeconds)
		{
			this.storyManager = storyManager;
			this.task = task;
			this.HasTimer = timerEnabled;
			this.CoinSkipCost = coinSkipCost;
			this.WaitTimeInSeconds = waitTimeInSeconds;
		}

		public bool HasTimer { get; private set; }

		public int CoinSkipCost { get; private set; }

		public int WaitTimeInSeconds { get; private set; }

		public bool IsTimerStarted()
		{
			return this.storyManager.IsTaskTimerInProgress(this.task);
		}

		public bool IsTimerComplete()
		{
			return this.storyManager.GetSecondsRemainingInTimedTask(this.task) <= 0;
		}

		public string GetFormattedTimeRemaining()
		{
			if (!this.HasTimer)
			{
				return string.Empty;
			}
			int totalSeconds = (!this.IsTimerStarted()) ? this.WaitTimeInSeconds : this.storyManager.GetSecondsRemainingInTimedTask(this.task);
			return L.FormatSecondsAsColumnSeparated(totalSeconds, L.Get("Complete"), TimeFormatOptions.HideHoursIfZero);
		}

		private readonly IStoryManager storyManager;

		private readonly MapTask task;
	}
}
