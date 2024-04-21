using System;
using UnityEngine;

namespace TactileModules.RuntimeTools
{
	public interface ITextureLoader
	{
		Texture2D LoadTexture(string filePath, bool mipMapsEnabled = false);
	}
}
