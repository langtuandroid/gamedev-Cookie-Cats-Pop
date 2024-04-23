using System;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;

namespace TactileModules.InstallTimeTracking
{
	public static class InstallTimeBuilder
	{
		public static IInstallTime Build()
		{
			TactileDateTime tactileDateTime = new TactileDateTime();
			PlayerPrefsSignedString installTimeStore = new PlayerPrefsSignedString(string.Empty, "InstallTimeTrackingFirstInstallTime");
			PlayerPrefsSignedString playerPrefsSignedString = new PlayerPrefsSignedString(string.Empty, "FeatureManagerFirstInstallTime");
			PlayerPrefsSignedString playerPrefsSignedString2 = new PlayerPrefsSignedString(string.Empty, "AdProviderManagerFirstInstallTime");
			return new InstallTime(tactileDateTime, installTimeStore, new ILocalStorageString[]
			{
				playerPrefsSignedString,
				playerPrefsSignedString2
			});
		}
	}
}
