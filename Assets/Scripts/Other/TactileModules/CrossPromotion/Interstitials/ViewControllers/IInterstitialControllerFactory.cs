using System;

namespace TactileModules.CrossPromotion.Interstitials.ViewControllers
{
	public interface IInterstitialControllerFactory
	{
		IInterstitialViewController CreateViewController();
	}
}
