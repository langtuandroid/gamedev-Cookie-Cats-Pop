using System;
using System.Collections.Generic;

namespace TactileModules.SpecialOffers.Model
{
	public interface IAvailableSpecialOffers
	{
		List<ISpecialOffer> GetOffers();
	}
}
