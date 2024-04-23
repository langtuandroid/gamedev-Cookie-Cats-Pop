using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.DisplayNaming;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.DummyFeature
{
	[DisplayName("Dummy")]
	[VersionDescription("This is a dummy description")]
	public class DummyFeatureHandler : IFeatureTypeHandler<FeatureInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>, IFeatureTypeHandler
	{
		public DummyFeatureHandler(IFeatureManager featureManager)
		{
			this.featureManager = featureManager;
			featureManager.OnFeatureListUpdated += this.AttemptToActivateFeatures;
		}

		private void AttemptToActivateFeatures()
		{
			List<FeatureData> availableFeatures = this.featureManager.GetAvailableFeatures(this);
			foreach (FeatureData featureData in availableFeatures)
			{
				if (this.featureManager.CanActivateFeature(this, featureData))
				{
					this.featureManager.ActivateFeature(this, featureData);
				}
			}
		}

		public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
		{
			this.featureManager.DeactivateFeature(this, activatedFeatureInstanceData);
		}

		public void FadeToBlack()
		{
		}

		public string FeatureType
		{
			get
			{
				return "dummy";
			}
		}

		public bool AllowMultipleFeatureInstances
		{
			get
			{
				return true;
			}
		}

		public int MetaDataVersion
		{
			get
			{
				return 0;
			}
		}

		public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
		{
			return metaData;
		}

		public FeatureInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
		{
			return new FeatureInstanceCustomData();
		}

		public FeatureTypeCustomData NewFeatureTypeCustomData()
		{
			return new FeatureTypeCustomData();
		}

		public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : FeatureInstanceCustomData
		{
		}

		public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : FeatureTypeCustomData
		{
		}

		private readonly IFeatureManager featureManager;
	}
}
