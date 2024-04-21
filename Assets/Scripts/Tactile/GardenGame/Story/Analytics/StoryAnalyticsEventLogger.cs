using System;
using Tactile.GardenGame.Story.Analytics.Events;
using TactileModules.Analytics.Interfaces;
using UnityEngine;

namespace Tactile.GardenGame.Story.Analytics
{
	public class StoryAnalyticsEventLogger
	{
		public StoryAnalyticsEventLogger(IAnalytics analytics, StoryManager storyManager, IStoryMapLevels mainLevelsManager)
		{
			this.analytics = analytics;
			this.storyManager = storyManager;
			this.mainLevelsManager = mainLevelsManager;
			this.factoryCollection = new ActionEventFactoryCollection();
			this.RegisterEvents();
		}

		private void RegisterEvents()
		{
			this.storyManager.TaskStarted += this.StoryManagerOnTaskStarted;
			this.storyManager.TaskEnded += this.StoryManagerOnTaskEnded;
		}

		private void StoryManagerOnTaskStarted(MapTask task)
		{
			task.ActionStarted += this.TaskOnActionStarted;
			task.ActionEnded += this.TaskOnActionEnded;
			task.TaskSkippedToAction += this.OnTaskSkippedToAction;
			this.analytics.LogEvent(new TaskStartedEvent(task, this.storyManager.CurrentChapter, this.mainLevelsManager.NumberOfAvailableLevels), -1.0, null);
			this.currentTask = task;
		}

		private void StoryManagerOnTaskEnded(MapTask task)
		{
			task.ActionStarted -= this.TaskOnActionStarted;
			task.ActionEnded -= this.TaskOnActionEnded;
			task.TaskSkippedToAction -= this.OnTaskSkippedToAction;
			this.analytics.LogEvent(new TaskEndedEvent(task, this.storyManager.CurrentChapter, this.mainLevelsManager.NumberOfAvailableLevels, task.SkippedTask), -1.0, null);
		}

		private void TaskOnActionStarted(MapAction action)
		{
			this.LogAction(action, true, null);
		}

		private void TaskOnActionEnded(MapAction action, object result)
		{
			this.LogAction(action, false, result);
		}

		private void OnTaskSkippedToAction(MapTask task, MapAction action, bool isLastAction)
		{
			string skipTo = (!isLastAction) ? action.GetType().Name : "MapActionLast";
			this.analytics.LogEvent(new TaskSkippedEvent(task, this.storyManager.CurrentChapter, this.mainLevelsManager.NumberOfAvailableLevels, skipTo), -1.0, null);
		}

		private void LogAction(MapAction action, bool isStartEvent, object result)
		{
			object obj = this.factoryCollection.TryCreateEventFromAction(this.currentTask, action, isStartEvent, result);
			if (obj != null)
			{
				this.analytics.LogEvent(obj, -1.0, null);
			}
		}

		private readonly IAnalytics analytics;

		private readonly StoryManager storyManager;

		private readonly IStoryMapLevels mainLevelsManager;

		private readonly ActionEventFactoryCollection factoryCollection;

		private MapTask currentTask;
	}
}
