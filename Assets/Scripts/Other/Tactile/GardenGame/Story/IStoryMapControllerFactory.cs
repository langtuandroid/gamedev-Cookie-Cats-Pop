using System;
using Tactile.GardenGame.MapSystem;

namespace Tactile.GardenGame.Story
{
	public interface IStoryMapControllerFactory
	{
		IStoryMapController Create(MainMapController mainMapController);
	}
}
