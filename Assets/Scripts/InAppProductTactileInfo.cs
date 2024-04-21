using System;
using ConfigSchema;

[ObsoleteJsonName(new string[]
{
	"Type",
	"TypeId",
	"Consumable"
})]
public sealed class InAppProductTactileInfo
{
	[JsonSerializable("Identifier", null)]
	public string Identifier { private get; set; }

	[JsonSerializable("Title", null)]
	public string Title { get; set; }

	[JsonSerializable("Description", null)]
	public string Description { get; set; }

	[JsonSerializable("Price", null)]
	public double Price { get; set; }

	[JsonSerializable("CurrencyCode", null)]
	public string CurrencyCode { get; set; }

	[JsonSerializable("FormattedPrice", null)]
	public string FormattedPrice { get; set; }

	[JsonSerializable("Amount", null)]
	public int Amount { get; set; }

	[StringEnum(new string[]
	{
		"Consumable",
		"Non-Consumable",
		"Auto-Renewable Subscription"
	})]
	[JsonSerializable("ProductType", null)]
	public string ProductType { get; set; }

	[JsonSerializable("OnSale", null)]
	public InAppProductSaleInfo OnSale { get; set; }

	public int RealAmount
	{
		get
		{
			if (this.OnSale == null)
			{
				return this.Amount;
			}
			return this.OnSale.Amount;
		}
	}

	public static string IdentifierPrefix
	{
		get
		{
			return SystemInfoHelper.BundleIdentifier + ".iap.";
		}
	}

	public string PartialIdentifier
	{
		get
		{
			return this.Identifier;
		}
	}

	public string FullIdentifier
	{
		get
		{
			if (!string.IsNullOrEmpty(this.Identifier))
			{
				return InAppProductTactileInfo.IdentifierPrefix + this.Identifier;
			}
			return this.Identifier;
		}
	}

	public override string ToString()
	{
		return this.Title;
	}

	public const string CONSUMABLE = "Consumable";

	public const string NON_CONSUMABLE = "Non-Consumable";

	public const string AUTO_RENEWAL_SUBSCRIPTION = "Auto-Renewable Subscription";
}
