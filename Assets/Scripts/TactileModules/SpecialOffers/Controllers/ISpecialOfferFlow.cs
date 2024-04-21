using System;
using System.Collections;

namespace TactileModules.SpecialOffers.Controllers
{
	public interface ISpecialOfferFlow
	{
		IEnumerator Run();
	}
}
