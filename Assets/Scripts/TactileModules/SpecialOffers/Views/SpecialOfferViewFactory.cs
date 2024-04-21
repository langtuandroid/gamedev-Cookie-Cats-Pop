using System;
using TactileModules.PuzzleGames.SpecialOffers;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Views
{
	internal class SpecialOfferViewFactory : ISpecialOfferViewFactory
	{
		public SpecialOfferViewFactory(ITemplateAssetFactory templateAssetFactory)
		{
			this.templateAssetFactory = templateAssetFactory;
		}

		public ISpecialOfferIapView CreateIAPView(ISpecialOffer offer)
		{
			ISpecialOfferIapView specialOfferIapView = this.templateAssetFactory.CreateSpecialOfferIAPView();
			specialOfferIapView.gameObject.name = "SpecialOfferIapView_" + offer.GetAnalyticsId();
			return specialOfferIapView;
		}

		public ISpecialOfferView CreateCoinView(ISpecialOffer offer)
		{
			ISpecialOfferView specialOfferView = this.templateAssetFactory.CreateSpecialOfferCoinView();
			specialOfferView.gameObject.name = "SpecialOfferCoinView_" + offer.GetAnalyticsId();
			return specialOfferView;
		}

		public ISpecialOfferView CreateFreeView(ISpecialOffer offer)
		{
			ISpecialOfferView specialOfferView = this.templateAssetFactory.CreateSpecialOfferFreeView();
			specialOfferView.gameObject.name = "SpecialOfferFreeView_" + offer.GetAnalyticsId();
			return specialOfferView;
		}

		public ISpecialOfferRewardView CreateRewardView(ISpecialOffer offer)
		{
			ISpecialOfferRewardView specialOfferRewardView = this.templateAssetFactory.CreateSpecialOfferRewardView();
			specialOfferRewardView.gameObject.name = "SpecialOfferRewardView_" + offer.GetAnalyticsId();
			specialOfferRewardView.Initialize(offer.GetReward());
			return specialOfferRewardView;
		}

		private readonly ITemplateAssetFactory templateAssetFactory;
	}
}
