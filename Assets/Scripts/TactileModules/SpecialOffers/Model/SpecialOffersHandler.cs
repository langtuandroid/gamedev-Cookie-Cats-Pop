using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FeatureManager;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.DisplayNaming;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.SpecialOffers.Model
{
	[DisplayName("Special Offer \ud83d\udcb0")]
	[VersionDescription("Special offers no longer uses Asset Bundles for its visuals, but rather textures.")]
	public sealed class SpecialOffersHandler : IFeatureTypeHandler<SpecialOfferInstanceCustomData, SpecialOfferMetaData, SpecialOfferCustomData>, IFeatureUrlFileHandler, IFeatureNotifications, IFeatureTypeHandler
	{
		public SpecialOffersHandler(IConfigurationManager configurationManager)
		{
			this.configurationManager = configurationManager;
		}

		public void FeatureRemovedFromDashboard(ActivatedFeatureInstanceData activatedFeatureInstanceData)
		{
		}

		public void FadeToBlack()
		{
		}

		public string FeatureType
		{
			get
			{
				return "special-offer";
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
				return 2;
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

		public SpecialOfferInstanceCustomData NewFeatureInstanceCustomData(FeatureData featureData)
		{
			return new SpecialOfferInstanceCustomData();
		}

		public SpecialOfferCustomData NewFeatureTypeCustomData()
		{
			return new SpecialOfferCustomData();
		}

		public void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : SpecialOfferInstanceCustomData
		{
			toMerge = ((current.DidShowTimeStamp <= cloud.DidShowTimeStamp) ? cloud : current);
		}

		public void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : SpecialOfferCustomData
		{
			toMerge = ((current.GlobalCooldownTimestamp <= cloud.GlobalCooldownTimestamp) ? cloud : current);
		}

		public List<string> GetUrlsToCache(FeatureData featureData)
		{
			List<string> list = new List<string>();
			SpecialOfferMetaData metaData = featureData.GetMetaData(this);
			if (metaData != null)
			{
				if (!string.IsNullOrEmpty(metaData.TextureURL))
				{
					list.Add(metaData.TextureURL);
				}
				if (!string.IsNullOrEmpty(metaData.SideMapButtonTextureURL))
				{
					list.Add(metaData.SideMapButtonTextureURL);
				}
			}
			return list;
		}

		public void FeatureInstanceWasHidden(ActivatedFeatureInstanceData instanceData)
		{
		}

		public FeatureNotificationSettings FeatureNotificationSettings
		{
			get
			{
				return this.configurationManager.GetConfig<SpecialOffersConfig>().FeatureNotificationSettings;
			}
		}

		public string GetNotificationText(TimeSpan span, ActivatedFeatureInstanceData instanceData)
		{
			string arg = (span.Hours > 0) ? (span.Hours + " " + L.Get("hours")) : (span.Minutes + " " + L.Get("minutes"));
			SpecialOfferMetaData metaData = instanceData.GetMetaData<SpecialOfferInstanceCustomData, SpecialOfferMetaData, SpecialOfferCustomData>(this);
			string notificationDisplayName = metaData.NotificationDisplayName;
			return string.Format(L.Get("The {0} will expire in {1}!"), notificationDisplayName, arg);
		}

		private readonly IConfigurationManager configurationManager;
	}
}
