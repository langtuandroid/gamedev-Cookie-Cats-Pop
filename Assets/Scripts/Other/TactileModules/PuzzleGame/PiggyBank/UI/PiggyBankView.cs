using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using Spine;
using TactileModules.PuzzleGame.PiggyBank.Controllers;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public class PiggyBankView : ExtensibleView<IPiggyBankViewExtension>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OpenButtonClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action TutorialButtonClicked;



		public void Initialize(PiggyBankStateController.PiggyBankState state, IPiggyBankProgression progression, IPiggyBankRewards rewards, IIAPProvider iapProvider)
		{
			this.progression = progression;
			this.rewards = rewards;
			this.iapProvider = iapProvider;
			this.piggyBankViewText = base.GetComponent<IPiggyBankViewText>();
			if (base.Extension != null)
			{
				base.Extension.Initialize(this);
			}
			this.progressBar.Progress = rewards.CoinProgress;
			this.InitializeLabels();
			GameObject instance = this.closeButtonUIInstantiator.GetInstance();
			this.closeButton = instance.GetComponentInChildren<UIButton>();
			this.openButton = this.openButtonUIInstantiator.GetInstance<UIButton>();
			this.tutorialButton = this.tutorialButtonUIInstantiator.GetInstance<UIButton>();
			this.openButton.Clicked += this.OpenButtonClickedHandler;
			this.closeButton.Clicked += this.CloseButtonClickedHandler;
			this.tutorialButton.Clicked += this.TutorialButtonClickedHandler;
			this.ShowState(state);
		}

		private void InitializeLabels()
		{
			this.amountLabel.text = this.rewards.CollectedCoins + " / " + this.rewards.Capacity;
			this.capacityIncreaseLabel.text = "+" + this.rewards.AvailableCapacityIncrease;
			bool flag = this.piggyBankViewText != null;
			this.openNotAvailableFreeOpenLabel.text = ((!flag) ? this.GenericFreeOpenNotAvailableText(this.progression.GetNextFreeOpenLevelHumanNumber()) : this.piggyBankViewText.FreeOpenNotAvailableText(this.progression.GetNextFreeOpenLevelHumanNumber()));
			this.openNotAvailableCollectCoinsLabel.text = ((!flag) ? this.GenericCollectCoinsNotAvailableText(this.rewards.CoinsRequiredForPaidOpening) : this.piggyBankViewText.CollectCoinsNotAvailableText(this.rewards.CoinsRequiredForPaidOpening));
			this.openAvailableLabel.text = ((!flag) ? this.GenericNextFreeAvailableAtText(this.progression.GetNextFreeOpenLevelHumanNumber()) : this.piggyBankViewText.NextFreeAvailableAtText(this.progression.GetNextFreeOpenLevelHumanNumber()));
		}

		private void OpenButtonClickedHandler(UIButton uiButton)
		{
			this.DisableOpenButton(true);
			this.openButton.Clicked -= this.OpenButtonClickedHandler;
			this.OpenButtonClicked();
		}

		private void CloseButtonClickedHandler(UIButton uiButton)
		{
			base.Close(0);
		}

		private void TutorialButtonClickedHandler(UIButton uiButton)
		{
			this.TutorialButtonClicked();
		}

		private void DisableOpenButton(bool disable)
		{
			this.openButton.gameObject.SetActive(!disable);
			this.openButtonDisabledSprite.gameObject.SetActive(disable);
		}

		private void ShowState(PiggyBankStateController.PiggyBankState piggyBankState)
		{
			this.DisableAllStateGameObjects();
			if (piggyBankState != PiggyBankStateController.PiggyBankState.FreeOpening)
			{
				if (piggyBankState != PiggyBankStateController.PiggyBankState.PaidOpening)
				{
					if (piggyBankState == PiggyBankStateController.PiggyBankState.Locked)
					{
						this.ShowLockedState();
					}
				}
				else
				{
					this.ShowPaidOpeningState();
				}
			}
			else
			{
				this.ShowFreeOpeningState();
			}
		}

		private void DisableAllStateGameObjects()
		{
			this.openButton.gameObject.SetActive(false);
			this.openButtonDisabledSprite.gameObject.SetActive(false);
			this.openButtonOpenNotAvailableSprite.gameObject.SetActive(false);
			this.capacityIncreaseBadge.gameObject.SetActive(false);
			this.tutorialButton.gameObject.SetActive(false);
			this.openNotAvailableFreeOpenLabel.gameObject.SetActive(false);
			this.openNotAvailableCollectCoinsLabel.gameObject.SetActive(false);
			this.openAvailableLabel.gameObject.SetActive(false);
		}

		private void ShowFreeOpeningState()
		{
			this.SetOpenButtonLabelText(L.Get("Open for Free"));
			this.DisableOpenButton(false);
			this.closeButton.gameObject.SetActive(false);
			if (base.Extension != null)
			{
				Vector3 position = this.spineContainer.gameObject.transform.position;
				position.y += base.Extension.YOffSetFreeOpeningState;
				this.spineContainer.gameObject.transform.position = position;
				this.fiber.Start(base.Extension.PlayOpeningAnimation());
				base.Extension.PlayOpeningAvailableIdleAnimation();
			}
		}

		private void ShowPaidOpeningState()
		{
			this.SetOpenButtonLabelText(string.Format(L.Get("Open now {0}"), this.iapProvider.GetFormattedOpenPrice()));
			this.DisableOpenButton(false);
			this.closeButton.gameObject.SetActive(true);
			this.capacityIncreaseBadge.gameObject.SetActive(this.ShouldShowCapacityIncreaseBadge());
			this.openAvailableLabel.gameObject.SetActive(true);
			if (base.Extension != null)
			{
				base.Extension.PlayOpeningAvailableIdleAnimation();
			}
		}

		private void ShowLockedState()
		{
			this.openButtonOpenNotAvailableSprite.gameObject.SetActive(true);
			this.closeButton.gameObject.SetActive(true);
			this.tutorialButton.gameObject.SetActive(true);
			this.openNotAvailableFreeOpenLabel.gameObject.SetActive(true);
			this.openNotAvailableCollectCoinsLabel.gameObject.SetActive(true);
			if (base.Extension != null)
			{
				base.Extension.PlayLockedJumpingAnimation();
			}
		}

		private void SetOpenButtonLabelText(string labelText)
		{
			UILabel componentInChildren = this.openButton.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = labelText;
			}
			this.openButtonDisabledLabel.text = labelText;
		}

		private bool ShouldShowCapacityIncreaseBadge()
		{
			return this.rewards.Capacity < this.rewards.MaxCapacity;
		}

		public void PurchaseCancelled()
		{
			this.DisableOpenButton(false);
			this.openButton.Clicked += this.OpenButtonClickedHandler;
		}

		public IEnumerator ShowPiggyBankBought(int coinsToAnimate)
		{
			yield return this.AnimateEmptying(coinsToAnimate);
			yield break;
		}

		public IEnumerator ShowPiggyBankFreeOpened(int coinsToAnimate)
		{
			yield return this.AnimateEmptying(coinsToAnimate);
			yield break;
		}

		private IEnumerator AnimateEmptying(int amount)
		{
			yield return new Fiber.OnExit(delegate()
			{
				UICamera.EnableInput();
			});
			UICamera.DisableInput();
			float duration = this.EmptyPiggy();
			yield return FiberHelper.Wait(duration / 2f, (FiberHelper.WaitFlag)0);
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateCoinAmount(Mathf.Min(amount, 10), duration / 4f);
			}
			yield return FiberHelper.Wait(duration / 3f, (FiberHelper.WaitFlag)0);
			if (base.Extension != null)
			{
				base.Extension.PauseRefreshingCoins(false);
			}
			yield break;
		}

		private float EmptyPiggy()
		{
			if (base.Extension != null)
			{
				TrackEntry trackEntry = base.Extension.PlayEmptyBankAnimation();
				trackEntry.End += this.EmptyPiggyPlayed;
				return trackEntry.EndTime;
			}
			return 0f;
		}

		private void EmptyPiggyPlayed(Spine.AnimationState animationState, int trackIndex)
		{
			base.Close(0);
		}

		protected override void ViewWillDisappear()
		{
			base.ViewWillDisappear();
			this.openButton.Clicked -= this.OpenButtonClickedHandler;
			this.tutorialButton.Clicked -= this.TutorialButtonClickedHandler;
		}

		private void OnDestroy()
		{
			if (base.Extension != null)
			{
				base.Extension.PauseRefreshingCoins(false);
			}
			this.fiber.Terminate();
		}

		private string GenericFreeOpenNotAvailableText(int nextFreeOpenLevelHumanNumber)
		{
			return string.Format(L.Get("Free open at level {0}"), nextFreeOpenLevelHumanNumber);
		}

		private string GenericCollectCoinsNotAvailableText(int coinsRequiredForPaidOpening)
		{
			return string.Format(L.Get("Or collect {0} coins to buy open."), coinsRequiredForPaidOpening);
		}

		private string GenericNextFreeAvailableAtText(int nextFreeOpenLevelHumanNumber)
		{
			return string.Format(L.Get("Next free open at level {0}"), nextFreeOpenLevelHumanNumber);
		}

		[SerializeField]
		private UIInstantiator tutorialButtonUIInstantiator;

		[SerializeField]
		private UIInstantiator closeButtonUIInstantiator;

		[SerializeField]
		private UIInstantiator openButtonUIInstantiator;

		[SerializeField]
		private UISprite openButtonOpenNotAvailableSprite;

		[SerializeField]
		private UISprite openButtonDisabledSprite;

		[SerializeField]
		private UILabel openButtonDisabledLabel;

		[SerializeField]
		private UILabel amountLabel;

		[SerializeField]
		private UISprite capacityIncreaseBadge;

		[SerializeField]
		private UILabel capacityIncreaseLabel;

		[SerializeField]
		private UILabel openNotAvailableFreeOpenLabel;

		[SerializeField]
		private UILabel openNotAvailableCollectCoinsLabel;

		[SerializeField]
		private UILabel openAvailableLabel;

		[SerializeField]
		private GameObject spineContainer;

		[SerializeField]
		private UIProgressBar progressBar;

		private IPiggyBankProgression progression;

		private IPiggyBankRewards rewards;

		private IIAPProvider iapProvider;

		private UIButton tutorialButton;

		private UIButton closeButton;

		private UIButton openButton;

		private Fiber fiber = new Fiber();

		private IPiggyBankViewText piggyBankViewText;
	}
}
