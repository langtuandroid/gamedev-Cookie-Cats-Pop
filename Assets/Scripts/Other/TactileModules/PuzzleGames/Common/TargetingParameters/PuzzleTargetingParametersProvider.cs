using System;
using System.Collections;
using Tactile;
using TactileModules.Ads;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.TactileCloud.TargetingParameters;

namespace TactileModules.PuzzleGames.Common.TargetingParameters
{
	public class PuzzleTargetingParametersProvider : ITargetingParametersProvider
	{
		public PuzzleTargetingParametersProvider(IMainProgression mainProgression, IUserSettings userSettings)
		{
			this.mainProgression = mainProgression;
			
			this.userSettings = userSettings;
		}

		protected virtual bool IsPayingUser()
		{
			return true;
		}

		public Hashtable GetAdditionalTargetingParameters()
		{
			Hashtable hashtable = new Hashtable
			{
				{
					"levelId",
					this.mainProgression.GetFarthestUnlockedLevelHumanNumber()
				},
				{
					"isPayingUser",
					this.IsPayingUser()
				}
			};
			
			return hashtable;
		}

		private readonly IMainProgression mainProgression;

		private readonly IUserSettings userSettings;
	}
}
