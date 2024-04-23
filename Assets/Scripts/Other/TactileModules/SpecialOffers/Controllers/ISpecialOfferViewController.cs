using System;
using System.Collections;
using Fibers;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Controllers
{
	public interface ISpecialOfferViewController
	{
		IEnumerator ShowView(EnumeratorResult<PurchaseData> purchaseDataResult);
	}
}
