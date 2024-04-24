using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class TicketShopView : ExtensibleView<TicketShopView.IExtension>
{
	protected override void ViewLoad(object[] parameters)
	{
		for (int i = 0; i < this.rankButtons.Count; i++)
		{
			this.rankButtons[i].GetInstance().GetComponent<ITournamentRankSelectButton>().Init(i + TournamentRank.Bronze);
		}
		if (parameters.Length > 0)
		{
			this.requiredRank = (TournamentRank)parameters[0];
			this.selectedRank = this.requiredRank;
			this.HandleRankSelected(this.selectedRank);
		}
	}

	protected override void ViewWillAppear()
	{
		this.UpdateUI();
	}

	private void OnDestroy()
	{
		this.purchaseFiber.Terminate();
	}

	private void UpdateUI()
	{
		this.UpdateOwnedTicketsItems();
		this.PopulateTicketList();
	}

	private void PopulateTicketList()
	{
		List<ShopItem> list = new List<ShopItem>();
		foreach (ShopItem shopItem in ShopManager.Instance.ShopItemsWithCustomTag("tickets"))
		{
			if (shopItem.PartialIAPIdentifier.Contains(this.selectedRank.ToString().ToLower()))
			{
				list.Add(shopItem);
			}
		}
		ShopItem shopItem2 = list[0];
		ShopItem shopItem3 = list[0];
		foreach (ShopItem shopItem4 in list)
		{
			if (shopItem4.CalculatePricePerItem() < shopItem2.CalculatePricePerItem())
			{
				shopItem2 = shopItem4;
			}
			if (shopItem4.CalculatePricePerItem() > shopItem3.CalculatePricePerItem())
			{
				shopItem3 = shopItem4;
			}
		}
		string discount = this.MakeNiceDiscountNumber(1f - shopItem2.CalculatePricePerItem() / shopItem3.CalculatePricePerItem());
		for (int i = 0; i < this.ticketTiers; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ticketElementPrefab);
			gameObject.transform.parent = this.ticketList.transform;
			gameObject.SetLayerRecursively(gameObject.transform.parent.gameObject.layer);
			this.ticketElements.Add(gameObject.GetComponent<TicketShopItem>());
			gameObject.GetComponent<TicketShopItem>().Init(list[i], list[i] == shopItem2, discount, this.selectedRank, i, new Action<TicketShopItem>(this.HandleTicketButtonClicked));
			gameObject.name = "Ticket" + i;
		}
	}

	private string MakeNiceDiscountNumber(float pctSaved)
	{
		int num = Mathf.FloorToInt(pctSaved * 100f);
		if (num != 33 || num != 66)
		{
			num = Mathf.RoundToInt((float)num / 5f) * 5;
		}
		return num.ToString();
	}

	private void UpdateTicketItemsUI()
	{
		if (this.ticketElements.Count > 0)
		{
			List<ShopItem> list = new List<ShopItem>();
			foreach (ShopItem shopItem in ShopManager.Instance.ShopItemsWithCustomTag("tickets"))
			{
				if (shopItem.PartialIAPIdentifier.ToLower().Contains(this.selectedRank.ToString().ToLower()))
				{
					list.Add(shopItem);
				}
			}
			ShopItem shopItem2 = list[0];
			ShopItem shopItem3 = list[0];
			foreach (ShopItem shopItem4 in list)
			{
				if (shopItem4.CalculatePricePerItem() < shopItem2.CalculatePricePerItem())
				{
					shopItem2 = shopItem4;
				}
				if (shopItem4.CalculatePricePerItem() > shopItem3.CalculatePricePerItem())
				{
					shopItem3 = shopItem4;
				}
			}
			string discount = this.MakeNiceDiscountNumber(shopItem2.CalculatePricePerItem() / shopItem3.CalculatePricePerItem());
			for (int i = 0; i < this.ticketElements.Count; i++)
			{
				this.ticketElements[i].Init(list[i], list[i] == shopItem2, discount, this.selectedRank, i, new Action<TicketShopItem>(this.HandleTicketButtonClicked));
			}
		}
	}

	private void UpdateOwnedTicketsItems()
	{
		this.ticketPanel.GetInstance<TicketPanel>().Init(this.selectedRank);
	}

	private void HandleRankSelected(TournamentRank rank)
	{
		foreach (UIInstantiator uiinstantiator in this.rankButtons)
		{
			ITournamentRankSelectButton component = uiinstantiator.GetInstance().GetComponent<ITournamentRankSelectButton>();
			component.Selected = (component.Rank == rank);
		}
		this.selectedRank = rank;
		this.ticketPanel.GetInstance<TicketPanel>().Init(rank);
		this.UpdateOwnedTicketsItems();
		this.UpdateTicketItemsUI();
	}

	private void HandleTicketButtonClicked(TicketShopItem item)
	{
		
	}



	private IEnumerator AnimateTicketOnPurchase(int amount, TournamentRank rank, Vector3 source)
	{
		yield return this.ticketPanel.GetInstance<TicketPanel>().AnimateTicket(rank, TicketPanel.AnimationDirection.IntoPanel, source);
		if (base.Extension != null)
		{
			base.Extension.AnimatedItemLandingInInventory();
		}
		yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	private void TournamentButtonClicked(UIEvent e)
	{
		TournamentRank rank = e.sender.GetElement().GetComponent<ITournamentRankSelectButton>().Rank;
		this.HandleRankSelected(rank);
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public UILabel anyPurchaseRemovesAdsLabel;

	public GameObject ticketElementPrefab;

	public UIGridLayout ticketList;

	public UIInstantiator ticketPanel;

	public List<UIInstantiator> rankButtons;

	private Fiber purchaseFiber = new Fiber();

	private int ticketTiers = 3;

	private List<TicketShopItem> ticketElements = new List<TicketShopItem>();

	private TournamentRank requiredRank;

	private TournamentRank selectedRank = TournamentRank.Bronze;

	public interface IExtension
	{
		void PurchaseSuccessful();

		void AnimatedItemLandingInInventory();
	}
}
