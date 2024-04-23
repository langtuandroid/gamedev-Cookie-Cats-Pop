using System;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank
{
	public class PiggyBankSystem
	{
		public PiggyBankSystem(PiggyBankControllerFactory controllerFactory, IPiggyBankProgression progression, IPiggyBankRewards rewards)
		{
			this.ControllerFactory = controllerFactory;
			this.Progression = progression;
			this.Rewards = rewards;
		}

		public PiggyBankControllerFactory ControllerFactory { get; set; }

		public IPiggyBankProgression Progression { get; set; }

		public IPiggyBankRewards Rewards { get; set; }
	}
}
