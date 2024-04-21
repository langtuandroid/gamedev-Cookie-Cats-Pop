using System;
using UnityEngine;

[Serializable]
public class ShopItemMetaData : ScriptableObject
{
	public string Title
	{
		get
		{
			if (string.IsNullOrEmpty(this.title))
			{
				return base.name;
			}
			return this.title;
		}
	}

	public string Description
	{
		get
		{
			if (string.IsNullOrEmpty(this.description))
			{
				return "Description of " + this.Title;
			}
			return this.description;
		}
	}

	public string DescriptionRaw
	{
		get
		{
			return this.description;
		}
	}

	public string ImageSpriteName
	{
		get
		{
			return this.imageSpriteName;
		}
	}

	public const string META_ASSET_FOLDER = "Assets/[ShopItems]/Resources/ShopItemMetaData";

	[SerializeField]
	private string title;

	[SerializeField]
	[Multiline]
	private string description;

	[SerializeField]
	[UISpriteName]
	private string imageSpriteName;
}
