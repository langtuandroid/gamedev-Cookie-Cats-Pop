using System;
using JetBrains.Annotations;

namespace TactileModules.CrossPromotion.Cloud.DataInterfaces
{
	public interface IPromotedGameUtilityPrimaryData
	{
		[UsedImplicitly]
		string PackageName { get; }

		[UsedImplicitly]
		string SchemeUrl { get; }

		[UsedImplicitly]
		string FacebookAppId { get; }
	}
}
