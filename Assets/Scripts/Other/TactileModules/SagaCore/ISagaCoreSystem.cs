using System;

namespace TactileModules.SagaCore
{
	public interface ISagaCoreSystem
	{
		IMainMapFlowFactory MainMapFlowFactory { get; }

		MapFacade MapFacade { get; }
	}
}
