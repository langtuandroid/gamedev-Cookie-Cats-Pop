using System;
using System.Collections.Generic;
using TactileModules.Validation;
using UnityEngine;

public class UIProjectSettings : ScriptableObject
{
	public UIProjectSettings.UIProjectSettingsData SettingsData
	{
		get
		{
			if (this.settingsData != null)
			{
				return this.settingsData;
			}
			UIProjectSettings.UIProjectSettingsData uiprojectSettingsData = JsonSerializer.Decode<UIProjectSettings.UIProjectSettingsData>(this.settingsDataSerialized);
			if (uiprojectSettingsData != null)
			{
				this.settingsData = uiprojectSettingsData;
				return uiprojectSettingsData;
			}
			this.settingsData = new UIProjectSettings.UIProjectSettingsData();
			return this.settingsData;
		}
	}

	public static UIProjectSettings Get()
	{
		if (UIProjectSettings.instance == null)
		{
			UIProjectSettings.instance = Resources.Load<UIProjectSettings>(UIProjectSettings.RESOURCE_PATH);
		}
		if (UIProjectSettings.instance == null)
		{
		}
		return UIProjectSettings.instance;
	}

	private static UIProjectSettings instance;

	public static string RESOURCE_PATH = "NinjaUI Project Settings";

	public static string DEFAULT_SHADER = "Unlit/Transparent Colored";

	public Vector2 defaultElementSize = new Vector2(100f, 100f);

	public Vector2 defaultViewSize = new Vector2(640f, 960f);

	public float webPlayerAspectRatio = 1.33333337f;

	public UIAtlas defaultAtlas;

	public UIFont defaultFont;

	[OptionalSerializedField]
	public UIFont secondaryFont;

	[OptionalSerializedField]
	public UIViewLayerAnimation defaultLayerAnimation;

	[Space]
	public List<UIView> viewPrefabs;

	[SerializeField]
	[HideInInspector]
	private string settingsDataSerialized;

	private UIProjectSettings.UIProjectSettingsData settingsData;

	public class UIProjectSettingsData
	{
		public UIProjectSettingsData()
		{
			this.ViewInfosByName = new Dictionary<string, UIProjectSettings.ViewInfo>();
		}

		[JsonSerializable("ViewInfoByName", typeof(UIProjectSettings.ViewInfo))]
		public Dictionary<string, UIProjectSettings.ViewInfo> ViewInfosByName { get; set; }

		public UIProjectSettings.ViewInfo FindInfoWithType(string fullTypeName)
		{
			foreach (UIProjectSettings.ViewInfo viewInfo in this.ViewInfosByName.Values)
			{
				if (viewInfo.FullTypeName == fullTypeName)
				{
					return viewInfo;
				}
			}
			return null;
		}
	}

	public class ViewInfo
	{
		[JsonSerializable("TypeName", null)]
		public string FullTypeName { get; set; }

		[JsonSerializable("AssetPath", null)]
		public string AssetPath { get; set; }
	}
}
