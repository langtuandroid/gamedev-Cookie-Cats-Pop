using System;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankControllerFactory
	{
		public PiggyBankControllerFactory(IMainProgression mainProgression, IPiggyBankProvider provider, IPiggyBankProgression progression, IPiggyBankRewards rewards)
		{
			this.rewards = rewards;
			this.mainProgression = mainProgression;
			this.progression = progression;
			this.provider = provider;
		}

		public PiggyBankGameSessionController CreateGameSessionController()
		{
			return new PiggyBankGameSessionController(this.provider, this.rewards, this.progression);
		}

		public PiggyBankTutorialController CreateTutorialController()
		{
			return new PiggyBankTutorialController(new PiggyBankViewFactory(), this.rewards, this.progression);
		}

		public PiggyBankOfferController CreateBankOfferController()
		{
			return new PiggyBankOfferController( new PiggyBankViewFactory(), this.rewards, this);
		}

		public PiggyBankStateController CreateStateController()
		{
			return new PiggyBankStateController(this, new PiggyBankViewFactory(), this.progression, this.rewards, this.mainProgression);
		}

		private readonly IPiggyBankRewards rewards;

		private readonly IPiggyBankProgression progression;

		private readonly IDataProvider<PiggyBankConfig> configGetter;

		private readonly IDataProvider<PiggyBankPersistableState> persistableStateGetter;

		private readonly IMainProgression mainProgression;

		private readonly IPiggyBankProvider provider;
	}
}
