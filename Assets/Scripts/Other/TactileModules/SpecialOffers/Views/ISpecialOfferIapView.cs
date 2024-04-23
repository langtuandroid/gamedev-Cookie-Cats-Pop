using System;

namespace TactileModules.SpecialOffers.Views
{
	public interface ISpecialOfferIapView : ISpecialOfferView, IUIView
	{
		void SetPriceNow(string price);

		void SetPriceBefore(string price);
	}
}
