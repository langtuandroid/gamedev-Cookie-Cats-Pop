using System;

namespace TactileModules.Ads
{
	public class InterstitialNotPayingUserRequirement : IInterstitialRequirement
	{
		public InterstitialNotPayingUserRequirement(IPayingUserProvider payingUserProvider)
		{
			this.payingUserProvider = payingUserProvider;
		}

		public bool RequirementIsMet(InterstitialProviderManagerData data)
		{
			return !this.payingUserProvider.IsPayingUser();
		}

		private readonly IPayingUserProvider payingUserProvider;
	}
}
