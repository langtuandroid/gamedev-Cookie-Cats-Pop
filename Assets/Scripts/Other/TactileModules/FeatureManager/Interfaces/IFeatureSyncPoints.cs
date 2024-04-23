using System;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureSyncPoints
	{
		event Action SafeForFeaturesToSync;
	}
}
