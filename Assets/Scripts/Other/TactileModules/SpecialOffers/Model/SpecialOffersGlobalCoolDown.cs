using System;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOffersGlobalCoolDown : ISpecialOffersGlobalCoolDown
	{
		public SpecialOffersGlobalCoolDown(IFeatureManager featureManager, IFeatureTypeHandler handler, IConfigurationManager configurationManager)
		{
			this.featureManager = featureManager;
			this.handler = handler;
			this.configurationManager = configurationManager;
		}

		public bool IsCoolingDown()
		{
			SpecialOffersConfig config = this.configurationManager.GetConfig<SpecialOffersConfig>();
			SpecialOfferCustomData specialOfferCustomData = this.featureManager.GetFeatureTypeCustomData(this.handler) as SpecialOfferCustomData;
			int serverTime = this.featureManager.ServerTime;
			return serverTime <= specialOfferCustomData.GlobalCooldownTimestamp + config.GlobalCooldown;
		}

		public void Reset()
		{
			SpecialOfferCustomData specialOfferCustomData = this.featureManager.GetFeatureTypeCustomData(this.handler) as SpecialOfferCustomData;
			int serverTime = this.featureManager.ServerTime;
			specialOfferCustomData.GlobalCooldownTimestamp = serverTime;
		}

		public int GetCoolDownTimeStamp()
		{
			SpecialOfferCustomData specialOfferCustomData = this.featureManager.GetFeatureTypeCustomData(this.handler) as SpecialOfferCustomData;
			return specialOfferCustomData.GlobalCooldownTimestamp;
		}

		private readonly IFeatureManager featureManager;

		private readonly IFeatureTypeHandler handler;

		private readonly IConfigurationManager configurationManager;

		private SpecialOfferCustomData specialOfferCustomData;
	}
}
