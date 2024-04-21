using System;
using Tactile;

namespace TactileModules.AgeInfo
{
	[ConfigProvider("AgeInfoConfig")]
	[ObsoleteJsonName("ageThreshold")]
	public class AgeInfoConfig
	{
		[JsonSerializable("isActive", null)]
		public bool IsActive { get; set; }

		[JsonSerializable("countryFilter", typeof(string))]
		public CountryCodeContainer CountryFilter { get; set; }

		[JsonSerializable("termsOfServiceURL", null)]
		public string TermsOfServiceURL { get; set; }

		[JsonSerializable("privacyPolicyURL", null)]
		public string PrivacyPolicyURL { get; set; }
	}
}
