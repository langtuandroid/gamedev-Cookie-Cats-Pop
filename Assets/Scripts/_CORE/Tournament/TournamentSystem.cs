using System;

public class TournamentSystem : ITournamentSystem
{
	public TournamentSystem(TournamentControllerFactory controllerFactory, TournamentCloudManager cloudManager)
	{
		this.ControllerFactory = controllerFactory;
		this.CloudManager = cloudManager;
	}

	public ITournamentControllerFactory ControllerFactory { get; private set; }

	public TournamentCloudManager CloudManager { get; private set; }
}
