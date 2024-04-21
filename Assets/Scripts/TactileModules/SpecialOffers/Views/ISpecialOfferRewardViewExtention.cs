using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.SpecialOffers.Views
{
	public interface ISpecialOfferRewardViewExtention
	{
		void Initialize(List<ItemAmount> rewards);

		IEnumerator AnimateClaimRewards();
	}
}
