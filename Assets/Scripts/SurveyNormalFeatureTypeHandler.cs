using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

public class SurveyNormalFeatureTypeHandler : IFeatureTypeHandler<FeatureInstanceCustomData, SurveyMetaData, FeatureTypeCustomData>, IFeatureTypeHandler
{
	public SurveyNormalFeatureTypeHandler(FeatureManager featureManager)
	{
		this.featureManager = featureManager;
	}

	public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
	{
	}

	public void FadeToBlack()
	{
		foreach (FeatureData featureData in this.featureManager.GetFeatures(this))
		{
			if (this.featureManager.CanActivateFeature(this, featureData))
			{
				this.featureManager.ActivateFeature(this, featureData);
			}
		}
	}

	public string FeatureType
	{
		get
		{
			return "survey-normal";
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
			return 3;
		}
	}

	public Hashtable UpgradeMetaData(Hashtable metaData, int fromVersion, int toVersion)
	{
		if (fromVersion == 0 && toVersion == 1)
		{
			return metaData;
		}
		if (fromVersion == 1 && toVersion == 2)
		{
			return metaData;
		}
		throw new NotSupportedException();
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

	private readonly FeatureManager featureManager;
}
