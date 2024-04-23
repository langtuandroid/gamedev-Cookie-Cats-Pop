using System;
using System.Collections.Generic;
using Tactile;
using Tactile.GardenGame.Story;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class CurrencyAvailability : ICurrencyAvailability
	{
		public CurrencyAvailability(IMainProgression mainProgression, StoryMapEventActivation featureActivation, IStoryManager storyManager, InventoryManager inventoryManager)
		{
			this.mainProgression = mainProgression;
			this.featureActivation = featureActivation;
			this.storyManager = storyManager;
			this.inventoryManager = inventoryManager;
		}

		public bool HasMainProgressionLevelsToPlay
		{
			get
			{
				return this.mainProgression.GetFarthestUnlockedLevelIndex() < this.mainProgression.MaxAvailableLevel;
			}
		}

		public int MinimumLevelIndex
		{
			get
			{
				return this.mainProgression.GetFarthestUnlockedLevelIndex();
			}
		}

		private int EndOfContentMinimumLevelIndex
		{
			get
			{
				return this.mainProgression.MaxAvailableLevel - 100;
			}
		}

		public int GetRandomEndOfContentLevelIndex()
		{
			bool flag = false;
			int num = -1;
			while (!flag)
			{
				num = UnityEngine.Random.Range(this.EndOfContentMinimumLevelIndex, this.mainProgression.MaxAvailableLevel);
				LevelProxy level = this.mainProgression.GetDatabase().GetLevel(num);
				PuzzleLevel puzzleLevel = level.LevelAsset as PuzzleLevel;
				if (!(puzzleLevel == null))
				{
					flag = (puzzleLevel.TutorialSteps.Count<ITutorialStep>() == 0);
				}
			}
			return num;
		}

		public bool ShouldLevelAwardStoryCurrency(int levelIndex)
		{
			return this.featureActivation.HasActiveFeature() && this.storyManager.FirstIntroCompleted && this.GetCurrencyLeftToEarn() != 0 && ((!this.HasMainProgressionLevelsToPlay && levelIndex >= this.EndOfContentMinimumLevelIndex) || levelIndex >= this.MinimumLevelIndex);
		}

		public int GetCurrencyLeftToEarn()
		{
			int num = 0;
			foreach (MapTask mapTask in this.GetAllIncompleteTasks())
			{
				num += mapTask.StarsRequired;
			}
			int amount = this.inventoryManager.GetAmount("Star");
			return Mathf.Clamp(num - amount, 0, int.MaxValue);
		}

		private IEnumerable<MapTask> GetAllIncompleteTasks()
		{
			StoryManager.PersistableState storyState = (this.storyManager as StoryManager).State;
			for (int i = 1; i <= 5; i++)
			{
				foreach (MapTask task in this.storyManager.IterateTasksFromChapter(i))
				{
					StoryManager.PersistableState.TaskState taskState = storyState.GetTaskState(task.ID);
					if (taskState != StoryManager.PersistableState.TaskState.Completed)
					{
						yield return task;
					}
				}
			}
			yield break;
		}

		private const int MAX_CHAPTER_NUMBER = 5;

		private const int MINIMUM_LEVEL_RANGE = 100;

		private readonly IMainProgression mainProgression;

		private readonly StoryMapEventActivation featureActivation;

		private readonly IStoryManager storyManager;

		private readonly InventoryManager inventoryManager;
	}
}
