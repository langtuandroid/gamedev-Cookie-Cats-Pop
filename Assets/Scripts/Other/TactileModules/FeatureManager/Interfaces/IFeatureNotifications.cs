using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureNotifications
	{
		FeatureNotificationSettings FeatureNotificationSettings { get; }

		string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
	}
}
