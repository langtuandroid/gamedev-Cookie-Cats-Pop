using System;

public interface IPlayerState
{
	int FarthestUnlockedLevelIndex { get; }

	int FarthestUnlockedLevelHumanNumber { get; }

	bool IsPayingUser { get; }

	bool IsVIP { get; }

	int Lives { get; }
}
