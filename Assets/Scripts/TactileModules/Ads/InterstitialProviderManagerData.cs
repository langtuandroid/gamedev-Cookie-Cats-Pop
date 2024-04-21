using System;

namespace TactileModules.Ads
{
	public class InterstitialProviderManagerData
	{
		[JsonSerializable("LastInterstitialShown", typeof(DateTime))]
		public DateTime LastInterstitialShown { get; set; }

		[JsonSerializable("TotalInterstitialsShown", null)]
		public int TotalInterstitialsShown { get; set; }
	}
}
