using System;
using UnityEngine;

[Serializable]
public class AchievementAsset : ScriptableObject
{
	public string Id
	{
		get
		{
			return base.name;
		}
	}

	public string PlatformAchievementId
	{
		get
		{
			return this.Id;
		}
	}

	public string FormattedTitle
	{
		get
		{
			string format = L.Get(this.titleKey);
			return string.Format(format, L.FormatNumber(this.objectiveThreshold));
		}
	}

	public string FormattedDescription
	{
		get
		{
			int number = this.objectiveThreshold;
			string format = L.Get(this.descriptionKey);
			return string.Format(format, L.FormatNumber(number));
		}
	}

	public virtual bool AllowProgress(GameEvent e)
	{
		return true;
	}

	public const string META_ASSET_FOLDER = "Assets/[Achievements]/Resources/Achievements";

	[HideInInspector]
	public string googlePlayId = "<Insert GooglePlay ID>";

	[HideInInspector]
	public string titleKey = "<Title>";

	[HideInInspector]
	public string descriptionKey = "<Description>";

	[HideInInspector]
	public string descriptionPostKey = string.Empty;

	[HideInInspector]
	public int objective;

	[HideInInspector]
	public int objectiveThreshold;

	[HideInInspector]
	public AchievementAsset.ThresholdType thresholdType;

	[HideInInspector]
	public string tag;

	public enum ThresholdType
	{
		NumberOfEvents,
		ValueAccumulatedInEvents,
		ValueInSingleEvent,
		ValueInSingleEventExact
	}
}
