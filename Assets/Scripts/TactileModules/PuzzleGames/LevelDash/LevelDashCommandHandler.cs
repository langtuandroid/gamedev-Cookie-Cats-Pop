using System;
using System.Collections.Generic;


namespace TactileModules.PuzzleGames.LevelDash
{
	public class LevelDashCommandHandler : BaseCommandHandler
	{
		public static void InjectDependencies(LevelDashViewController levelDashViewController)
		{
			LevelDashCommandHandler.levelDashViewController = levelDashViewController;
		}

	
		private static void ShowStartView()
		{
			FiberCtrl.Pool.Run(LevelDashCommandHandler.levelDashViewController.ShowStartPopup(false), false);
		}

		
		private static void ShowLeaderboardView()
		{
			FiberCtrl.Pool.Run(LevelDashCommandHandler.levelDashViewController.ShowLeaderboardView(), false);
		}


		private static void ShowRewardView()
		{
			LevelDashConfig.Reward reward = new LevelDashConfig.Reward();
			reward.Rank = 1;
			reward.Items = new List<ItemAmount>
			{
				new ItemAmount
				{
					Amount = 700,
					ItemId = "Coin"
				},
				new ItemAmount
				{
					Amount = 86400,
					ItemId = "UnlimitedLives"
				}
			};
			FiberCtrl.Pool.Run(LevelDashCommandHandler.levelDashViewController.HandleFeatureEnd(reward), false);
		}

		private static LevelDashViewController levelDashViewController;
	}
}
