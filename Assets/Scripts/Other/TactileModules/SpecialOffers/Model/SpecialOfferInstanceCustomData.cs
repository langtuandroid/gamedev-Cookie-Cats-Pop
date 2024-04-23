using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOfferInstanceCustomData : FeatureInstanceCustomData
	{
		[JsonSerializable("DidShowTimeStamp", null)]
		public int DidShowTimeStamp { get; set; }
	}
}
