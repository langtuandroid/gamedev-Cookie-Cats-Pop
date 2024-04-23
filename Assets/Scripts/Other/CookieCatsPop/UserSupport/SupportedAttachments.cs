using System;
using System.Collections.Generic;
using TactileModules.UserSupport;

namespace CookieCatsPop.UserSupport
{
	public class SupportedAttachments : ISupportedAttachments
	{
		public List<string> GetSupportedAttachments()
		{
			return this.supportedAttachments;
		}

		private readonly List<string> supportedAttachments = new List<string>
		{
			"Coin",
			"BoosterShield",
			"BoosterSuperAim",
			"BoosterSuperQueue",
			"BoosterShield_Unlimited",
			"BoosterSuperAim_Unlimited",
			"BoosterSuperQueue_Unlimited",
			"BoosterFinalPower",
			"BoosterRainbow"
		};
	}
}
