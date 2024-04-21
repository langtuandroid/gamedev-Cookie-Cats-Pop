using System;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.TactilePrefs;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class ReminderCooldown : IReminderCooldown
	{
		public ReminderCooldown(ILocalStorageObject<ReminderTimeStamp> reminderTimeStampStorageObject, IFeatureManager featureManager, IConfigGetter<StoryMapEventConfig> configGetter)
		{
			this.reminderTimeStampStorageObject = reminderTimeStampStorageObject;
			this.featureManager = featureManager;
			this.configGetter = configGetter;
			this.reminderTimeStamp = this.reminderTimeStampStorageObject.Load();
		}

		public bool IsTimeToShow()
		{
			int reminderCooldown = this.configGetter.Get().ReminderCooldown;
			int serverTime = this.featureManager.ServerTime;
			int lastShownTimeStamp = this.reminderTimeStamp.LastShownTimeStamp;
			int num = serverTime - lastShownTimeStamp;
			return num > reminderCooldown;
		}

		public void Reset()
		{
			int serverTime = this.featureManager.ServerTime;
			this.reminderTimeStamp.LastShownTimeStamp = serverTime;
			this.reminderTimeStampStorageObject.Save(this.reminderTimeStamp);
		}

		private ILocalStorageObject<ReminderTimeStamp> reminderTimeStampStorageObject;

		private readonly IFeatureManager featureManager;

		private readonly IConfigGetter<StoryMapEventConfig> configGetter;

		private ReminderTimeStamp reminderTimeStamp;
	}
}
