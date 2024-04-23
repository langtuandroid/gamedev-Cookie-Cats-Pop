using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class TournamentControllerFactory : ITournamentControllerFactory
{
	public TournamentControllerFactory(CloudClientBase cloudClientBase, MapFacade mapFacade, IPlayFlowFactory playFlowFactory, IFullScreenManager fullScreenManager, IFlowStack flowStack, TournamentManager manager)
	{
		this.cloudClientBase = cloudClientBase;
		this.mapFacade = mapFacade;
		this.playFlowFactory = playFlowFactory;
		this.fullScreenManager = fullScreenManager;
		this.flowStack = flowStack;
		this.manager = manager;
	}

	public TournamentMapFlow CreateAndPushMapFlow()
	{
		TournamentMapFlow tournamentMapFlow = new TournamentMapFlow(this.cloudClientBase, "Tournament", this.mapFacade, this.playFlowFactory, this.fullScreenManager, this.flowStack, this.manager);
		this.flowStack.Push(tournamentMapFlow);
		return tournamentMapFlow;
	}

	private readonly CloudClientBase cloudClientBase;

	private readonly MapFacade mapFacade;

	private readonly IPlayFlowFactory playFlowFactory;

	private readonly IFullScreenManager fullScreenManager;

	private readonly IFlowStack flowStack;

	private readonly TournamentManager manager;
}
