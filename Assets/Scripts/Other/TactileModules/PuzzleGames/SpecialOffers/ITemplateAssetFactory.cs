using System;
using TactileModules.SpecialOffers.Views;

namespace TactileModules.PuzzleGames.SpecialOffers
{
	public interface ITemplateAssetFactory
	{
		ISpecialOfferView CreateSpecialOfferCoinView();

		ISpecialOfferView CreateSpecialOfferFreeView();

		ISpecialOfferIapView CreateSpecialOfferIAPView();

		ISpecialOfferRewardView CreateSpecialOfferRewardView();
	}
}
