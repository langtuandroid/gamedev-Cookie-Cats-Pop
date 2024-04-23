using System;
using Tactile.GardenGame.MapSystem;

namespace Tactile.GardenGame.Story
{
	public class StoryDataMergeHandler
	{
		public StoryDataMergeHandler(UserSettingsManager userSettingsManager, StoryManager storyManager, PropsManager propsManager)
		{
			userSettingsManager.SettingsSynced += this.HandleSettingsSynced;
			this.propsManager = propsManager;
			this.storyManager = storyManager;
		}

		private void HandleSettingsSynced(UserSettingsManager userSettingsManager)
		{
			bool flag = this.storyManager.CheckAndApplyChanges();
			if (flag)
			{
				userSettingsManager.SaveLocalSettings();
			}
			this.propsManager.NotifyPropStateChanged();
		}

		private readonly StoryManager storyManager;

		private readonly PropsManager propsManager;
	}
}
