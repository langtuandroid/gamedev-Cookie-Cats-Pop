using System;
using System.Collections;
using Fibers;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Controllers
{
	public abstract class SpecialOfferViewController : ISpecialOfferViewController
	{
		public abstract IEnumerator ShowView(EnumeratorResult<PurchaseData> purchaseDataResult);
	}
}
