using System;
using UnityEngine;

public abstract class LevelCollectionEntry : ScriptableObject
{
	public string ContentHash
	{
		get
		{
			return this.contentHash;
		}
	}

	[SerializeField]
	[HideInInspector]
	private string contentHash = string.Empty;
}
