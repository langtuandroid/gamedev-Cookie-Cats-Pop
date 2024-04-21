using System;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankGameSessionController
	{
		public PiggyBankGameSessionController(IPiggyBankProvider provider, IPiggyBankRewards rewards, IPiggyBankProgression progression)
		{
			this.provider = provider;
			this.rewards = rewards;
			this.progression = progression;
			provider.LevelPlayed += this.LevelPlayedHandler;
			provider.InGameBoosterUsed += this.InGameBoosterUsedHandler;
		}

		public IPiggyBankProvider Provider
		{
			get
			{
				return this.provider;
			}
		}

		public void GameSessionEnded()
		{
			this.provider.LevelPlayed -= this.LevelPlayedHandler;
			this.provider.InGameBoosterUsed -= this.InGameBoosterUsedHandler;
		}

		private void LevelPlayedHandler()
		{
			if (this.IsActive())
			{
				this.rewards.AddPlayedCoins();
			}
			this.GameSessionEnded();
		}

		private void InGameBoosterUsedHandler()
		{
			if (this.IsActive())
			{
				this.rewards.AddBoosterCoins();
			}
		}

		private bool IsActive()
		{
			return this.IsFeatureStarted() && this.progression.TutorialShown;
		}

		private bool IsFeatureStarted()
		{
			return this.progression.IsFeatureEnabled() && this.progression.StartingLevel > 0;
		}

		private readonly IPiggyBankProvider provider;

		private readonly IPiggyBankRewards rewards;

		private readonly IPiggyBankProgression progression;
	}
}
