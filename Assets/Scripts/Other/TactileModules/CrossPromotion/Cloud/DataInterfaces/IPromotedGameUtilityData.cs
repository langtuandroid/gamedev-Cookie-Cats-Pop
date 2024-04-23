using System;
using JetBrains.Annotations;

namespace TactileModules.CrossPromotion.Cloud.DataInterfaces
{
	public interface IPromotedGameUtilityData : IPromotedGameUtilityPrimaryData
	{
		[UsedImplicitly]
		string TrackerToken { get; }

		[UsedImplicitly]
		string ITunesConnectId { get; }
	}
}
