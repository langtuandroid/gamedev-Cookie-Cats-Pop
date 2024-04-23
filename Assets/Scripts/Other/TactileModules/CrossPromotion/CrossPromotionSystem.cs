using System;
using TactileModules.CrossPromotion.Interstitials.ViewControllers;

namespace TactileModules.CrossPromotion
{
	public class CrossPromotionSystem
	{
		public CrossPromotionSystem(IInterstitialControllerFactory interstitialControllerFactory)
		{
			this.InterstitialControllerFactory = interstitialControllerFactory;
		}

		public IInterstitialControllerFactory InterstitialControllerFactory { get; private set; }
	}
}
