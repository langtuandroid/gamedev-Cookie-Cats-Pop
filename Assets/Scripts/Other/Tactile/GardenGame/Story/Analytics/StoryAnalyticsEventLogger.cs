using System;
using Tactile.GardenGame.Story.Analytics.Events;
using TactileModules.Analytics.Interfaces;
using UnityEngine;

namespace Tactile.GardenGame.Story.Analytics
{
	public class StoryAnalyticsEventLogger
	{
		public StoryAnalyticsEventLogger(StoryManager storyManager, IStoryMapLevels mainLevelsManager)
		{
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
			this.currentTask = task;
		}

		private void StoryManagerOnTaskEnded(MapTask task)
		{
			task.ActionStarted -= this.TaskOnActionStarted;
			task.ActionEnded -= this.TaskOnActionEnded;
			task.TaskSkippedToAction -= this.OnTaskSkippedToAction;
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
		}

		private void LogAction(MapAction action, bool isStartEvent, object result)
		{
			object obj = this.factoryCollection.TryCreateEventFromAction(this.currentTask, action, isStartEvent, result);
		}

		private readonly StoryManager storyManager;

		private readonly IStoryMapLevels mainLevelsManager;

		private readonly ActionEventFactoryCollection factoryCollection;

		private MapTask currentTask;
	}
}
