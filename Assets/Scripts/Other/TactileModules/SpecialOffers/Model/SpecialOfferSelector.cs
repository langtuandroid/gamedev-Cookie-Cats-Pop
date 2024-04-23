using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.SpecialOffers.Analytics;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOfferSelector : ISpecialOfferSelector
	{
		public SpecialOfferSelector(ISpecialOffersMainProgressionProvider mainProgression, IAvailableSpecialOffers availableSpecialOffers, ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, IAnalyticsReporter analyticsReporter)
		{
			this.mainProgression = mainProgression;
			this.availableSpecialOffers = availableSpecialOffers;
			this.specialOffersGlobalCoolDown = specialOffersGlobalCoolDown;
			this.analyticsReporter = analyticsReporter;
		}

		public ISpecialOffer GetOffer()
		{
			List<ISpecialOffer> offers = this.availableSpecialOffers.GetOffers();
			ISpecialOffer activatableOfferToShow = this.GetActivatableOfferToShow(offers);
			if (activatableOfferToShow != null)
			{
				return activatableOfferToShow;
			}
			ISpecialOffer activatedOfferToShow = this.GetActivatedOfferToShow(offers);
			bool flag = this.specialOffersGlobalCoolDown.IsCoolingDown();
			if (!flag && activatedOfferToShow != null)
			{
				return activatedOfferToShow;
			}
			this.LogNoOfferAvailableEvent(offers, flag, activatableOfferToShow, activatedOfferToShow);
			return null;
		}

		private void LogNoOfferAvailableEvent(List<ISpecialOffer> offers, bool cooldown, ISpecialOffer activatableOffer, ISpecialOffer activatedOffer)
		{
			string noOfferAvailableReason = string.Join(",", new List<string>
			{
				(!cooldown) ? "no cooldown" : "coolDown",
				(activatableOffer != null) ? "activatableOffer" : "no activatableOffer",
				(activatedOffer != null) ? "activatedOffer" : "no activatedOffer"
			}.ToArray());
			this.analyticsReporter.LogSpecialOfferSelectorNoOfferAvailable(offers, noOfferAvailableReason);
		}

		private ISpecialOffer GetActivatableOfferToShow(List<ISpecialOffer> offers)
		{
			foreach (ISpecialOffer specialOffer in offers)
			{
				if (!specialOffer.IsActivated())
				{
					if (specialOffer.CanActivate(this.mainProgression.GetMainProgression()))
					{
						return specialOffer;
					}
				}
			}
			return null;
		}

		private ISpecialOffer GetActivatedOfferToShow(List<ISpecialOffer> offers)
		{
			ISpecialOffer specialOffer = null;
			foreach (ISpecialOffer specialOffer2 in offers)
			{
				if (specialOffer2.IsActivated())
				{
					if (specialOffer == null)
					{
						specialOffer = specialOffer2;
					}
					else if (specialOffer2.GetLastShowingTimeStamp() < specialOffer.GetLastShowingTimeStamp())
					{
						specialOffer = specialOffer2;
					}
				}
			}
			return specialOffer;
		}

		private readonly IFeatureTypeHandler specialOffersHandler;

		private readonly IFeatureManager featureManager;

		private readonly ISpecialOffersMainProgressionProvider mainProgression;

		private readonly IConfigurationManager configurationManager;

		private readonly ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown;

		private readonly IAnalyticsReporter analyticsReporter;

		private readonly IAvailableSpecialOffers availableSpecialOffers;
	}
}
