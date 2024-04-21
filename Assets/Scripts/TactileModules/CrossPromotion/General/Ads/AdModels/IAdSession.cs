using System;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public interface IAdSession
	{
		void IncrementNumberOfTimesShown();

		bool CanShowInThisSession();
	}
}
