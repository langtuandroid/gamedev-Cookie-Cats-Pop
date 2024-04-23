using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.SpecialOffers.Views
{
	public interface ISpecialOfferRewardView : IUIView
	{
		void Initialize(List<ItemAmount> rewards);

		IEnumerator AnimateClaimRewards();
	}
}
