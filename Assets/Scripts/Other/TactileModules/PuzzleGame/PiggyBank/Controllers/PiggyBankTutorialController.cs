using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankTutorialController : IFiberRunnable
	{
		public PiggyBankTutorialController(IPiggyBankViewFactory viewFactory, IPiggyBankRewards rewards, IPiggyBankProgression progression)
		{
			this.viewFactory = viewFactory;
			this.rewards = rewards;
			this.progression = progression;
		}

		public IEnumerator Run()
		{
			UIViewManager.UIViewState vs = this.viewFactory.ShowView<PiggyBankTutorialView>(false);
			PiggyBankTutorialView view = (PiggyBankTutorialView)vs.View;
			view.Initialize(this.progression.GetNextFreeOpenLevelHumanNumber(), this.rewards.FreeOpenInterval, this.rewards.CoinsRequiredForPaidOpening);
			yield return vs.WaitForClose();
			this.progression.ShowedTutorial();
			this.progression.SavePersistabelState();
			yield break;
		}

		public void OnExit()
		{
		}

		private readonly IPiggyBankViewFactory viewFactory;

		private readonly IPiggyBankRewards rewards;

		private readonly IPiggyBankProgression progression;
	}
}
