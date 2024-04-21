using System;
using Tactile;
using UnityEngine;

public class TournamentReward : MonoBehaviour
{
	public void Init(ItemAmount itemAmount)
	{
		this.amount.text = ((itemAmount.Amount <= 1) ? string.Empty : ("x" + itemAmount.Amount.ToString()));
		InventoryItemMetaData metaData = InventoryManager.Instance.GetMetaData(itemAmount.ItemId);
		this.icon.SpriteName = metaData.IconSpriteName;
	}

	public UILabel amount;

	public UISprite icon;
}
