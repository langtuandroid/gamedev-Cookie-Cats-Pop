using System;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.TactileHub.Data
{
	public class HubGameData : IPromotedGameUtilityData, IPromotedGameUtilityPrimaryData
	{
		[JsonSerializable("name", null)]
		public string Name { get; set; }

		[JsonSerializable("iconUrl", null)]
		public string IconUrl { get; set; }

		[JsonSerializable("packageName", null)]
		public string PackageName { get; set; }

		[JsonSerializable("schemeUrl", null)]
		public string SchemeUrl { get; set; }

		[JsonSerializable("facebookAppId", null)]
		public string FacebookAppId { get; set; }

		[JsonSerializable("iTunesConnectId", null)]
		public string ITunesConnectId { get; set; }

		[JsonSerializable("adjustTrackerToken", null)]
		public string TrackerToken { get; set; }
	}
}
