using System;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager
{
	public class FeatureManagerSystem
	{
		public FeatureManagerSystem(IFeatureManager featureManager)
		{
			this.FeatureManager = featureManager;
		}

		public IFeatureManager FeatureManager { get; private set; }
	}
}
