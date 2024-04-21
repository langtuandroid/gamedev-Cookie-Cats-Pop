using System;
using TactileModules.SpecialOffers.Views;
using UnityEngine;

namespace TactileModules.PuzzleGames.SpecialOffers
{
	public class TemplateAssetFactory : ITemplateAssetFactory
	{
		public ISpecialOfferView CreateSpecialOfferCoinView()
		{
			SpecialOfferView original = Resources.Load<SpecialOfferView>("SpecialOffers/SpecialOfferCoinView");
			return UnityEngine.Object.Instantiate<SpecialOfferView>(original);
		}

		public ISpecialOfferView CreateSpecialOfferFreeView()
		{
			SpecialOfferView original = Resources.Load<SpecialOfferView>("SpecialOffers/SpecialOfferFreeView");
			return UnityEngine.Object.Instantiate<SpecialOfferView>(original);
		}

		public ISpecialOfferIapView CreateSpecialOfferIAPView()
		{
			SpecialOfferIapView original = Resources.Load<SpecialOfferIapView>("SpecialOffers/SpecialOfferIAPView");
			return UnityEngine.Object.Instantiate<SpecialOfferIapView>(original);
		}

		public ISpecialOfferRewardView CreateSpecialOfferRewardView()
		{
			SpecialOfferRewardView original = Resources.Load<SpecialOfferRewardView>("SpecialOffers/SpecialOfferRewardView");
			return UnityEngine.Object.Instantiate<SpecialOfferRewardView>(original);
		}
	}
}
