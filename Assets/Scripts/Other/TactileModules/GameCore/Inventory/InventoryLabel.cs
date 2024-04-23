using System;
using UnityEngine;

namespace TactileModules.GameCore.Inventory
{
	public class InventoryLabel : MonoBehaviour
	{
		public Action AmountChanged { get; set; }

		public int BaseValue { get; set; }

		public void StartListen(IVisualInventory visualInventory)
		{
			this.visualInventory = visualInventory;
			visualInventory.VisualInventoryChanged += this.InventoryChanged;
			this.UpdateLabel();
		}

		public void StopListen()
		{
			if (this.visualInventory != null)
			{
				this.visualInventory.VisualInventoryChanged -= this.InventoryChanged;
			}
		}

		private void InventoryChanged(InventoryItem item)
		{
			if (item != this.inventoryType)
			{
				return;
			}
			this.UpdateLabel();
		}

		private void UpdateLabel()
		{
			int visualAmount = this.visualInventory.GetVisualAmount(this.inventoryType);
			this.SetAmount(visualAmount);
		}

		private void SetAmount(int amount)
		{
			amount -= this.BaseValue;
			this.label.text = L.FormatNumber(amount);
			if (this.AmountChanged != null)
			{
				this.AmountChanged();
			}
		}

		public void SetBaseValueToCurrent()
		{
			this.BaseValue = this.visualInventory.GetVisualAmount(this.inventoryType);
			this.UpdateLabel();
		}

		[SerializeField]
		private UILabel label;

		public InventoryItem inventoryType;

		private IVisualInventory visualInventory;
	}
}
