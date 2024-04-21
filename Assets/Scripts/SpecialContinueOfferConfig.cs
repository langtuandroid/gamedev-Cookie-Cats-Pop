using System;
using Tactile;

[ConfigProvider("SpecialContinueOfferConfig")]
public class SpecialContinueOfferConfig
{
	[JsonSerializable("NeedToFailXTimes", null)]
	public int NeedToFailXTimes { get; set; }

	[JsonSerializable("Probability", null)]
	public int Probability { get; set; }
}
