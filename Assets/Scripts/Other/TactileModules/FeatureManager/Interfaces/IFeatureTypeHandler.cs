using System;
using System.Collections;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureTypeHandler
	{
		void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData);

		void FadeToBlack();

		string FeatureType { get; }

		bool AllowMultipleFeatureInstances { get; }

		int MetaDataVersion { get; }

		Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion);
	}
}
