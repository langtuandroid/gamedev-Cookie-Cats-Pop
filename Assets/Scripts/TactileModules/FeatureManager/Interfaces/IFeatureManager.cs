using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureManager
	{
		event Action OnFeatureListUpdated;

		event Action<ActivatedFeatureInstanceData> OnFeatureActivated;

		event Action<ActivatedFeatureInstanceData> OnFeatureDeactivated;

		int ServerTime { get; }

		ActivatedFeatureInstanceData GetActivatedFeature(IFeatureTypeHandler featureTypeHandler);

		ActivatedFeatureInstanceData GetActivatedFeature(IFeatureTypeHandler featureTypeHandler, string id);

		FeatureData GetAvailableFeature(IFeatureTypeHandler featureTypeHandler);

		List<ActivatedFeatureInstanceData> GetActivatedFeatures(IFeatureTypeHandler featureTypeHandler);

		List<FeatureData> GetAllCloudUpcomingFeatures();

		int GetStabilizedTimeLeftToFeatureDurationEnd(ActivatedFeatureInstanceData state);

		int GetTimeLeftToFeatureStart(FeatureData featureData);

		List<FeatureData> GetAvailableFeatures(IFeatureTypeHandler featureTypeHandler);

		bool CanActivateFeature(IFeatureTypeHandler featureTypeHandler);

		bool CanActivateFeature(IFeatureTypeHandler featureTypeHandler, FeatureData featureData);

		bool HasActiveFeature(IFeatureTypeHandler featureTypeHandler);

		FeatureData GetFeature(IFeatureTypeHandler featureTypeHandler);

		FeatureData GetFeature(IFeatureTypeHandler featureTypeHandler, string id);

		ActivatedFeatureInstanceData ActivateFeature(IFeatureTypeHandler featureTypeHandler);

		ActivatedFeatureInstanceData ActivateFeature(IFeatureTypeHandler featureTypeHandler, FeatureData featureData);

		bool ShouldDeactivateFeature(IFeatureTypeHandler featureTypeHandler);

		bool ShouldDeactivateFeature(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData state);

		void DeactivateFeature(IFeatureTypeHandler featureTypeHandler);

		void DeactivateFeature(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData activatedInstanceData);

		U GetFeatureInstanceMetaData<U>(FeatureData featureData) where U : FeatureMetaData;

		FeatureTypeCustomData GetFeatureTypeCustomData(IFeatureTypeHandler featureTypeHandler);

		Texture2D LoadTextureFromCache(IFeatureTypeHandler handler, string url);

		List<ActivatedFeatureInstanceData> GetAllActivatedFeatures();

		IEnumerable<IFeatureTypeHandler> GetAllFeatureHandlers();

		void UpgradeOutdatedMetaData(IFeatureTypeHandler featureHandler);

		List<ActivatedFeatureInstanceData> GetAllActivatedFeaturesUnfiltered();
	}
}
