using System;
using UnityEngine;

[Serializable]
public class TextAssetResource
{
	public TextAsset TextAsset
	{
		get
		{
			return Resources.Load<TextAsset>(this.path);
		}
	}

	public string path;
}
