using System;
using System.Collections;
using TactileModules.Ads.Analytics;

namespace TactileModules.CrossPromotion.Interstitials.ViewControllers
{
	public interface IInterstitialViewController
	{
		bool ShowViewIfPossible(AdGroupContext adGroupContext);

		IEnumerator WaitForClose();
	}
}
