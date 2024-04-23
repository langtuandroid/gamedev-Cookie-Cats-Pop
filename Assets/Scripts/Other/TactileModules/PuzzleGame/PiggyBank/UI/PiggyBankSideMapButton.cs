using System;
using Fibers;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public abstract class PiggyBankSideMapButton : SideMapButton
	{
		private void Awake()
		{
			this.controllerFactory = ManagerRepository.Get<PiggyBankSystem>().ControllerFactory;
			this.stateController = this.controllerFactory.CreateStateController();
			this.rewards = ManagerRepository.Get<PiggyBankSystem>().Rewards;
			this.button.Clicked += this.ClickedHandler;
		}

		protected override void UpdateOncePerSecond()
		{
			if (this.coins != this.rewards.CollectedCoins || this.capacity != this.rewards.Capacity)
			{
				this.UpdateButton();
			}
		}

		protected override void Setup()
		{
			base.Setup();
			this.UpdateButton();
		}

		private void UpdateButton()
		{
			this.coins = this.rewards.CollectedCoins;
			this.capacity = this.rewards.Capacity;
			this.contentLabel.text = this.GetButtonText();
			PiggyBankStateController.PiggyBankState currentState = this.stateController.GetCurrentState();
			this.ShowState(currentState);
			this.piggyBankBadge.SetActive(this.IsUnlocked(currentState));
		}

		private bool IsUnlocked(PiggyBankStateController.PiggyBankState state)
		{
			return state == PiggyBankStateController.PiggyBankState.PaidOpening || state == PiggyBankStateController.PiggyBankState.FreeOpening;
		}

		private string GetButtonText()
		{
			return this.rewards.CollectedCoins + " / " + this.rewards.Capacity;
		}

		private void ClickedHandler(UIButton uiButton)
		{
			this.ShowPiggyBankView();
		}

		private void ShowPiggyBankView()
		{
			this.showViewFiber.Start(this.stateController.RunPiggyBankViewFlow());
		}

		private void OnDisable()
		{
			this.showViewFiber.Terminate();
		}

		public override SideMapButton.AreaSide Side
		{
			get
			{
				return SideMapButton.AreaSide.Right;
			}
		}

		public override bool VisibilityChecker(object data)
		{
			return this.stateController.IsActive();
		}

		public override Vector2 Size
		{
			get
			{
				return this.GetElementSize();
			}
		}

		public override object Data
		{
			get
			{
				return null;
			}
		}

		protected bool ShouldShowUnlockAnimation(PiggyBankStateController.PiggyBankState state)
		{
			return (state == PiggyBankStateController.PiggyBankState.FreeOpening || state == PiggyBankStateController.PiggyBankState.PaidOpening) && !this.stateController.HasHandledUnlockVisuals();
		}

		protected string GetAnimationNameForState(PiggyBankStateController.PiggyBankState state)
		{
			switch (state)
			{
			case PiggyBankStateController.PiggyBankState.Tutorial:
				return this.tutorialStateAnimationName;
			case PiggyBankStateController.PiggyBankState.Locked:
				return this.lockedStateAnimationName;
			case PiggyBankStateController.PiggyBankState.FreeOpening:
				return this.freeOpeningStateAnimationName;
			case PiggyBankStateController.PiggyBankState.PaidOpening:
				return this.paidOpeningStateAnimationName;
			default:
				return string.Empty;
			}
		}

		private void ShowState(PiggyBankStateController.PiggyBankState state)
		{
			if (this.ShouldShowUnlockAnimation(state))
			{
				this.PlayUnlockSpine(this.unlockAnimationName);
			}
			if (state == PiggyBankStateController.PiggyBankState.Inactive)
			{
				return;
			}
			string animationNameForState = this.GetAnimationNameForState(state);
			if (this.ShouldAddAnimation(animationNameForState))
			{
				this.PlayAnimation(animationNameForState);
			}
		}

		protected abstract void PlayUnlockSpine(string animationName);

		protected abstract bool ShouldAddAnimation(string animationName);

		protected abstract void PlayAnimation(string animationName);

		[SerializeField]
		private GameObject piggyBankBadge;

		[SerializeField]
		private UIButton button;

		[SerializeField]
		private UILabel contentLabel;

		[Header("Animation names")]
		[SerializeField]
		private string unlockAnimationName = "PiggieBankFree";

		[SerializeField]
		private string freeOpeningStateAnimationName = "PiggieBankOneFrameFree";

		[SerializeField]
		private string paidOpeningStateAnimationName = "PiggieBankOneFrameFree";

		[SerializeField]
		private string lockedStateAnimationName = "PiggieBankOneframeLock";

		[SerializeField]
		private string tutorialStateAnimationName = "PiggieBankOneframeLock";

		private readonly Fiber showViewFiber = new Fiber();

		protected PiggyBankStateController stateController;

		private PiggyBankControllerFactory controllerFactory;

		private IPiggyBankRewards rewards;

		private PiggyBankStateController.PiggyBankState currentPiggyBankButtonState;

		private int coins;

		private int capacity;
	}
}
