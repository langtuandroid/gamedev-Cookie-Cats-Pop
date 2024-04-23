using System;

namespace TactileModules.SagaCore
{
	public class SagaCoreSystem : ISagaCoreSystem
	{
		public SagaCoreSystem(MainMapFlowFactory mainMapFlowFactory, MapFacade mapFacade)
		{
			this.MainMapFlowFactory = mainMapFlowFactory;
			this.MapFacade = mapFacade;
		}

		public IMainMapFlowFactory MainMapFlowFactory { get; private set; }

		public MapFacade MapFacade { get; private set; }
	}
}
