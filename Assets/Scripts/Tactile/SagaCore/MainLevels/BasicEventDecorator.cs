using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGame.MainLevels;

namespace Tactile.SagaCore.MainLevels
{
	public class BasicEventDecorator : IEventDecorator<BasicEvent>, IEventDecorator
	{
		public BasicEventDecorator(IMainProgression mainProgression, GateManager gateManager)
		{
			this.mainProgression = mainProgression;
			this.gateManager = gateManager;
		}

		public void Decorate(BasicEvent eventObject)
		{
			int farthestUnlockedLevelHumanNumber = this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
			double userProgressAsDouble = this.GetUserProgressAsDouble();
			eventObject.SetUserProgression(farthestUnlockedLevelHumanNumber, userProgressAsDouble);
		}

		private double GetUserProgressAsDouble()
		{
			int farthestUnlockedLevelHumanNumber = this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
			int num = 0;
			if (this.gateManager.PlayerOnGate)
			{
				num = this.gateManager.CurrentGateKeys + 1;
			}
			return (double)farthestUnlockedLevelHumanNumber + (double)num * 0.1;
		}

		private readonly IMainProgression mainProgression;

		private readonly GateManager gateManager;
	}
}
