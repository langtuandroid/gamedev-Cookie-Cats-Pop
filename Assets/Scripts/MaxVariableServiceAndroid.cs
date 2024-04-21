using System;
using UnityEngine;

public class MaxVariableServiceAndroid
{
	public static MaxVariableServiceAndroid Instance
	{
		get
		{
			return MaxVariableServiceAndroid._instance;
		}
	}

	public void LoadVariables()
	{
		MaxVariableServiceAndroid._maxUnityPluginClass.CallStatic("loadVariables", new object[0]);
	}

	public bool GetBoolean(string key, bool defaultValue = false)
	{
		return MaxVariableServiceAndroid._maxUnityPluginClass.CallStatic<bool>("getBoolean", new object[]
		{
			key,
			defaultValue
		});
	}

	public string GetString(string key, string defaultValue = "")
	{
		return MaxVariableServiceAndroid._maxUnityPluginClass.CallStatic<string>("getString", new object[]
		{
			key,
			defaultValue
		});
	}

	private static readonly AndroidJavaClass _maxUnityPluginClass = new AndroidJavaClass("com.applovin.mediation.unity.MaxUnityPlugin");

	private static readonly MaxVariableServiceAndroid _instance = new MaxVariableServiceAndroid();
}
