using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class StoryMapEventSystem : IStoryMapEventSystem
	{
		public StoryMapEventSystem(IStoryMapEventFeatureHandler storyMapEventFeatureHandler)
		{
			this.StoryMapEventFeatureHandler = storyMapEventFeatureHandler;
		}

		public IStoryMapEventFeatureHandler StoryMapEventFeatureHandler { get; private set; }
	}
}
