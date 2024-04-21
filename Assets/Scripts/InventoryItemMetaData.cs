using System;
using UnityEngine;

public class InventoryItemMetaData : ScriptableObject
{
	public InventoryItem Id
	{
		get
		{
			return base.name;
		}
	}

	public string Title
	{
		get
		{
			return this.title;
		}
	}

	public string Description
	{
		get
		{
			return this.description;
		}
	}

	public string IconSpriteName
	{
		get
		{
			return this.iconSpriteName;
		}
	}

	public string FormattedQuantity(int amount)
	{
		if (amount == 0)
		{
			return string.Empty;
		}
		InventoryItemMetaData.QuantityFormat quantityFormat = this.quantityFormat;
		if (quantityFormat != InventoryItemMetaData.QuantityFormat.Seconds)
		{
			return string.Format(L.Get("x{0}"), amount);
		}
		if (amount < 60)
		{
			return string.Format(L.Get("{0}s"), amount);
		}
		if (amount < 3600)
		{
			return string.Format(L.Get("{0}m"), amount / 60);
		}
		return string.Format(L.Get("{0}h"), amount / 3600);
	}

	public const string META_ASSET_FOLDER = "Assets/[Inventory]/Resources/InventoryMetaData";

	[SerializeField]
	protected string title;

	[Multiline]
	[SerializeField]
	protected string description;

	[UISpriteName]
	[SerializeField]
	protected string iconSpriteName;

	[SerializeField]
	protected InventoryItemMetaData.QuantityFormat quantityFormat;

	public enum QuantityFormat
	{
		Units,
		Seconds
	}
}
