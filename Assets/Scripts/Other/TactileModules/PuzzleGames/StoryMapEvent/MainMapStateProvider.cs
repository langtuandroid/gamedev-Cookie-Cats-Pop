using System;
using Tactile.GardenGame.MapSystem;
using TactileModules.GameCore.Boot;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class MainMapStateProvider : IMainMapStateProvider
	{
		public MainMapStateProvider(ICurrencyAvailability currencyAvailability)
		{
			this.currencyAvailability = currencyAvailability;
		}

		public MainMapState GetMainMapState()
		{
			throw new NotImplementedException();
		}

		public int GetNextDotIndexToOpen()
		{
			if (!this.currencyAvailability.HasMainProgressionLevelsToPlay)
			{
				return this.currencyAvailability.GetRandomEndOfContentLevelIndex();
			}
			return this.currencyAvailability.MinimumLevelIndex;
		}

		private readonly ICurrencyAvailability currencyAvailability;
	}
}
