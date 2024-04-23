using System;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.MapEditor
{
	public interface IMapEditorBoot
	{
		IFlowStack FlowStack { get; }

		IFullScreenManager FullScreenManager { get; }

		StorySystem Story { get; }

		PropsManager Props { get; }

		MainMapStateFactory MainMapStateFactory { get; }
	}
}
