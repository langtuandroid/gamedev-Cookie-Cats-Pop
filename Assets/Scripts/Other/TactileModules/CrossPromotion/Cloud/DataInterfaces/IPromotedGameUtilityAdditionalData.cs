using System;

namespace TactileModules.CrossPromotion.Cloud.DataInterfaces
{
	public interface IPromotedGameUtilityAdditionalData
	{
		string RequestId { get; }

		string CampaignId { get; }

		string TrackerCreativeName { get; }
	}
}
