using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	public interface IFeatureAvailabilityModel
	{
		void UpdateFeatureAvailabilityAndLogAnalyticsEvent(FeatureData featureData, bool isAvailable, bool areFeatureAssetsAvailable);

		void EnsureFeatureIsRemoved(string featureId);
	}
}
