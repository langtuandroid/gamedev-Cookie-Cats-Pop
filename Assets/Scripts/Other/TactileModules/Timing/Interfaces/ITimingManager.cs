using System;

namespace TactileModules.Timing.Interfaces
{
	public interface ITimingManager
	{
		event Action<string> TimeDone;

		void CreateTimeStamp(string name, int durationInSeconds);

		int GetTimePassedInSeconds(string name);

		int GetTimeDuration(string name);

		int GetTimeLeftInSeconds(string name);

		bool TimeStampExist(string name);

		void RemoveTimeStampIfItExist(string name);

		void PerformSaveIfNeeded();
	}
}
