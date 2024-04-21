using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IStoryMapEventActivation
	{
		void ActivateStoryMap();

		void DeactivateStoryMap();

		bool ShouldActivateStoryMap();

		bool ShouldDeactivateStoryMap();

		bool HasActiveFeature();

		bool IsFeatureEnabledInConfiguration();

		int GetSecondsLeft();
	}
}
