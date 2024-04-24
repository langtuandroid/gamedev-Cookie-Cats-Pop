using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.PiggyBank.Analytics;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using TactileModules.PuzzleGame.PiggyBank.UI;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankStateController
	{
		public PiggyBankStateController(PiggyBankControllerFactory controllerFactory, IPiggyBankViewFactory viewFactory, IPiggyBankProgression progression, IPiggyBankRewards rewards, IMainProgression mainProgression)
		{
			this.controllerFactory = controllerFactory;
			this.mainProgression = mainProgression;
			this.progression = progression;
			this.rewards = rewards;
			this.viewFactory = viewFactory;
			this.Initialize();
		}

		private void Initialize()
		{
			if (!this.progression.IsFeatureEnabled())
			{
				return;
			}
			if (!this.IsActive() && this.progression.IsAvailableOnLevel(this.mainProgression.GetFarthestUnlockedLevelHumanNumber()))
			{
				this.StartPiggyBank();
			}
		}

		public PiggyBankStateController.PiggyBankState GetCurrentState()
		{
			if (!this.IsFeatureStarted())
			{
				return PiggyBankStateController.PiggyBankState.Inactive;
			}
			if (!this.progression.TutorialShown)
			{
				return PiggyBankStateController.PiggyBankState.Tutorial;
			}
			if (this.progression.IsNextFreeOpenReady())
			{
				return PiggyBankStateController.PiggyBankState.FreeOpening;
			}
			if (this.rewards.PaidOpeningReady())
			{
				return PiggyBankStateController.PiggyBankState.PaidOpening;
			}
			return PiggyBankStateController.PiggyBankState.Locked;
		}

		public void StartPiggyBank()
		{
			this.progression.SetStartingLevel(this.mainProgression.GetFarthestUnlockedLevelHumanNumber());
			this.progression.SavePersistabelState();
		}

		public bool IsActive()
		{
			return this.IsFeatureStarted() && this.progression.TutorialShown;
		}

		private bool IsFeatureStarted()
		{
			return this.progression.IsFeatureEnabled() && this.progression.StartingLevel > 0;
		}

		public IEnumerator RunPiggyBankViewFlow()
		{
			UIViewManager.UIViewState vs = this.viewFactory.ShowView<PiggyBankView>(false);
			this.view = (PiggyBankView)vs.View;
			this.view.OpenButtonClicked += this.OpenButtonClickedHandler;
			this.view.TutorialButtonClicked += this.TutorialClickedHandler;
			this.view.Initialize(this.GetCurrentState(), this.progression, this.rewards);
			yield return vs.WaitForClose();
			yield break;
		}

		private void OpenButtonClickedHandler()
		{
			if (this.GetCurrentState() == PiggyBankStateController.PiggyBankState.FreeOpening)
			{
				this.StartShowFreeOpened();
			}
			else
			{
				
			}
		}

		private void StartShowFreeOpened()
		{
			this.fiber.Start(this.ShowFreeOpen());
		}

		private IEnumerator ShowFreeOpen()
		{
			this.coinsToAnimate = this.rewards.CollectedCoins;
			yield return this.view.ShowPiggyBankFreeOpened(this.coinsToAnimate);
			this.ClaimFreeContent();
			yield break;
		}

		private void TutorialClickedHandler()
		{
			this.StartShowTutorial();
		}

		private void StartShowTutorial()
		{
			this.fiber.Start(this.ShowTutorial());
		}

		private IEnumerator ShowTutorial()
		{
			yield return this.controllerFactory.CreateTutorialController();
			yield break;
		}
		

		private void PurchaseCancelledHandler()
		{
			this.view.PurchaseCancelled();
		}

		private void ShowPiggyBankBought()
		{
			this.animateFiber.Start(this.view.ShowPiggyBankBought(this.coinsToAnimate));
		}

		public bool HasHandledUnlockVisuals()
		{
			return this.progression.HasHandledUnlockVisuals();
		}

		public void UnlockVisualsHandled()
		{
			this.progression.UnlockVisualsHandled();
		}

		private void ClaimFreeContent()
		{
			PiggyBankAnalytics.LogContentClaimed(this.rewards.CollectedCoins, this.rewards.Capacity);
			this.progression.UpdateToNextFreeLevel();
			this.ClaimContent();
		}

		private void ClaimPurchasedContent()
		{
			PiggyBankAnalytics.LogContentPurchased(this.rewards.CollectedCoins, this.rewards.Capacity);
			this.rewards.IncreaseCapacity();
			this.ClaimContent();
			this.ShowPiggyBankBought();
		}

		private void ClaimContent()
		{
			this.rewards.GiveContentToPlayer();
			this.rewards.ResetCoins();
			this.progression.ResetUnlockVisualsHandled();
			this.rewards.SavePersistabelState();
		}

		private readonly PiggyBankControllerFactory controllerFactory;

		private readonly IPiggyBankViewFactory viewFactory;

		private readonly IPiggyBankProgression progression;

		private readonly IPiggyBankRewards rewards;

		private readonly IMainProgression mainProgression;

		private PiggyBankView view;

		private Fiber fiber = new Fiber();

		private Fiber animateFiber = new Fiber();

		private int coinsToAnimate;

		public enum PiggyBankState
		{
			Inactive,
			Tutorial,
			Locked,
			FreeOpening,
			PaidOpening
		}
	}
}
