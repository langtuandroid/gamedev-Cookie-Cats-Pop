using System;
using System.Collections.Generic;
using OneSignalPush.MiniJSON;

internal class OneSignalPlatformHelper
{
	internal static OSPermissionSubscriptionState parsePermissionSubscriptionState(OneSignalPlatform platform, string jsonStr)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(jsonStr) as Dictionary<string, object>;
		OSPermissionSubscriptionState ospermissionSubscriptionState = new OSPermissionSubscriptionState();
		ospermissionSubscriptionState.permissionStatus = platform.parseOSPermissionState(dictionary["permissionStatus"]);
		ospermissionSubscriptionState.subscriptionStatus = platform.parseOSSubscriptionState(dictionary["subscriptionStatus"]);
		if (dictionary.ContainsKey("emailSubscriptionStatus"))
		{
			ospermissionSubscriptionState.emailSubscriptionStatus = platform.parseOSEmailSubscriptionState(dictionary["emailSubscriptionStatus"]);
		}
		return ospermissionSubscriptionState;
	}

	internal static OSPermissionStateChanges parseOSPermissionStateChanges(OneSignalPlatform platform, string stateChangesJSONString)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(stateChangesJSONString) as Dictionary<string, object>;
		return new OSPermissionStateChanges
		{
			to = platform.parseOSPermissionState(dictionary["to"]),
			from = platform.parseOSPermissionState(dictionary["from"])
		};
	}

	internal static OSSubscriptionStateChanges parseOSSubscriptionStateChanges(OneSignalPlatform platform, string stateChangesJSONString)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(stateChangesJSONString) as Dictionary<string, object>;
		return new OSSubscriptionStateChanges
		{
			to = platform.parseOSSubscriptionState(dictionary["to"]),
			from = platform.parseOSSubscriptionState(dictionary["from"])
		};
	}

	internal static OSEmailSubscriptionStateChanges parseOSEmailSubscriptionStateChanges(OneSignalPlatform platform, string stateChangesJSONString)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(stateChangesJSONString) as Dictionary<string, object>;
		return new OSEmailSubscriptionStateChanges
		{
			to = platform.parseOSEmailSubscriptionState(dictionary["to"]),
			from = platform.parseOSEmailSubscriptionState(dictionary["from"])
		};
	}
}
