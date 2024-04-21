using System;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.Cloud.RequestState;
using TactileModules.RuntimeTools;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public class CrossPromotionAdUpdater : ICrossPromotionAdUpdater
	{
		public CrossPromotionAdUpdater(ICrossPromotionAdRetriever crossPromotionAdRetriever, IGeneralDataRetriever generalDataRetriever, ITactileDateTime tactileDateTime, IRequestStateHandler requestStateHandler)
		{
			this.crossPromotionAdRetriever = crossPromotionAdRetriever;
			this.generalDataRetriever = generalDataRetriever;
			this.tactileDateTime = tactileDateTime;
			this.requestStateHandler = requestStateHandler;
		}

		public void UpdateCrossPromotionAd()
		{
			ICrossPromotionAd promotion = this.crossPromotionAdRetriever.GetPromotion();
			if (promotion == null)
			{
				this.HandleMissingCrossPromotion();
			}
			else if (promotion.HasExpired())
			{
				this.RequestNewCrossPromotion();
			}
			else
			{
				promotion.EnsureIsCached();
			}
		}

		private void HandleMissingCrossPromotion()
		{
			DateTime lastSuccessfulRequestTimestamp = this.requestStateHandler.GetLastSuccessfulRequestTimestamp();
			TimeSpan timeSpan = this.tactileDateTime.UtcNow.Subtract(lastSuccessfulRequestTimestamp);
			int maxAdAge = this.generalDataRetriever.GetGeneralData().CrossPromotionClientConfiguration.MaxAdAge;
			if (timeSpan.TotalSeconds >= (double)maxAdAge)
			{
				this.RequestNewCrossPromotion();
			}
		}

		private void RequestNewCrossPromotion()
		{
			this.crossPromotionAdRetriever.RequestNewPromotion();
		}

		private readonly ICrossPromotionAdRetriever crossPromotionAdRetriever;

		private readonly IGeneralDataRetriever generalDataRetriever;

		private readonly ITactileDateTime tactileDateTime;

		private readonly IRequestStateHandler requestStateHandler;
	}
}
