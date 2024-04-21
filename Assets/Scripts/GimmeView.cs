using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using UnityEngine;

public class GimmeView : UIView
{
	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	protected override void ViewWillAppear()
	{
		this.defaultItemGrid.gameObject.SetActive(true);
		this.boostersItemGrid.gameObject.SetActive(true);
		this.AllPossibleItems = base.GetComponentsInChildren<GimmeItemSelectionItem>();
		this.defaultItemGrid.gameObject.SetActive(true);
		this.boostersItemGrid.gameObject.SetActive(false);
	}

	private void ChangeToDefaultCategory(UIEvent e)
	{
		this.defaultItemGrid.gameObject.SetActive(true);
		this.boostersItemGrid.gameObject.SetActive(false);
	}

	private void ChangeToBoosterCategory(UIEvent e)
	{
		this.defaultItemGrid.gameObject.SetActive(false);
		this.boostersItemGrid.gameObject.SetActive(true);
	}

	private void AddSelectedToCart(UIEvent e)
	{
		List<ItemAmount> list = new List<ItemAmount>();
		for (int i = 0; i < this.AllPossibleItems.Length; i++)
		{
			GimmeItemSelectionItem gimmeItemSelectionItem = this.AllPossibleItems[i];
			int num = gimmeItemSelectionItem.ReturnAndResetCurrent();
			if (num != 0)
			{
				ItemAmount item = new ItemAmount
				{
					Amount = num,
					ItemId = gimmeItemSelectionItem.CachedMetaData.Id
				};
				this.currentItemsCart.Add(item);
				list.Add(item);
			}
		}
		this.rewardGrid.Initialize(list, false);
	}

	private void ClaimCart(UIEvent e)
	{
		foreach (ItemAmount itemAmount in this.currentItemsCart)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "cheat");
		}
		this.animFiber.Start(this.Animate());
	}

	protected override void ViewDidAppear()
	{
	}

	private IEnumerator Animate()
	{
		yield return this.rewardGrid.Animate(false, true);
		this.Internal_EmptyCart();
		yield break;
	}

	private void EmptyCart(UIEvent e)
	{
		this.Internal_EmptyCart();
	}

	private void Internal_EmptyCart()
	{
		this.rewardGrid.EmptySlots();
		this.currentItemsCart.Clear();
	}

	[SerializeField]
	private GameObject DefaultCategoryButton;

	[SerializeField]
	private GameObject BoosterCategoryButton;

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private UIGridLayout defaultItemGrid;

	[SerializeField]
	private UIGridLayout boostersItemGrid;

	private List<ItemAmount> currentItemsCart = new List<ItemAmount>();

	private GimmeItemSelectionItem[] AllPossibleItems;

	private Fiber animFiber = new Fiber();
}
