using System;
using Tactile;
using UnityEngine;

public class GimmeItemSelectionItem : MonoBehaviour
{
	public InventoryItemMetaData CachedMetaData { get; set; }

	private void OnEnable()
	{
		this.CachedMetaData = InventoryManager.Instance.GetMetaData(this.ItemType);
		UISprite icon = this.RewardItem.icon;
		icon.SpriteName = this.CachedMetaData.IconSpriteName;
		icon.KeepAspect = true;
		icon.Atlas = UIProjectSettings.Get().defaultAtlas;
		this.UpdateCurrentAmount();
	}

	private void OnMoreClicked(UIEvent e)
	{
		this.currentAmount += this.stepAmount;
		this.UpdateCurrentAmount();
	}

	private void OnLessClicked(UIEvent e)
	{
		this.currentAmount -= this.stepAmount;
		this.UpdateCurrentAmount();
	}

	private void UpdateCurrentAmount()
	{
		this.RewardItem.label.text = this.CachedMetaData.FormattedQuantity(this.currentAmount);
	}

	public int ReturnAndResetCurrent()
	{
		int result = this.currentAmount;
		this.currentAmount = 0;
		this.UpdateCurrentAmount();
		return result;
	}

	[SerializeField]
	private RewardItem RewardItem;

	[SerializeField]
	private InventoryItem ItemType;

	[SerializeField]
	private int stepAmount;

	private int currentAmount;
}
