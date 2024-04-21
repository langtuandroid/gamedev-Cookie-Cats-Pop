using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Story
{
	public interface IStoryManager
	{
		event Action ProgressionChanged;

		int CurrentChapter { get; }

		List<MapTask> AllTasks { get; }

		IHookList<int, bool> LastTaskInChapterComplete { get; }

		bool FirstIntroCompleted { get; }

		int TotalPagesCollected { get; set; }

		int GetChapterProgression();

		bool ShouldEnterStoryMapAutomatically { get; }

		bool NoTasksCompleted { get; }

		bool HasStarsToCompleteATask();

		StoryManager.PersistableState State { get; }

		float GetChapterProgressionNormalized();

		float GetPreviousChapterProgressionNormalized();

		void ResetLastCompletedTask();

		void NotEnoughStarsPlayedWasClicked();

		void BrowseTaskStart();

		MapTask GetLastCompletedTask();

		List<MapTask> GetActiveTasks();

		List<MapTask> GetActiveTasks(MapAction.ActionType ofType);

		IEnumerator TryCompletingTaskUsingStars(MapTask task, IEnumerator success, IEnumerator failed, IEnumerator timerStarted);

		IEnumerator TryCompletingTimedTaskOrSkippingUsingCoins(MapTask task, IEnumerator success);

		bool IsTaskTimerInProgress(MapTask task);

		int GetSecondsRemainingInTimedTask(MapTask task);

		IEnumerable<MapTask> IterateTasksFromChapter(int chapterNo);
	}
}
