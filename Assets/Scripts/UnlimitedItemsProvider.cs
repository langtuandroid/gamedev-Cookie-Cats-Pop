using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.Inventory;

public class UnlimitedItemsProvider : IUnlimitedItemsProvider
{
	public UnlimitedItemsProvider()
	{
		this.unlimitedItems.Add("BoosterShield_Unlimited");
		this.unlimitedItems.Add("BoosterSuperAim_Unlimited");
		this.unlimitedItems.Add("BoosterSuperQueue_Unlimited");
		this.unlimitedItemToItemMapping.Add("BoosterShield_Unlimited", "BoosterShield");
		this.unlimitedItemToItemMapping.Add("BoosterSuperAim_Unlimited", "BoosterSuperAim");
		this.unlimitedItemToItemMapping.Add("BoosterSuperQueue_Unlimited", "BoosterSuperQueue");
	}

	public bool IsUnlimitedType(InventoryItem inventoryItem)
	{
		return this.unlimitedItems.Contains(inventoryItem);
	}

	public InventoryItem GetCorrespondingNonUnlimitedItem(InventoryItem unlimitedItem)
	{
		InventoryItem inventoryItem;
		this.unlimitedItemToItemMapping.TryGetValue(unlimitedItem, out inventoryItem);
		return (!(inventoryItem == null)) ? inventoryItem : unlimitedItem;
	}

	private readonly List<InventoryItem> unlimitedItems = new List<InventoryItem>();

	private readonly Dictionary<InventoryItem, InventoryItem> unlimitedItemToItemMapping = new Dictionary<InventoryItem, InventoryItem>();
}
