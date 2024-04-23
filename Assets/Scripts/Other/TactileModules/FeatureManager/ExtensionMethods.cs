using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager
{
	public static class ExtensionMethods
	{
		public static U GetMetaData<T, U, V>(this FeatureData featureData, IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			return FeatureManager.Instance.GetFeatureInstanceMetaData<T, U, V>(featureTypeHandler, featureData);
		}

		public static FeatureManagerPersistableState PersistableState(this IFeatureManagerProvider provider)
		{
			return UserSettingsManager.Get<FeatureManagerPersistableState>();
		}

		public static void Save(this IFeatureManagerProvider provider)
		{
			UserSettingsManager.Instance.SaveLocalSettings();
			UserSettingsManager.Instance.SyncUserSettings();
		}

		public static V GetFeatureTypeCustomData<T, U, V>(this IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			return FeatureManager.Instance.GetFeatureTypeCustomData<T, U, V>(featureTypeHandler);
		}

		public static T GetFeatureInstanceCustomData<T, U, V>(this IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			ActivatedFeatureInstanceData activatedFeature = featureTypeHandler.GetActivatedFeature();
			if (activatedFeature == null)
			{
				return (T)((object)null);
			}
			return activatedFeature.GetCustomInstanceData<T, U, V>(featureTypeHandler);
		}

		public static U GetFeatureInstanceMetaData<T, U, V>(this IFeatureTypeHandler<T, U, V> featureTypeHandler) where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
		{
			ActivatedFeatureInstanceData activatedFeature = featureTypeHandler.GetActivatedFeature();
			if (activatedFeature == null)
			{
				return (U)((object)null);
			}
			return activatedFeature.GetMetaData<T, U, V>(featureTypeHandler);
		}

		public static FeatureTypeData GetFeatureTypeData(this IFeatureTypeHandler featureTypeHandler)
		{
			return FeatureManager.Instance.GetFeatureTypeData(featureTypeHandler);
		}

		public static List<ActivatedFeatureInstanceData> GetActivatedFeatures(this IFeatureTypeHandler featureTypeHandler)
		{
			return FeatureManager.Instance.GetActivatedFeatures(featureTypeHandler);
		}

		public static ActivatedFeatureInstanceData GetActivatedFeature(this IFeatureTypeHandler featureTypeHandler)
		{
			return FeatureManager.Instance.GetActivatedFeature(featureTypeHandler);
		}

		public static FeatureData GetAvailableFeature(this IFeatureTypeHandler featureTypeHandler)
		{
			return FeatureManager.Instance.GetAvailableFeature(featureTypeHandler);
		}

		public static List<FeatureData> GetAvailableFeatures(this IFeatureTypeHandler featureTypeHandler)
		{
			return FeatureManager.Instance.GetAvailableFeatures(featureTypeHandler);
		}

		public static IFeatureTypeHandler FeatureHandler(this FeatureData featureData)
		{
			foreach (IFeatureTypeHandler featureTypeHandler in FeatureManager.Instance.GetAllFeatureHandlers())
			{
				if (featureTypeHandler.FeatureType == featureData.Type)
				{
					return featureTypeHandler;
				}
			}
			throw new Exception("Featurehandler not found for feature data " + featureData.Type);
		}

		public static IFeatureTypeHandler FeatureHandler(this ActivatedFeatureInstanceData activatedFeatureInstanceData)
		{
			foreach (IFeatureTypeHandler featureTypeHandler in FeatureManager.Instance.GetAllFeatureHandlers())
			{
				for (int i = 0; i < featureTypeHandler.GetActivatedFeatures().Count; i++)
				{
					ActivatedFeatureInstanceData activatedFeatureInstanceData2 = featureTypeHandler.GetActivatedFeatures()[i];
					if (activatedFeatureInstanceData2.Id == activatedFeatureInstanceData.Id)
					{
						return featureTypeHandler;
					}
				}
			}
			throw new Exception("Featurehandler not found for feature instance " + activatedFeatureInstanceData.Id);
		}

		public static bool HasActiveFeature(this IFeatureTypeHandler featureTypeHandler)
		{
			return FeatureManager.Instance.HasActiveFeature(featureTypeHandler);
		}
	}
}
