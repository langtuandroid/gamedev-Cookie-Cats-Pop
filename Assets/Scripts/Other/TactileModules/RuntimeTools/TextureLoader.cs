using System;
using System.IO;
using UnityEngine;

namespace TactileModules.RuntimeTools
{
	public class TextureLoader : ITextureLoader
	{
		public Texture2D LoadTexture(string filePath, bool mipMapsEnabled = false)
		{
			byte[] data = File.ReadAllBytes(filePath);
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, mipMapsEnabled);
			texture2D.LoadImage(data);
			return texture2D;
		}
	}
}
