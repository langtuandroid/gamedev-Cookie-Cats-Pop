using System;
using System.IO;
using TactileModules.NativeSharing.Plugins;
using UnityEngine;

namespace TactileModules.NativeSharing
{
	public static class SharingService
	{
		private static string FilePath
		{
			get
			{
				return Application.persistentDataPath + "/NativeSharingPicture.png";
			}
		}

		public static void Share(string text = "", string subject = "", string androidTitle = "", Texture2D texture = null)
		{
			SharingService.WriteTextureFile(texture);
			NativeSharingAndroid.Share(text, subject, androidTitle, SharingService.FilePath);
		}

		private static void WriteTextureFile(Texture2D texture)
		{
			if (texture == null)
			{
				return;
			}
			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(SharingService.FilePath, bytes);
		}
	}
}
