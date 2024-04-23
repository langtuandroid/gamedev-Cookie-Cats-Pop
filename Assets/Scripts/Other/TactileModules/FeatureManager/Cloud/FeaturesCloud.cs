using System;
using System.Collections;
using System.Collections.Generic;
using Shared.FeatureManager.Module.Cloud;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.TactileCloud;
using TactileModules.TactileCloud.TargetingParameters;

namespace TactileModules.FeatureManager.Cloud
{
	public class FeaturesCloud : IFeaturesCloud
	{
		public FeaturesCloud(ICloudInterfaceBase cloudInterface, ICloudClientState cloudClientState, ITargetingParameterFactory targetingParameterFactory)
		{
			this.cloudInterface = cloudInterface;
			this.cloudClientState = cloudClientState;
			this.targetingParameterFactory = targetingParameterFactory;
		}

		public IEnumerator RefreshFeatures(IFeatureManager featureManager, FeaturesCloud.UpcomingFeaturesResultDelegate callback)
		{
			Dictionary<string, int> metaDataVersions = this.CollectMetaDataVersions(featureManager);
			List<string> activeFeatures = this.CollectActiveFeatureIds(featureManager);
			FeaturesResponse response = new FeaturesResponse();
			string userId = (!this.cloudClientState.HasValidUser) ? null : this.cloudClientState.CachedMe.CloudId;
			Hashtable targetingParameters = this.targetingParameterFactory.GetTargetingParameters();
			yield return this.cloudInterface.GetFeatures(userId, targetingParameters, metaDataVersions, activeFeatures, response);
			try
			{
				if (response.Success)
				{
					callback(null, response.AvailableFeatures, response.UpcomingFeatures, response.UnavailableFeatures);
				}
				else
				{
					callback("Failed to get upcoming features: " + response.ErrorInfo, null, null, null);
				}
			}
			catch (Exception ex)
			{
			}
			yield break;
		}

		private List<string> CollectActiveFeatureIds(IFeatureManager featureManager)
		{
			List<string> list = new List<string>();
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in featureManager.GetAllActivatedFeaturesUnfiltered())
			{
				list.Add(activatedFeatureInstanceData.Id);
			}
			return list;
		}

		private Dictionary<string, int> CollectMetaDataVersions(IFeatureManager featureManager)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (IFeatureTypeHandler featureTypeHandler in featureManager.GetAllFeatureHandlers())
			{
				dictionary.Add(featureTypeHandler.FeatureType, featureTypeHandler.MetaDataVersion);
			}
			return dictionary;
		}

		private readonly ICloudInterfaceBase cloudInterface;

		private readonly ICloudClientState cloudClientState;

		private readonly ITargetingParameterFactory targetingParameterFactory;

		public delegate void UpcomingFeaturesResultDelegate(object error, List<FeatureData> availableFeatures, List<FeatureData> upcomingFeatures, List<FeatureData> unavailableFeatures);
	}
}
