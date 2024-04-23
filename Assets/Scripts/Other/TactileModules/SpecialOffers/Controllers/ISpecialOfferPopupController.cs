using System;
using System.Collections;

namespace TactileModules.SpecialOffers.Controllers
{
	public interface ISpecialOfferPopupController
	{
		IEnumerator Run(IViewPresenter viewPresenter);
	}
}
