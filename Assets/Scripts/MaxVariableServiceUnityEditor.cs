using System;

public class MaxVariableServiceUnityEditor
{
	public static MaxVariableServiceUnityEditor Instance
	{
		get
		{
			return MaxVariableServiceUnityEditor._instance;
		}
	}

	public void LoadVariables()
	{
	}

	public bool GetBoolean(string key, bool defaultValue = false)
	{
		return defaultValue;
	}

	public string GetString(string key, string defaultValue = "")
	{
		return defaultValue;
	}

	private static readonly MaxVariableServiceUnityEditor _instance = new MaxVariableServiceUnityEditor();
}
