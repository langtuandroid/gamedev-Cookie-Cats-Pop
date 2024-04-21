using System;

public class InAppProduct
{
	public InAppProduct(InAppProductTactileInfo tactileInfo)
	{
		this.tactileInfo = tactileInfo;
	}

	public InAppProductTactileInfo TactileInfo
	{
		get
		{
			return this.tactileInfo;
		}
		set
		{
			this.tactileInfo = value;
		}
	}

	public InAppProductStoreInfo StoreInfo
	{
		get
		{
			return this.storeInfo;
		}
		set
		{
			this.storeInfo = value;
		}
	}

	public string FormattedPrice
	{
		get
		{
			return (this.storeInfo != null && !string.IsNullOrEmpty(this.storeInfo.formattedPrice)) ? this.storeInfo.formattedPrice : this.tactileInfo.FormattedPrice;
		}
	}

	public string Identifier
	{
		get
		{
			return (this.storeInfo != null && !string.IsNullOrEmpty(this.storeInfo.identifier)) ? this.storeInfo.identifier : this.tactileInfo.FullIdentifier;
		}
	}

	public string Title
	{
		get
		{
			return (this.storeInfo != null && !string.IsNullOrEmpty(this.storeInfo.title)) ? this.storeInfo.title : this.tactileInfo.Title;
		}
	}

	public string Description
	{
		get
		{
			return this.tactileInfo.Description;
		}
	}

	public int Amount
	{
		get
		{
			return this.tactileInfo.RealAmount;
		}
	}

	public bool Consumable
	{
		get
		{
			return string.IsNullOrEmpty(this.tactileInfo.ProductType) || this.tactileInfo.ProductType == "Consumable";
		}
	}

	public string ProductType
	{
		get
		{
			return this.tactileInfo.ProductType;
		}
	}

	public override string ToString()
	{
		return this.Title;
	}

	private InAppProductStoreInfo storeInfo;

	private InAppProductTactileInfo tactileInfo;
}
