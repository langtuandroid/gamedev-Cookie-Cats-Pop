using System;

namespace TactileModules.SpecialOffers.Model
{
	public interface ISpecialOffersGlobalCoolDown
	{
		bool IsCoolingDown();

		void Reset();

		int GetCoolDownTimeStamp();
	}
}
