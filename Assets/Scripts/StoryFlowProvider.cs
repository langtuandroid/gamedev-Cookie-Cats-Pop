using System;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class StoryFlowProvider : IStoryIntroProvider
{
	public IFlow CreateStoryFlow()
	{
		return new StoryFromMapFlow();
	}
}
