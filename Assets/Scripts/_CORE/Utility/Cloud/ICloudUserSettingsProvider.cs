using System;
using System.Collections;

public interface ICloudUserSettingsProvider
{
	bool HasValidUser { get; }

	IEnumerator GetUserSettings(Action<object, CloudUserSettings> callback);

	IEnumerator CreateUserSettings(Hashtable privateSettings, Hashtable publicSettings, Action<object, CloudUserSettings> callback);

	IEnumerator UpdateUserSettings(Hashtable privateSettings, Hashtable publicSettings, int version, Action<object, CloudUserSettings, bool> callback);

	IEnumerator PatchUserSettings(Hashtable objPathsToSet, Hashtable objPathsToUnset, int version, Action<object, CloudUserSettings, bool> callback);

	CloudUserSettings GetCloudSettingsForCloudUser(CloudUser user);
}
