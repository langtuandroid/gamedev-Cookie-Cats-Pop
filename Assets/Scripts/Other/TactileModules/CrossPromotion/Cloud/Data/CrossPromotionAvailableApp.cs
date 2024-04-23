using System;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionAvailableApp : IPromotedGameUtilityPrimaryData
	{
		[JsonSerializable("packageName", null)]
		public string PackageName { get; set; }

		[JsonSerializable("schemeUrl", null)]
		public string SchemeUrl { get; set; }

		public string FacebookAppId { get; set; }
	}
}
