using System;
using System.Diagnostics;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class ShopViewItem : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<ShopViewItem> OnClick;

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private InAppPurchaseManager InAppPurchaseManager
	{
		get
		{
			return ManagerRepository.Get<InAppPurchaseManager>();
		}
	}

	private void Awake()
	{
	}

	private void OnDestroy()
	{
	}

	public void Initialize(ShopItem item, bool bestOffer)
	{
		this.mostPopularOfferSticker.gameObject.SetActive(false);
		bool flag = item.CustomTag.Contains("unlimitedLives") && this.ConfigurationManager.GetConfig<ShopConfig>().ShowInfiniteLifeRibbon;
		this.unlimitedLivesBanner.SetActive(flag);
		if (flag)
		{
			int hours = TimeSpan.FromSeconds((double)this.ConfigurationManager.GetConfig<LivesConfig>().InfiniteLivesDurationInSeconds).Hours;
			this.unlimitedLivesDurationLabel.text = hours + "h";
		}
		this.bestOfferSticker.SetActive(this.ConfigurationManager.GetConfig<ShopConfig>().ShowBestOfferSticker);
		this.shopItem = item;
		ShopItemMetaData metaData = ShopManager.Instance.GetMetaData<ShopItemMetaData>(item.Type);
		this.amount.text = this.shopItem.CoinAmount.ToString();
		this.price.text = this.shopItem.FormattedPricePreferIAP(this.InAppPurchaseManager);
		this.coinSprite.SpriteName = metaData.ImageSpriteName;
		this.GetElement().DoLayout();
		this.coinSprite.CorrectAspect(AspectCorrection.Fit);
		Vector3 localPosition = this.coinSprite.transform.localPosition;
		localPosition.x = -this.GetElementSize().x * 0.5f + this.coinSprite.Size.x * 0.5f;
		this.coinSprite.transform.localPosition = localPosition;
		if (item.CurrencyPrice <= 0)
		{
			this.saveTitle.gameObject.SetActive(false);
			this.saveAmount.gameObject.SetActive(false);
		}
		else
		{
			this.saveTitle.gameObject.SetActive(this.ConfigurationManager.GetConfig<ShopConfig>().ShowSaveAmountText);
			this.saveAmount.gameObject.SetActive(this.ConfigurationManager.GetConfig<ShopConfig>().ShowSaveAmountText);
			this.saveAmount.text = string.Format("{0}%", item.CurrencyPrice.ToString());
			this.bestOfferSticker.SetActive(bestOffer && this.ConfigurationManager.GetConfig<ShopConfig>().ShowBestOfferSticker);
		}
	}

	public void ShowMostPopularSticker()
	{
		this.mostPopularOfferSticker.gameObject.SetActive(true);
	}

	private void Clicked(UIEvent e)
	{
		if (this.OnClick != null)
		{
			this.OnClick(this);
		}
	}

	public ShopItem GetShopItem()
	{
		return this.shopItem;
	}

	public Vector3 GetCoinSpawnPos()
	{
		return this.coinSpawnPos.transform.position + Vector3.back * 40f;
	}

	public UILabel amount;

	public UILabel saveTitle;

	public UILabel saveAmount;

	public UILabel price;

	public UISprite coinSprite;

	public GameObject bestOfferSticker;

	public GameObject mostPopularOfferSticker;

	private ShopItem shopItem;

	public Transform coinSpawnPos;

	public GameObject unlimitedLivesBanner;

	public UILabel unlimitedLivesDurationLabel;
}
