using System;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

internal class GameSessionManagerProvider : GameSessionManager.IGameSessionManagerProvider
{
	GameSessionConfig GameSessionManager.IGameSessionManagerProvider.Configuration
	{
		get
		{
			return ConfigurationManager.Get<GameSessionConfig>();
		}
	}

	bool GameSessionManager.IGameSessionManagerProvider.IsOnAMap
	{
		get
		{
			FlowStack flowStack = ManagerRepository.Get<FlowStack>();
			return flowStack.Top is MapFlow;
		}
	}
}
