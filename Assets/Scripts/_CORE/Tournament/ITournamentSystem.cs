using System;

public interface ITournamentSystem
{
	ITournamentControllerFactory ControllerFactory { get; }

	TournamentCloudManager CloudManager { get; }
}
