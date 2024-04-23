using System;
using Tactile;
using UnityEngine;

public class BonusDropMapButton : SideMapButton
{
	private void OnEnable()
	{
		InventoryManager.Instance.InventoryChanged += this.InventoryManager_InventoryChanged;
		this.UpdateLabel();
	}

	private void OnDisable()
	{
		InventoryManager.Instance.InventoryChanged -= this.InventoryManager_InventoryChanged;
	}

	private void InventoryManager_InventoryChanged(InventoryManager.ItemChangeInfo info)
	{
		if (info.Item == "BonusDrop")
		{
			this.UpdateLabel();
		}
	}

	private void UpdateLabel()
	{
		this.label.text = string.Format("{0}/{1}", Singleton<BonusDropManager>.Instance.DropsCollected, Singleton<BonusDropManager>.Instance.DropsRequiredForPrize);
	}

	private new void Clicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<BonusDropInfoView>(new object[0]);
	}

	public override bool VisibilityChecker(object data)
	{
		return Singleton<BonusDropManager>.Instance.ArePresentsEnabled;
	}

	public override SideMapButton.AreaSide Side
	{
		get
		{
			return SideMapButton.AreaSide.Left;
		}
	}

	public override Vector2 Size
	{
		get
		{
			return Vector2.zero;
		}
	}

	public override object Data
	{
		get
		{
			return null;
		}
	}

	public UILabel label;
}
