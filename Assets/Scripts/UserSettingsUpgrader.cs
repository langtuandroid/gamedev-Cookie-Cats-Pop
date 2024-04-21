using System;
using System.Collections;

public static class UserSettingsUpgrader
{
	public static void Upgrade(Hashtable privateSettings, Hashtable publicSettings)
	{
		FeatureManagerDataUpgrader.UpgradeFeatureManager(privateSettings, null);
	}
}
