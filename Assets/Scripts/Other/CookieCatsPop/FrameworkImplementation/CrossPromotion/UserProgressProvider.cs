using System;
using TactileModules.CrossPromotion.General.Ads;
using TactileModules.PuzzleGame.MainLevels;

namespace CookieCatsPop.FrameworkImplementation.CrossPromotion
{
	public class UserProgressProvider : IUserProgressProvider
	{
		public UserProgressProvider(IMainProgression mainProgression)
		{
			this.mainProgression = mainProgression;
		}

		public int GetUserProgress()
		{
			return this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
		}

		private readonly IMainProgression mainProgression;
	}
}
