using System;

namespace Tactile.GardenGame.Story
{
	public interface ITimedTask
	{
		bool HasTimer { get; }

		string GetFormattedTimeRemaining();

		int CoinSkipCost { get; }

		int WaitTimeInSeconds { get; }

		bool IsTimerStarted();

		bool IsTimerComplete();
	}
}
