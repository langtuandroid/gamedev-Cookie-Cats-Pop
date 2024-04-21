using System;
using TactileModules.CrossPromotion.Analytics.Data;

namespace TactileModules.CrossPromotion.Analytics.AnalyticsEvents
{
	public class CrossPromotionBasicEvent : BasicEvent
	{
		protected CrossPromotionBasicEvent(CrossPromotionAnalyticsData data)
		{
			this.RequestId = data.RequestId;
			this.CampaignId = data.CampaignId;
			this.CrossPromotionGame = data.CrossPromotionGame;
			this.CrossPromotionType = data.CrossPromotionType;
			this.Location = data.Location;
			this.ImageId = data.ImageId;
			this.ImageName = data.ImageName;
			this.ImageResolution = data.ImageResolution;
			this.VideoId = data.VideoId;
			this.VideoName = data.VideoName;
			this.VideoResolution = data.VideoResolution;
			this.Orientation = data.Orientation;
		}

		private TactileAnalytics.RequiredParam<string> RequestId { get; set; }

		private TactileAnalytics.RequiredParam<string> CampaignId { get; set; }

		private TactileAnalytics.RequiredParam<string> CrossPromotionGame { get; set; }

		private TactileAnalytics.RequiredParam<string> CrossPromotionType { get; set; }

		private TactileAnalytics.RequiredParam<string> Location { get; set; }

		private TactileAnalytics.RequiredParam<string> ImageId { get; set; }

		private TactileAnalytics.RequiredParam<string> ImageName { get; set; }

		private TactileAnalytics.RequiredParam<string> ImageResolution { get; set; }

		private TactileAnalytics.OptionalParam<string> VideoId { get; set; }

		private TactileAnalytics.OptionalParam<string> VideoName { get; set; }

		private TactileAnalytics.OptionalParam<string> VideoResolution { get; set; }

		private TactileAnalytics.RequiredParam<string> Orientation { get; set; }
	}
}
