using System;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public class AdSession : IAdSession
	{
		public AdSession(AdType type, IGeneralDataRetriever generalDataRetriever, IGameSessionManager gameSessionManager)
		{
			this.type = type;
			this.generalDataRetriever = generalDataRetriever;
			gameSessionManager.NewSessionStarted += this.OnNewSession;
		}

		public void IncrementNumberOfTimesShown()
		{
			this.numberOfTimesShown++;
		}

		public bool CanShowInThisSession()
		{
			return this.numberOfTimesShown < this.generalDataRetriever.GetMaxAdsPerSession(this.type);
		}

		private void OnNewSession()
		{
			this.numberOfTimesShown = 0;
		}

		private readonly AdType type;

		private readonly IGeneralDataRetriever generalDataRetriever;

		private int numberOfTimesShown;
	}
}
