using System;

namespace TactileModules.SagaCore
{
	public interface IMainLevelsFlowFactory
	{
		GateFlow CreateGateFlow();

		MainLevelFlow CreateMainLevelFlow(LevelProxy proxy);
	}
}
