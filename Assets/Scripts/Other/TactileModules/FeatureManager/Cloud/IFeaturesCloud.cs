using System;
using System.Collections;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Cloud
{
	public interface IFeaturesCloud
	{
		IEnumerator RefreshFeatures(IFeatureManager featureManager, FeaturesCloud.UpcomingFeaturesResultDelegate callback);
	}
}
