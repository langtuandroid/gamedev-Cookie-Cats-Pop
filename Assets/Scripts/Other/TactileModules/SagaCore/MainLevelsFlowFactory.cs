using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;

namespace TactileModules.SagaCore
{
	public class MainLevelsFlowFactory : IMainLevelsFlowFactory
	{
		public MainLevelsFlowFactory(IPlayFlowFactory playFlowFactory, MainProgressionManager mainProgressionManager, LeaderboardManager leaderboardManager, GateManager gateManager)
		{
			this.playFlowFactory = playFlowFactory;
			this.mainProgressionManager = mainProgressionManager;
			this.leaderboardManager = leaderboardManager;
			this.gateManager = gateManager;
		}

		public GateFlow CreateGateFlow()
		{
			return new GateFlow(this.playFlowFactory, this.gateManager, this.mainProgressionManager);
		}

		public MainLevelFlow CreateMainLevelFlow(LevelProxy proxy)
		{
			return new MainLevelFlow(this.playFlowFactory, proxy, this.mainProgressionManager, this.leaderboardManager);
		}

		private readonly IPlayFlowFactory playFlowFactory;

		private readonly LeaderboardManager leaderboardManager;

		private readonly GateManager gateManager;

		private readonly MainProgressionManager mainProgressionManager;
	}
}
