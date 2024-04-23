using System;

namespace Tactile.GardenGame.Story
{
	public class CheatViewTimedTaskData : ITimedTask
	{
		public bool HasTimer
		{
			get
			{
				return false;
			}
		}

		public string GetFormattedTimeRemaining()
		{
			return string.Empty;
		}

		public int CoinSkipCost
		{
			get
			{
				return 0;
			}
		}

		public int WaitTimeInSeconds
		{
			get
			{
				return 0;
			}
		}

		public bool IsTimerStarted()
		{
			return false;
		}

		public bool IsTimerComplete()
		{
			return false;
		}
	}
}
