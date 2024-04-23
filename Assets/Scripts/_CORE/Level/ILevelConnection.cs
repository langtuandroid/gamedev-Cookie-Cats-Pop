using System;

public interface ILevelConnection
{
	int CompletedLevelSteps { get; }

	int FailedLevelSteps { get; }

	int SessionResultSteps(bool isLevelCompleted);

	ILevelProxy NextLevel(ILevelProxy current, bool isLevelCompleted);
}
