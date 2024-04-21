using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class TactilePlayerPrefs
{
	public static void Configure(Func<bool> pauseSave)
	{
		TactilePlayerPrefs.pauseSave = pauseSave;
		UnityEngine.Object.DontDestroyOnLoad(new GameObject("TactilePlayerPrefsLifecycle", new Type[]
		{
			typeof(TactilePlayerPrefs.TactilePlayerPrefsLifecycle)
		}));
	}

	public static void Save()
	{
		TactilePlayerPrefs.savePending = true;
	}

	public static void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			PlayerPrefs.Save();
		}
	}

	public static void LateUpdate()
	{
		if (TactilePlayerPrefs.savePending && (TactilePlayerPrefs.pauseSave == null || !TactilePlayerPrefs.pauseSave()))
		{
			PlayerPrefs.Save();
			TactilePlayerPrefs.savePending = false;
		}
	}

	public static string HashName(string name)
	{
		string text;
		if (TactilePlayerPrefs.hashLookup.TryGetValue(name, out text))
		{
			return text;
		}
		MD5 md = MD5.Create();
		byte[] array = md.ComputeHash(Encoding.UTF8.GetBytes(name));
		StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		text = stringBuilder.ToString();
		TactilePlayerPrefs.hashLookup.Add(name, text);
		return text;
	}

	public static void SetBool(string name, bool value)
	{
		PlayerPrefs.SetInt(TactilePlayerPrefs.HashName(name), (!value) ? 0 : 1);
	}

	public static bool GetBool(string name)
	{
		return PlayerPrefs.GetInt(TactilePlayerPrefs.HashName(name)) == 1;
	}

	public static bool GetBool(string name, bool defaultValue)
	{
		return PlayerPrefs.GetInt(TactilePlayerPrefs.HashName(name), (!defaultValue) ? 0 : 1) == 1;
	}

	public static void SetInt(string name, int value)
	{
		PlayerPrefs.SetInt(TactilePlayerPrefs.HashName(name), value);
	}

	public static int GetInt(string name)
	{
		return PlayerPrefs.GetInt(TactilePlayerPrefs.HashName(name));
	}

	public static int GetInt(string name, int defaultValue)
	{
		return PlayerPrefs.GetInt(TactilePlayerPrefs.HashName(name), defaultValue);
	}

	public static void SetFloat(string name, float value)
	{
		PlayerPrefs.SetFloat(TactilePlayerPrefs.HashName(name), value);
	}

	public static float GetFloat(string name)
	{
		return PlayerPrefs.GetFloat(TactilePlayerPrefs.HashName(name));
	}

	public static float GetFloat(string name, float defaultValue)
	{
		return PlayerPrefs.GetFloat(TactilePlayerPrefs.HashName(name), defaultValue);
	}

	public static void SetString(string name, string value)
	{
		PlayerPrefs.SetString(TactilePlayerPrefs.HashName(name), value);
	}

	public static void SetSignedString(string name, string value)
	{
		PlayerPrefs.SetString(TactilePlayerPrefs.HashName(name), value + value.TactilePlayerPrefsHash(name));
	}

	public static void SetSecuredString(string name, string value)
	{
		TactilePlayerPrefs.SetSignedString(name, value);
	}

	public static string GetString(string name)
	{
		return PlayerPrefs.GetString(TactilePlayerPrefs.HashName(name));
	}

	public static string GetString(string name, string defaultValue)
	{
		return PlayerPrefs.GetString(TactilePlayerPrefs.HashName(name), defaultValue);
	}

	public static string GetSignedString(string name, string defaultValue)
	{
		if (TactilePlayerPrefs.HasKey(name))
		{
			string @string = PlayerPrefs.GetString(TactilePlayerPrefs.HashName(name));
			if (@string.Length >= 64)
			{
				string text = @string.Substring(0, @string.Length - 64);
				string a = @string.Substring(@string.Length - 64);
				string b = text.TactilePlayerPrefsHash(name);
				if (a != b)
				{
					PlayerPrefs.DeleteKey(TactilePlayerPrefs.HashName(name));
					return defaultValue;
				}
				return text;
			}
		}
		return defaultValue;
	}

	public static string GetSecuredString(string name, string defaultValue)
	{
		return TactilePlayerPrefs.GetSignedString(name, defaultValue);
	}

	private static bool IsBase64(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if ((s[i] < 'A' || s[i] > 'Z') && (s[i] < 'a' || s[i] > 'z') && (s[i] < '0' || s[i] > '9') && s[i] != '+' && s[i] != '/' && s[i] != '=')
			{
				return false;
			}
		}
		return true;
	}

	private static string BytesToHex(byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
		foreach (byte b in bytes)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private static byte[] HexToBytes(string hex)
	{
		if (hex.Length % 2 == 1)
		{
			return null;
		}
		byte[] array = new byte[hex.Length >> 1];
		for (int i = 0; i < hex.Length >> 1; i++)
		{
			array[i] = (byte)((TactilePlayerPrefs.GetHexVal(hex[i << 1]) << 4) + TactilePlayerPrefs.GetHexVal(hex[(i << 1) + 1]));
		}
		return array;
	}

	private static int GetHexVal(char hex)
	{
		return (int)(hex - ((hex >= ':') ? 'W' : '0'));
	}

	public static string GetEncryptedString(string name, string defaultValue)
	{
		if (TactilePlayerPrefs.HasKey(name))
		{
			string securedString = TactilePlayerPrefs.GetSecuredString(name, string.Empty);
			if (securedString.Length >= 32)
			{
				string s = securedString.Substring(0, securedString.Length - 32);
				if (TactilePlayerPrefs.IsBase64(s))
				{
					byte[] buffer = Convert.FromBase64String(s);
					string hex = securedString.Substring(securedString.Length - 32);
					RijndaelManaged rijndaelManaged = new RijndaelManaged();
					ICryptoTransform transform = rijndaelManaged.CreateDecryptor(name.TactilePlayerPrefsCryptKeyHash(), TactilePlayerPrefs.HexToBytes(hex));
					string result;
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
						{
							using (StreamReader streamReader = new StreamReader(cryptoStream))
							{
								result = streamReader.ReadToEnd();
							}
						}
					}
					return result;
				}
			}
		}
		return defaultValue;
	}

	public static bool HasKey(string name)
	{
		return PlayerPrefs.HasKey(TactilePlayerPrefs.HashName(name));
	}

	public static void DeleteKey(string name)
	{
		PlayerPrefs.DeleteKey(TactilePlayerPrefs.HashName(name));
	}

	private static bool savePending;

	private static Func<bool> pauseSave;

	private static Dictionary<string, string> hashLookup = new Dictionary<string, string>();

	private class TactilePlayerPrefsLifecycle : MonoBehaviour
	{
		private void OnApplicationPause(bool pauseStatus)
		{
			TactilePlayerPrefs.OnApplicationPause(pauseStatus);
		}

		private void LateUpdate()
		{
			TactilePlayerPrefs.LateUpdate();
		}
	}
}
