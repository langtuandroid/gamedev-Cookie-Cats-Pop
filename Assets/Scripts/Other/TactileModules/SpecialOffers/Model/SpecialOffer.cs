using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.InAppPurchase;
using UnityEngine;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOffer : ISpecialOffer
	{
		public SpecialOffer(IFeatureManager featureManager, IFeatureTypeHandler handler, IInAppPurchaseManager inAppPurchaseManager, string featureInstanceId)
		{
			this.featureManager = featureManager;
			this.handler = handler;
			this.inAppPurchaseManager = inAppPurchaseManager;
			this.FeatureInstanceId = featureInstanceId;
		}

		public string FeatureInstanceId { get; private set; }

		public bool IsActivated()
		{
			ActivatedFeatureInstanceData activatedFeature = this.featureManager.GetActivatedFeature(this.handler, this.FeatureInstanceId);
			return activatedFeature != null;
		}

		public bool CanActivate(int farthestUnlockedLevelHumanNumber)
		{
			FeatureData featureData = this.GetFeatureData();
			return featureData != null && this.featureManager.CanActivateFeature(this.handler, featureData) && farthestUnlockedLevelHumanNumber >= this.GetLevelRequirement();
		}

		public int GetLastShowingTimeStamp()
		{
			SpecialOfferInstanceCustomData instanceData = this.GetInstanceData();
			if (instanceData != null)
			{
				return instanceData.DidShowTimeStamp;
			}
			return 0;
		}

		public void SetLastShowingTimeStamp()
		{
			SpecialOfferInstanceCustomData instanceData = this.GetInstanceData();
			if (instanceData != null)
			{
				instanceData.DidShowTimeStamp = this.featureManager.ServerTime;
			}
		}

		public void Activate()
		{
			FeatureData featureData = this.GetFeatureData();
			this.featureManager.ActivateFeature(this.handler, featureData);
		}

		public void Deactivate()
		{
			ActivatedFeatureInstanceData activatedInstanceData = this.GetActivatedInstanceData();
			this.featureManager.DeactivateFeature(this.handler, activatedInstanceData);
		}

		public List<ItemAmount> GetReward()
		{
			return this.GetMetaData().Reward;
		}

		public Texture2D LoadTexture()
		{
			return this.featureManager.LoadTextureFromCache(this.handler, this.GetMetaData().TextureURL);
		}

		public Texture2D LoadSideMapButtonTexture()
		{
			return this.featureManager.LoadTextureFromCache(this.handler, this.GetMetaData().SideMapButtonTextureURL);
		}

		public string GetTimeLeft()
		{
			int secondsLeft = this.GetSecondsLeft();
			return L.FormatSecondsAsColumnSeparated(secondsLeft, "Ended", TimeFormatOptions.None);
		}

		public int GetSecondsLeft()
		{
			int result = 0;
			ActivatedFeatureInstanceData activatedInstanceData = this.GetActivatedInstanceData();
			if (activatedInstanceData != null)
			{
				result = this.featureManager.GetStabilizedTimeLeftToFeatureDurationEnd(activatedInstanceData);
			}
			return result;
		}

		public string GetPriceNow()
		{
			return this.GetOfferFormatedPrice(this.GetIAPIdentifier());
		}

		public string GetPriceBefore()
		{
			return this.GetOfferFormatedPrice(this.GetIAPIdentifierBefore());
		}

		public new SpecialOfferTypeEnum GetType()
		{
			return this.GetMetaData().SpecialOfferType;
		}

		public int GetCoinPrice()
		{
			return this.GetMetaData().CoinPrice;
		}

		public string GetAnalyticsId()
		{
			return this.GetMetaData().AnalyticsId;
		}

		public int GetLevelRequirement()
		{
			return this.GetMetaData().RequiredLevel;
		}

		public bool IsValid()
		{
			return string.IsNullOrEmpty(this.IsValidInternal());
		}

		public string GetValidationInfo()
		{
			return this.IsValidInternal();
		}

		private string IsValidInternal()
		{
			string text = string.Empty;
			if (this.GetType() == SpecialOfferTypeEnum.IAP)
			{
				text = this.ValidatePriceNow();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = this.ValidatePriceBefore();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return text;
		}

		private string ValidatePriceNow()
		{
			string result = string.Empty;
			if (string.IsNullOrEmpty(this.GetPriceNow()))
			{
				result = "newPriceIsNull";
			}
			return result;
		}

		private string ValidatePriceBefore()
		{
			string result = string.Empty;
			if (this.HidePriceBefore())
			{
				return result;
			}
			if (string.IsNullOrEmpty(this.GetPriceBefore()))
			{
				result = "oldPriceIsNull";
			}
			return result;
		}

		private bool HidePriceBefore()
		{
			string iapidentifierBefore = this.GetMetaData().IAPIdentifierBefore;
			return string.IsNullOrEmpty(iapidentifierBefore);
		}

		public string GetIAPIdentifier()
		{
			return InAppProductTactileInfo.IdentifierPrefix + this.GetMetaData().IAPIdentifier;
		}

		private string GetIAPIdentifierBefore()
		{
			return InAppProductTactileInfo.IdentifierPrefix + this.GetMetaData().IAPIdentifierBefore;
		}

		private string GetOfferFormatedPrice(string iapIdentifier)
		{
			InAppProduct productForIdentifier = this.inAppPurchaseManager.GetProductForIdentifier(iapIdentifier);
			if (productForIdentifier != null)
			{
				return productForIdentifier.FormattedPrice;
			}
			return string.Empty;
		}

		private FeatureData GetFeatureData()
		{
			ActivatedFeatureInstanceData activatedInstanceData = this.GetActivatedInstanceData();
			if (activatedInstanceData != null)
			{
				return activatedInstanceData.FeatureData;
			}
			return this.featureManager.GetFeature(this.handler, this.FeatureInstanceId);
		}

		private SpecialOfferMetaData GetMetaData()
		{
			FeatureData featureData = this.GetFeatureData();
			if (featureData != null)
			{
				return this.featureManager.GetFeatureInstanceMetaData<SpecialOfferMetaData>(featureData);
			}
			return null;
		}

		private ActivatedFeatureInstanceData GetActivatedInstanceData()
		{
			return this.featureManager.GetActivatedFeature(this.handler, this.FeatureInstanceId);
		}

		private SpecialOfferInstanceCustomData GetInstanceData()
		{
			ActivatedFeatureInstanceData activatedInstanceData = this.GetActivatedInstanceData();
			if (activatedInstanceData != null)
			{
				return activatedInstanceData.GetCustomInstanceData() as SpecialOfferInstanceCustomData;
			}
			return null;
		}

		private readonly IFeatureManager featureManager;

		private readonly IFeatureTypeHandler handler;

		private readonly IInAppPurchaseManager inAppPurchaseManager;
	}
}
