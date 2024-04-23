using System;
using Tactile;
using UnityEngine;

public class DailyQuestRewardRow : MonoBehaviour
{
	public void SetByRewardEntry(ItemAmount entry)
	{
		this.label.text = entry.Amount.ToString();
		this.icon.SpriteName = InventoryManager.Instance.GetMetaData<InventoryItemMetaData>(entry.ItemId).IconSpriteName;
	}

	public UILabel label;

	public UISprite icon;
}
