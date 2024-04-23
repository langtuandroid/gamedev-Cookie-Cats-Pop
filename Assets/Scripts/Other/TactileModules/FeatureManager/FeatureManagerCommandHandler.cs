using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager
{
	public class FeatureManagerCommandHandler : BaseCommandHandler
	{
		[UsedImplicitly]

		public static void ListAvailableFeatures()
		{
			foreach (FeatureData featureData in FeatureManager.Instance.GetAllCloudAvailableFeatures())
			{
			}
		}

		[UsedImplicitly]

		public static void ListUnavailableFeatures()
		{
			foreach (FeatureData featureData in FeatureManager.Instance.GetAllCloudUnavailableFeatures())
			{
			}
		}

		[UsedImplicitly]

		public static void ListUpcommingFeatures()
		{
			foreach (FeatureData featureData in FeatureManager.Instance.GetAllCloudUpcomingFeatures())
			{
			}
		}

		[UsedImplicitly]

		public static void ListActivatedFeatures()
		{
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in FeatureManager.Instance.GetAllActivatedFeatures())
			{
			}
		}

		[UsedImplicitly]

		public static void UpdateFeatureLists()
		{
			FeatureManager.Instance.UpdateFeatureLists(null);
		}

		[UsedImplicitly]

		public static void SyncTime()
		{
			FeatureManager.StabilizedTime.SyncTime(FeatureManager.Instance);
		}

		[UsedImplicitly]

		public static void ListMetaDataVersions()
		{
			string str = string.Empty;
			IEnumerable<IFeatureTypeHandler> allFeatureHandlers = FeatureManager.Instance.GetAllFeatureHandlers();
			foreach (IFeatureTypeHandler featureTypeHandler in allFeatureHandlers)
			{
				str += string.Format("{0}: {1}\n", featureTypeHandler.FeatureType, featureTypeHandler.MetaDataVersion);
			}
		}
	}
}
