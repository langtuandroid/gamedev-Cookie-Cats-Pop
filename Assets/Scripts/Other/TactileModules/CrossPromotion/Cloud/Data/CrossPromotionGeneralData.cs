using System;
using System.Collections.Generic;
using TactileModules.CrossPromotion.TactileHub.Data;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionGeneralData
	{
		public CrossPromotionGeneralData()
		{
			this.CrossPromotionClientConfiguration = new CrossPromotionClientConfiguration();
			this.AvailableApps = new List<CrossPromotionAvailableApp>();
			this.HubGames = new List<HubGameData>();
		}

		[JsonSerializable("clientConfiguration", null)]
		public CrossPromotionClientConfiguration CrossPromotionClientConfiguration { get; set; }

		[JsonSerializable("availableApps", typeof(CrossPromotionAvailableApp))]
		public List<CrossPromotionAvailableApp> AvailableApps { get; set; }

		[JsonSerializable("hub", typeof(HubGameData))]
		public List<HubGameData> HubGames { get; set; }
	}
}
