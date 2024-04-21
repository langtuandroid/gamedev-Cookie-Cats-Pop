using System;
using Tactile.GardenGame.MapSystem;

namespace TactileModules.GameCore.Boot
{
	public interface IMainMapStateProvider
	{
		MainMapState GetMainMapState();

		int GetNextDotIndexToOpen();
	}
}
