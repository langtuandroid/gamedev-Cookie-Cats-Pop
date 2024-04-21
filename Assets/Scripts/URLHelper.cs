using System;
using System.Collections;
using UnityEngine;

public class URLHelper
{
	public static void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public static void OpenFacebookApp(string facebookAppId)
	{
		URLHelper.OpenURL(string.Format("https://apps.facebook.com/{0}", facebookAppId));
	}

	private static string UrlDecode(string s)
	{
		return Uri.UnescapeDataString(s).Replace('+', ' ');
	}

	public static Hashtable ParseQueryParams(string s)
	{
		Hashtable hashtable = new Hashtable();
		if (s.Contains("?"))
		{
			s = s.Substring(s.IndexOf('?') + 1);
		}
		foreach (string text in s.Split(new char[]
		{
			'&'
		}))
		{
			int num = text.IndexOf("=");
			if (num != -1)
			{
				hashtable.Add(URLHelper.UrlDecode(text.Substring(0, num)), URLHelper.UrlDecode(text.Substring(num + 1)));
			}
			else
			{
				hashtable.Add(URLHelper.UrlDecode(text), string.Empty);
			}
		}
		return hashtable;
	}
}
