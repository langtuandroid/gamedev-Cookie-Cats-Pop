using System;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankControllerFactory
	{
		public PiggyBankControllerFactory(IMainProgression mainProgression, IIAPProvider iapProvider, IPiggyBankProvider provider, InAppPurchaseManager inAppPurchaseManager, IPiggyBankProgression progression, IPiggyBankRewards rewards)
		{
			this.rewards = rewards;
			this.mainProgression = mainProgression;
			this.iapProvider = iapProvider;
			this.inAppPurchaseManager = inAppPurchaseManager;
			this.progression = progression;
			this.provider = provider;
		}

		public PiggyBankGameSessionController CreateGameSessionController()
		{
			return new PiggyBankGameSessionController(this.provider, this.rewards, this.progression);
		}

		public PiggyBankIAPController CreateIAPController()
		{
			return new PiggyBankIAPController(this.inAppPurchaseManager, this.iapProvider);
		}

		public PiggyBankTutorialController CreateTutorialController()
		{
			return new PiggyBankTutorialController(new PiggyBankViewFactory(), this.rewards, this.progression);
		}

		public PiggyBankOfferController CreateBankOfferController()
		{
			return new PiggyBankOfferController(this.iapProvider, new PiggyBankViewFactory(), this.rewards, this);
		}

		public PiggyBankStateController CreateStateController()
		{
			return new PiggyBankStateController(this, new PiggyBankViewFactory(), this.progression, this.rewards, this.iapProvider, this.mainProgression);
		}

		private readonly IPiggyBankRewards rewards;

		private readonly InAppPurchaseManager inAppPurchaseManager;

		private readonly IPiggyBankProgression progression;

		private readonly IDataProvider<PiggyBankConfig> configGetter;

		private readonly IDataProvider<PiggyBankPersistableState> persistableStateGetter;

		private readonly IMainProgression mainProgression;

		private readonly IIAPProvider iapProvider;

		private readonly IPiggyBankProvider provider;
	}
}
