using System;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
	public abstract class PromotedGameUtilityAndroid : PromotedGameUtilityBase
	{
		protected PromotedGameUtilityAndroid(string currentGameId) : base(currentGameId)
		{
		}

		public override bool IsGameInstalled(IPromotedGameUtilityPrimaryData utilityData)
		{
			return ActivityAndroid.isPackageInstalled(utilityData.PackageName);
		}

		public override void LaunchGame(IPromotedGameUtilityPrimaryData utilityData)
		{
			ActivityAndroid.launchOtherApp(utilityData.PackageName);
		}
	}
}
