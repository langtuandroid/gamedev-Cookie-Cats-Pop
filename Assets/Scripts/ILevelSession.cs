using System;
using System.Collections.Generic;

public interface ILevelSession
{
	LevelProxy Level { get; }

	int Points { get; }

	bool FirstTimeCompleted { get; }

	LevelSessionState SessionState { get; }

	int MovesUsed { get; }

	IEnumerable<ITutorialStep> GetTutorialSteps();
}
