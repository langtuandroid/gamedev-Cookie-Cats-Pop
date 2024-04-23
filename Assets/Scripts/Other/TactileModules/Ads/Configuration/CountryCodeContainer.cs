using System;
using System.Collections.Generic;

namespace TactileModules.Ads.Configuration
{
	public class CountryCodeContainer
	{
		[JsonSerializable("CountryCodes", typeof(string))]
		public List<string> CountryCodes { get; set; }
	}
}
