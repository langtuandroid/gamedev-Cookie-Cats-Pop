using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

internal class MaxSdkUtils
{
	public static IDictionary<string, string> PropsStringToDict(string str)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (string.IsNullOrEmpty(str))
		{
			return dictionary;
		}
		string[] array = str.Split(new char[]
		{
			'\n'
		});
		foreach (string text in array)
		{
			int num = text.IndexOf('=');
			if (num > 0 && num < text.Length)
			{
				string key = text.Substring(0, num);
				string value = text.Substring(num + 1, text.Length - num - 1);
				if (!dictionary.ContainsKey(key))
				{
					dictionary[key] = value;
				}
			}
		}
		return dictionary;
	}

	public static string DictToPropsString(IDictionary<string, string> dict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (dict != null)
		{
			foreach (KeyValuePair<string, string> keyValuePair in dict)
			{
				if (keyValuePair.Key != null && keyValuePair.Value != null)
				{
					stringBuilder.Append(keyValuePair.Key);
					stringBuilder.Append(MaxSdkUtils._DictKeyValueSeparator);
					stringBuilder.Append(keyValuePair.Value);
					stringBuilder.Append(MaxSdkUtils._DictKeyValuePairSeparator);
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static string ParseColor(Color color)
	{
		int value = (int)(color.a * 255f);
		int value2 = (int)(color.r * 255f);
		int value3 = (int)(color.g * 255f);
		int value4 = (int)(color.b * 255f);
		return BitConverter.ToString(new byte[]
		{
			Convert.ToByte(value),
			Convert.ToByte(value2),
			Convert.ToByte(value3),
			Convert.ToByte(value4)
		}).Replace("-", string.Empty).Insert(0, "#");
	}

	public static bool IsTablet()
	{
		return MaxSdkUtils.MaxUnityPluginClass.CallStatic<bool>("isTablet", new object[0]);
	}

	private static readonly char _DictKeyValueSeparator = '\u001c';

	private static readonly char _DictKeyValuePairSeparator = '\u001d';

	private static readonly AndroidJavaClass MaxUnityPluginClass = new AndroidJavaClass("com.applovin.mediation.unity.MaxUnityPlugin");
}
