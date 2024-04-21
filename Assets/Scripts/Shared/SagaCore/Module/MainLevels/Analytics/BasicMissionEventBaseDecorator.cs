using System;
using TactileModules.Analytics.Interfaces;

namespace Shared.SagaCore.Module.MainLevels.Analytics
{
	public class BasicMissionEventBaseDecorator : IEventDecorator<BasicMissionEventBase>, IEventDecorator
	{
		public BasicMissionEventBaseDecorator(GateManager gateManager)
		{
			this.gateManager = gateManager;
		}

		public void Decorate(BasicMissionEventBase eventObject)
		{
			int gateNumber = 0;
			if (this.gateManager.PlayerOnGate)
			{
				gateNumber = this.gateManager.CurrentGateKeys + 1;
			}
			eventObject.SetGateNumber(gateNumber);
		}

		private readonly GateManager gateManager;
	}
}
