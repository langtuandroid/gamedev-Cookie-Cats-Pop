using System;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class DailyQuestLockedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		int levelRequiredForDailyQuests = ManagerRepository.Get<ConfigurationManager>().GetConfig<DailyQuestConfig>().LevelRequiredForDailyQuests;
		string text = string.Format(L.Get("Daily Quests are locked until level {0}"), levelRequiredForDailyQuests);
		this.description.text = text;
	}

	private void Dismiss(UIEvent e)
	{
		base.Close(0);
	}

	[SerializeField]
	private UILabel description;
}
