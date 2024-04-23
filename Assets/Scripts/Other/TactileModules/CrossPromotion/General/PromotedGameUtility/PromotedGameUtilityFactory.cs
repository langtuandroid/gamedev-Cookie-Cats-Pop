using System;

namespace TactileModules.CrossPromotion.General.PromotedGameUtility
{
	public class PromotedGameUtilityFactory
	{
		public IPromotedGameUtility Create(string currentGameId)
		{
			return new PromotedGameUtilityAndroidGoogle(currentGameId);
		}
	}
}
