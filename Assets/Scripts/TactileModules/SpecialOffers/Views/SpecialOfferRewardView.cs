using System;
using System.Collections;
using System.Collections.Generic;

namespace TactileModules.SpecialOffers.Views
{
	public class SpecialOfferRewardView : ExtensibleView<ISpecialOfferRewardViewExtention>, ISpecialOfferRewardView, IUIView
	{
		public void Initialize(List<ItemAmount> rewards)
		{
			if (base.Extension != null)
			{
				base.Extension.Initialize(rewards);
			}
		}

		public IEnumerator AnimateClaimRewards()
		{
			if (base.Extension != null)
			{
				yield return base.Extension.AnimateClaimRewards();
			}
			yield break;
		}
	}
}
