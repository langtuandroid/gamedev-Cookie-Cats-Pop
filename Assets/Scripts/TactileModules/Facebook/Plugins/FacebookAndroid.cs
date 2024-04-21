using System;
using UnityEngine;

namespace TactileModules.Facebook.Plugins
{
	public static class FacebookAndroid
	{
		static FacebookAndroid()
		{
			if (!FacebookAndroid.IsAndroid)
			{
				return;
			}
			//FacebookAndroid.Plugin = AndroidPluginManager.GetPlugin("dk.tactile.facebook.FacebookPlugin");
		}

		private static bool IsAndroid
		{
			get
			{
				return Application.platform == RuntimePlatform.Android;
			}
		}

		public static string GetAttributionId()
		{
			return (!FacebookAndroid.IsAndroid) ? null : FacebookAndroid.Plugin.Call<string>("getAttributionId", new object[0]);
		}

		private static readonly AndroidJavaObject Plugin;
	}
}
