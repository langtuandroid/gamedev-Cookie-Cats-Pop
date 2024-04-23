using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOfferCustomData : FeatureTypeCustomData
	{
		[JsonSerializable("gct", null)]
		public int GlobalCooldownTimestamp { get; set; }
	}
}
