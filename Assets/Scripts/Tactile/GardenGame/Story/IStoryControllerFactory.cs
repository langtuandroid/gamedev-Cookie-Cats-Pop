using System;
using Tactile.GardenGame.MapSystem;

namespace Tactile.GardenGame.Story
{
	public interface IStoryControllerFactory
	{
		IStoryController CreateController(MainMapController mainMapController);
	}
}
