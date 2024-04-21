using System;
using System.Collections;
using Fibers;
using Tactile;
using TactileModules.Ads;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

public class NoMoreLivesView : UIView
{
	private IRewardedVideoPresenter RewardedVideoPresenter
	{
		get
		{
			return ManagerRepository.Get<RewardedVideoPresenter>();
		}
	}

	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	private void OnDestroy()
	{
		this.watchFiber.Terminate();
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.playingTournament = (parameters.Length > 0 && (bool)parameters[0]);
		this.rewardItem = ((!this.playingTournament) ? "Life" : "TournamentLife");
		this.RewardedVideoPresenter.RequestVideo();
		bool active = this.RewardedVideoPresenter.IsPlacementEnabled(this.rewardedVideoPlacement);
		this.watchVideoPivot.gameObject.SetActive(active);
	}

	protected override void ViewWillAppear()
	{
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
		if (this.playingTournament)
		{
			this.buyButtonHeart.SpriteName = "heartTournament";
		}
		this.livesOverlay = base.ObtainOverlay<LivesOverlay>();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
		if (this.livesOverlay != null)
		{
			base.ReleaseOverlay<LivesOverlay>();
		}
	}

	protected override void ViewGotFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = delegate()
		{
			UIViewManager.Instance.ShowView<ShopView>(new object[]
			{
				0
			});
		};
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = delegate()
			{
				base.Close(0);
			};
		}
		this.UpdateFacebookButton();
		this.buyPriceLabel.text = ShopManager.Instance.GetShopItem("ShopItemExtraLives").CurrencyPrice.ToString();
	}

	protected override void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = null;
		}
	}

	private void UpdateFacebookButton()
	{
		this.askFriendsButton.SetActive(this.FacebookLoginManager.IsLoggedInAndUserRegistered || this.playingTournament);
		this.connectNormal.SetActive(!this.FacebookLoginManager.IsLoggedInAndUserRegistered && !this.playingTournament);
	}

	private void Update()
	{
		int num = (!this.playingTournament) ? LivesManager.Instance.GetRegenerationTimeLeft() : TournamentManager.Instance.GetSecondsUntilLifeHasRegenerated();
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)num);
		this.timerLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		this.UpdateWatchVideoButton();
	}

	private void UpdateWatchVideoButton()
	{
		if (this.dontUpdateWatchVideoButtonAnyMore)
		{
			return;
		}
		if (this.RewardedVideoPresenter.IsInCooldown(this.rewardedVideoPlacement))
		{
			this.watchVideoButton.SetActive(false);
			this.watchVideoTimer.SetActive(true);
			this.watchVideoTimerLabel.text = this.RewardedVideoPresenter.GetTimeLeftString(this.rewardedVideoPlacement);
		}
		else
		{
			this.watchVideoTimer.SetActive(false);
			this.watchVideoButton.SetActive(true);
			this.dontUpdateWatchVideoButtonAnyMore = true;
		}
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void ButtonConnectToFacebook(UIEvent e)
	{
		UIViewManager.Instance.ShowView<FacebookLoginInfoView>(new object[]
		{
			LoginContext.Lives
		});
	}

	private void ButtonAskFriendsClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<FacebookSelectFriendsAndRequestView>(new object[]
		{
			(!this.playingTournament) ? FacebookSelectFriendsAndRequestView.RequestType.Life : FacebookSelectFriendsAndRequestView.RequestType.TournamentLife,
			true
		});
	}

	private void BuyLives(UIEvent e)
	{
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemExtraLives");
		int tmpLives = (!this.playingTournament) ? InventoryManager.Instance.Lives : TournamentManager.Instance.Lives;
		Vector3 purchasePos = this.buyPriceButtonPivot.transform.position + Vector3.back * 10f;
		ShopManager.Instance.TrySpendCoins(shopItem, this.buyPriceButtonPivot, delegate(bool succes)
		{
			if (succes)
			{
				UICamera.DisableInput();
				if (this.livesOverlay != null)
				{
					int visibleLives = (!this.playingTournament) ? (InventoryManager.Instance.Lives - tmpLives) : (TournamentManager.Instance.Lives - tmpLives);
					this.livesOverlay.GiveLife(purchasePos, visibleLives, this.playingTournament, 1f, delegate
					{
						this.livesOverlay.Refresh();
						UICamera.EnableInput();
						this.Close(1);
					});
				}
				else
				{
					UICamera.EnableInput();
					this.Close(1);
				}
			}
		});
	}

	private void WatchVideoGetFreeLives(UIEvent e)
	{
		this.watchFiber.Start(this.WatchVideoGetFreeLivesCr());
	}

	private IEnumerator WatchVideoGetFreeLivesCr()
	{
		RewardedVideoParameters videoParameters = new RewardedVideoParameters(this.rewardedVideoPlacement, this.rewardItem, 1);
		yield return this.RewardedVideoPresenter.ShowRewardedVideo(videoParameters, delegate(bool didComplete)
		{
			if (didComplete)
			{
				this.WatchedVideoSuccessful();
			}
		});
		yield break;
	}

	private void WatchedVideoSuccessful()
	{
		InventoryManager.Instance.Add(this.rewardItem, 1, "WatchedVideo");
		if (this.livesOverlay == null)
		{
			this.livesOverlay = base.ObtainOverlay<LivesOverlay>();
			if (this.livesOverlay == null)
			{
				return;
			}
		}
		if (this.watchVideoButton == null || this.watchVideoButton.Equals(null))
		{
			return;
		}
		Vector3 position = this.watchVideoButton.transform.position + Vector3.back * 10f;
		this.livesOverlay.GiveLife(position, 1, false, 1f, delegate
		{
			if (this != null)
			{
				base.Close(1);
			}
		});
		this.livesOverlay.Refresh();
	}

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private GameObject watchVideoPivot;

	[SerializeField]
	private GameObject watchVideoButton;

	[SerializeField]
	private GameObject watchVideoTimer;

	[SerializeField]
	private UILabel watchVideoTimerLabel;

	[SerializeField]
	private GameObject askFriendsButton;

	[SerializeField]
	private GameObject connectNormal;

	[SerializeField]
	private GameObject buyPriceButtonPivot;

	[SerializeField]
	private UILabel buyPriceLabel;

	[SerializeField]
	private UISprite buyButtonHeart;

	private bool dontUpdateWatchVideoButtonAnyMore;

	private LivesOverlay livesOverlay;

	private CurrencyOverlay currencyOverlay;

	private bool playingTournament;

	private string rewardItem;

	private Fiber watchFiber = new Fiber();

	private string rewardedVideoPlacement = "OutOfLives";
}
