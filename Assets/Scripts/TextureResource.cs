using System;
using UnityEngine;

[Serializable]
public class TextureResource
{
	public Texture2D Texture
	{
		get
		{
			return Resources.Load<Texture2D>(this.path);
		}
	}

	public string path;
}
