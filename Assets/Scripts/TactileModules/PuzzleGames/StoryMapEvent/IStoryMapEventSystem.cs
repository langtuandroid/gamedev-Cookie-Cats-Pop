using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IStoryMapEventSystem
	{
		IStoryMapEventFeatureHandler StoryMapEventFeatureHandler { get; }
	}
}
