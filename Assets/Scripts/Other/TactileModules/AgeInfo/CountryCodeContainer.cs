using System;
using System.Collections.Generic;

namespace TactileModules.AgeInfo
{
	[ObsoleteJsonName(new string[]
	{
		"CountryCodes",
		"CountryAgeThresholds"
	})]
	public class CountryCodeContainer
	{
		[JsonSerializable("CountryConsentRequirements", typeof(CountryConsentRequirement))]
		public List<CountryConsentRequirement> CountryConsentRequirements { get; set; }
	}
}
