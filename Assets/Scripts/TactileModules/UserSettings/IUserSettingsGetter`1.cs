using System;

namespace TactileModules.UserSettings
{
	public interface IUserSettingsGetter<T>
	{
		T Get();

		T GetFriend(CloudUser cloudUser);
	}
}
