using System;
using Tactile.GardenGame.MapSystem;
using TactileModules.GameCore.Boot;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class FlowFactory : IFlowFactory
	{
		public FlowFactory(IFlowStack flowStack, MainMapStateFactory mainMapStateFactory, MainMapStateProvider mainMapStateProvider)
		{
			this.flowStack = flowStack;
			this.mainMapStateFactory = mainMapStateFactory;
			this.mainMapStateProvider = mainMapStateProvider;
		}

		public MainMapState CreateAndPushStoryMapFlow()
		{
			MainMapState mainMapState = this.mainMapStateFactory.CreateState(this.mainMapStateProvider);
			this.flowStack.Push(mainMapState);
			return mainMapState;
		}

		private readonly IFlowStack flowStack;

		private readonly MainMapStateFactory mainMapStateFactory;

		private readonly IMainMapStateProvider mainMapStateProvider;
	}
}
