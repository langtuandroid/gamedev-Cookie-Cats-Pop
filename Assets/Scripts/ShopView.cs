using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Fibers;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Ads;
using TactileModules.Foundation;
using UnityEngine;

public class ShopView : UIView
{
	
	protected override void ViewLoad(object[] parameters)
	{
		this.neededCoinsForPurchase = (int)parameters[0];
		this.spinnerButtonWatchAd = this.watchAdInstantiator.GetInstance<ButtonWithSpinner>();
		this.RequestVideo();
	}

	protected override void ViewWillAppear()
	{
		bool active = !true;
		this.anyPurchaseRemovesAdsLabel.gameObject.SetActive(active);
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
		this.spinnerButtonWatchAd.PrimaryTitle = L.Get("1 [C] Watch Ad!");
		this.spinnerButtonWatchAd.LoadingTitle = L.Get("Loading...");
		this.UpdateUI();
	}

	protected override void ViewWillDisappear()
	{
		this.hasBeenClosedOrIsClosing = true;
		this.animCoinsFiber.Terminate();
		this.requestingVideoFiber.Terminate();
		this.videoFiber.Terminate();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
	}

	protected override void ViewGotFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = delegate()
		{
			base.Close(0);
		};
	}

	protected override void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
	}

	private void UpdateUI()
	{
		this.UpdateCoinItemsUI();
	}

	private void RequestVideo()
	{
		
	}


	private void UpdateCoinItemsUI()
	{
		if (this.hasBeenClosedOrIsClosing)
		{
			return;
		}
		this.InitializeStaticList();
	}

	private void InitializeStaticList()
	{
		this.staticContentRoot.gameObject.SetActive(true);
		if (!this.isStaticListInitialized)
		{
			int num = 0;
			foreach (ShopItem shopItem in ShopManager.Instance.ShopItemsWithIAPForItems("Coin"))
			{
				if (shopItem.Type.Contains("CoinPack"))
				{
					num++;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.coinElementPrefab);
					gameObject.SetLayerRecursively(base.gameObject.layer);
					gameObject.SetActive(true);
					ShopViewItem component = gameObject.GetComponent<ShopViewItem>();
					component.Initialize(shopItem, false);
					component.OnClick += this.HandleCoinItemClicked;
					component.name = num.ToString();
					if (shopItem.CustomTag.Contains("popular"))
					{
						component.ShowMostPopularSticker();
					}
					component.transform.parent = this.staticGridLayout.transform;
					component.transform.localPosition = Vector3.zero;
				}
			}
			this.isStaticListInitialized = true;
		}
	}

	[UsedImplicitly]
	private void WatchAdClicked(UIEvent e)
	{
		
	}

	private void HandleCoinItemClicked(ShopViewItem item)
	{
		
	}


	

	private void AnimatePurchaseSuccess(int numCoins, Vector3 spawnPos)
	{
		this.animCoinsFiber.Start(this.AnimateCoins(numCoins, spawnPos));
	}

	private void EvaluateCoinsReceived()
	{
		if (this.neededCoinsForPurchase > 0 && InventoryManager.Instance.Coins >= this.neededCoinsForPurchase)
		{
			base.Close(0);
		}
	}

	private IEnumerator AnimateCoins(int numCoins, Vector3 spawnPos)
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.currencyOverlay.coinButton.PauseRefreshingCoins(false);
		});
		yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		this.currencyOverlay.coinAnimator.GiveCoins(spawnPos, numCoins, 0.5f, null, null);
		yield return FiberHelper.Wait(0.6f, (FiberHelper.WaitFlag)0);
		this.EvaluateCoinsReceived();
		yield break;
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private GameObject coinElementPrefab;

	[SerializeField]
	private UILabel anyPurchaseRemovesAdsLabel;

	[SerializeField]
	private UIInstantiator watchAdInstantiator;

	[SerializeField]
	private GameObject staticContentRoot;

	[SerializeField]
	private UIGridLayout staticGridLayout;

	private CurrencyOverlay currencyOverlay;

	private int neededCoinsForPurchase;

	private readonly Fiber animCoinsFiber = new Fiber();

	private readonly Fiber requestingVideoFiber = new Fiber();

	private readonly Fiber videoFiber = new Fiber();

	private ButtonWithSpinner spinnerButtonWatchAd;

	private bool isStaticListInitialized;

	private bool hasBeenClosedOrIsClosing;

	private string rewardedVideoPlacement = "Shop";

	[CompilerGenerated]
	private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;
}
