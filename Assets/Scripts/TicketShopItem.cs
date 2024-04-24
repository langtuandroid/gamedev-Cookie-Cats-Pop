using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class TicketShopItem : MonoBehaviour
{
	public void Init(ShopItem shopItem, bool bestOffer, string discount, TournamentRank rank, int tier, Action<TicketShopItem> callback)
	{
		this.bestOfferSticker.SetActive(false);
		this.ticketStaks.ForEach(delegate(GameObject i)
		{
			i.SetActive(false);
		});
		this.shopItem = shopItem;
		this.rank = rank;
		this.tier = tier;
		this.ticketIcon.SpriteName = "Honeyhex" + (int)rank;
		this.amountOfTickets.text = shopItem.Rewards[0].Amount.ToString();
		this.price.text = "0";
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(rank);
		UIFontStyle fontStyle = rankSetup.fontStyle;
		if (fontStyle)
		{
			this.amountOfTickets.fontStyle = fontStyle;
		}
		if (!bestOffer)
		{
			this.saveTitle.gameObject.SetActive(false);
			this.saveAmount.gameObject.SetActive(false);
		}
		else
		{
			this.saveTitle.gameObject.SetActive(true);
			this.saveAmount.gameObject.SetActive(true);
			this.saveAmount.text = string.Format("{0}%", discount);
			this.bestOfferSticker.SetActive(bestOffer);
		}
		this.purchaseCallback = callback;
		this.ticketStaks[this.tier].SetActive(true);
		InventoryItemMetaData metaData = InventoryManager.Instance.GetMetaData(rankSetup.ticketItem);
		foreach (UISprite uisprite in this.ticketStaks[this.tier].GetComponentsInChildren<UISprite>())
		{
			uisprite.SpriteName = metaData.IconSpriteName;
		}
	}

	private void ButtonClicked(UIEvent e)
	{
		this.purchaseCallback(this);
	}

	public ShopItem GetShopItem()
	{
		return this.shopItem;
	}

	public Vector3 GetTicketSpawnPos()
	{
		return this.ticketIcon.transform.position;
	}

	public UILabel amountOfTickets;

	public UISprite ticketIcon;

	public UILabel saveTitle;

	public UILabel saveAmount;

	public UILabel price;

	public GameObject bestOfferSticker;

	public List<GameObject> ticketStaks;

	public TournamentRank rank;

	private int tier;

	private ShopItem shopItem;

	private Action<TicketShopItem> purchaseCallback;
}
