using System;
using System.Collections.Generic;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOfferMetaData : FeatureMetaData
	{
		[Description("Id used for analytics purpose only (eg welcome-pack")]
		[JsonSerializable("AnalyticsId", null)]
		public string AnalyticsId { get; set; }

		[Description("Name of special offer to be shown in notification")]
		[JsonSerializable("NotificationDisplayName", null)]
		public string NotificationDisplayName { get; set; }

		[Required]
		[Description("Type of special offer")]
		[JsonSerializable("SpecialOfferType", null)]
		public SpecialOfferTypeEnum SpecialOfferType { get; set; }

		[Description("The level required to see this special offer")]
		[JsonSerializable("RequiredLevel", null)]
		public int RequiredLevel { get; set; }

		[Description("[Only use if SpecialOfferType==IAP] InAppPurchase Identifier for the price NOW")]
		[JsonSerializable("IAPIdentifier", null)]
		public string IAPIdentifier { get; set; }

		[Description("[Only use if SpecialOfferType==IAP] InAppPurchase Identifier for the price BEFORE")]
		[JsonSerializable("IAPIdentifierBefore", null)]
		public string IAPIdentifierBefore { get; set; }

		[Description("[Only use if SpecialOfferType==Coins] Coin Price")]
		[JsonSerializable("CoinPrice", null)]
		public int CoinPrice { get; set; }

		[Required]
		[Description("Texture URL")]
		[JsonSerializable("TextureURL", null)]
		public string TextureURL { get; set; }

		[Required]
		[Description("Side Map Button Texture URL")]
		[JsonSerializable("SideMapButtonTextureURL", null)]
		public string SideMapButtonTextureURL { get; set; }

		[Required]
		[Description("The contents of the special offer")]
		[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
		[JsonSerializable("OfferEntryRewardConfig", typeof(ItemAmount))]
		public List<ItemAmount> Reward { get; set; }
	}
}
