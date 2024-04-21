using System;
using System.Collections;

[TactileAnalytics.EventAttribute("adjustAttribution", true)]
public class AdjustAttributionEvent
{
	public AdjustAttributionEvent(Hashtable queryParams)
	{
		this.AcquisitionChannel = (string)queryParams["aq"];
		if (queryParams.ContainsKey("ad"))
		{
			this.FBAdName = (string)queryParams["ad"];
			this.AdjAttrCreative = (string)queryParams["ad"];
		}
		if (queryParams.ContainsKey("adid"))
		{
			this.FBAdId = (string)queryParams["adid"];
		}
		if (queryParams.ContainsKey("adset"))
		{
			this.FBAdSetName = (string)queryParams["adset"];
			this.AdjAttrAdgroup = (string)queryParams["adset"];
		}
		if (queryParams.ContainsKey("adsetid"))
		{
			this.FBAdSetId = (string)queryParams["adsetid"];
		}
		if (queryParams.ContainsKey("campaign"))
		{
			this.FBCampaignName = (string)queryParams["campaign"];
			this.AdjAttrCampaign = (string)queryParams["campaign"];
		}
		if (queryParams.ContainsKey("campaignid"))
		{
			this.FBCampaignId = (string)queryParams["campaignid"];
		}
		if (queryParams.ContainsKey("placement"))
		{
			this.FBPlacement = (string)queryParams["placement"];
		}
	}

	private TactileAnalytics.RequiredParam<string> AcquisitionChannel { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrAdgroup { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrAdgroupId { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrCampaign { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrCreative { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrNetwork { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrTrackerName { get; set; }

	private TactileAnalytics.OptionalParam<string> AdjAttrTrackerToken { get; set; }

	private TactileAnalytics.OptionalParam<string> ChartboostCampaignId { get; set; }

	private TactileAnalytics.OptionalParam<string> ChartboostCreativeId { get; set; }

	private TactileAnalytics.OptionalParam<string> ChartboostTargetId { get; set; }

	private TactileAnalytics.OptionalParam<string> FBAdId { get; set; }

	private TactileAnalytics.OptionalParam<string> FBAdName { get; set; }

	private TactileAnalytics.OptionalParam<string> FBAdSetId { get; set; }

	private TactileAnalytics.OptionalParam<string> FBAdSetName { get; set; }

	private TactileAnalytics.OptionalParam<string> FBCampaignId { get; set; }

	private TactileAnalytics.OptionalParam<string> FBCampaignName { get; set; }

	private TactileAnalytics.OptionalParam<string> FBPlacement { get; set; }

	private TactileAnalytics.OptionalParam<string> TweetId { get; set; }

	private TactileAnalytics.OptionalParam<string> TwitterLineItemId { get; set; }
}
