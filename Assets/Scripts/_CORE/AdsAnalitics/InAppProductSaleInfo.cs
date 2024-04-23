using System;

public class InAppProductSaleInfo
{
	[JsonSerializable("Amount", null)]
	public int Amount { get; set; }

	[JsonSerializable("PercentageOff", null)]
	public int PercentageOff { get; set; }
}
