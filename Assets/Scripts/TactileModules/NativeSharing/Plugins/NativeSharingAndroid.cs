using System;
using UnityEngine;

namespace TactileModules.NativeSharing.Plugins
{
	public static class NativeSharingAndroid
	{
		static NativeSharingAndroid()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			//NativeSharingAndroid.plugin = AndroidPluginManager.GetPlugin("dk.tactile.nativesharing.NativeSharingPlugin");
		}

		public static void Share(string text, string subject, string androidTitle, string filePath)
		{
			NativeSharingAndroid.plugin.Call("share", new object[]
			{
				text,
				subject,
				androidTitle,
				filePath
			});
		}

		private static readonly AndroidJavaObject plugin;
	}
}
