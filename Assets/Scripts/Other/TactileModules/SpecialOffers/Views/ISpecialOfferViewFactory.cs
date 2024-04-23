using System;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Views
{
	public interface ISpecialOfferViewFactory
	{
		ISpecialOfferIapView CreateIAPView(ISpecialOffer offer);

		ISpecialOfferView CreateCoinView(ISpecialOffer offer);

		ISpecialOfferView CreateFreeView(ISpecialOffer offer);

		ISpecialOfferRewardView CreateRewardView(ISpecialOffer offer);
	}
}
