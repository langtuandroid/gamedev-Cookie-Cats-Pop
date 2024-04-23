using System;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.SpecialOffers.Model
{
	[ConfigProvider("SpecialOffersConfig")]
	public class SpecialOffersConfig
	{
		[Description("The cooldown amount of all other offers that uses the global cooldown system")]
		[DevelopmentState(new DevelopmentStateAttribute.DevelopmentState[]
		{
			DevelopmentStateAttribute.DevelopmentState.dev,
			DevelopmentStateAttribute.DevelopmentState.prod
		})]
		[JsonSerializable("GlobalCooldown", null)]
		public int GlobalCooldown { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }
	}
}
