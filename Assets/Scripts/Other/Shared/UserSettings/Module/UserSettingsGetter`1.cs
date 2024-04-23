using System;
using Tactile;
using TactileModules.UserSettings;

namespace Shared.UserSettings.Module
{
	public class UserSettingsGetter<T> : IUserSettingsGetter<T> where T : IPersistableState<T>
	{
		public UserSettingsGetter(IUserSettings userSettings)
		{
			this.userSettings = userSettings;
		}

		public T Get()
		{
			return this.userSettings.GetSettings<T>();
		}

		public T GetFriend(CloudUser cloudUser)
		{
			return this.userSettings.GetFriendSettings<T>(cloudUser);
		}

		private readonly IUserSettings userSettings;
	}
}
