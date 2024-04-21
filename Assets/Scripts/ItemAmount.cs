using System;
using ConfigSchema;

public sealed class ItemAmount
{
	[IgnoreProperty]
	[Obsolete("No longer used, only used by old clients")]
	[JsonSerializable("ItemId", null)]
	private string _itemId
	{
		get
		{
			return this.ItemId;
		}
		set
		{
			this.ItemId = value;
		}
	}

	[Required]
	[InventoryItems]
	[JsonSerializable("Type", null)]
	public string ItemId { get; set; }

	[Required]
	[JsonSerializable("Amount", null)]
	public int Amount { get; set; }
}
