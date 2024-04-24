using System;

public interface IPlayerState
{
	int FarthestUnlockedLevelIndex { get; }

	int FarthestUnlockedLevelHumanNumber { get; }

	bool IsPayingUser { get; }

	int Lives { get; }
}
