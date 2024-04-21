using System;
using Tactile.GardenGame.MapSystem;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IFlowFactory
	{
		MainMapState CreateAndPushStoryMapFlow();
	}
}
