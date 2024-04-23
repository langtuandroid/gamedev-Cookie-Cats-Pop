using System;

namespace TactileModules.Ads
{
	public interface IInterstitialRequirement
	{
		bool RequirementIsMet(InterstitialProviderManagerData data);
	}
}
