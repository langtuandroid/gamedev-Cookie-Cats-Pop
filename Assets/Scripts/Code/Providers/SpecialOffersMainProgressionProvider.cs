using System;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.SpecialOffers.Model;

namespace Code.Providers
{
	public class SpecialOffersMainProgressionProvider : ISpecialOffersMainProgressionProvider
	{
		public SpecialOffersMainProgressionProvider(MainProgressionManager mainProgressionManager)
		{
			this.mainProgressionManager = mainProgressionManager;
		}

		public int GetMainProgression()
		{
			return this.mainProgressionManager.GetFarthestUnlockedLevelHumanNumber();
		}

		private readonly MainProgressionManager mainProgressionManager;
	}
}
