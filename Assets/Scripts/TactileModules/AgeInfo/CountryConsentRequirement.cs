using System;

namespace TactileModules.AgeInfo
{
	public class CountryConsentRequirement
	{
		[JsonSerializable("CountryCode", null)]
		public string CountryCode { get; set; }

		[JsonSerializable("RequireAge", null)]
		public bool RequireAge { get; set; }

		[JsonSerializable("AgeThreshold", null)]
		public int AgeThreshold { get; set; }
	}
}
