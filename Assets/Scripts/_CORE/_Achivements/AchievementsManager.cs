using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;
using UnityEngine;

public class AchievementsManager : ManagerWithMetaData<string, AchievementAsset>
{
	public AchievementsManager(GameEventManager gameEventManager, AchievementsManager.IAchievementsHelper helper, object gameServiceClientBase)
	{
		this.helper = helper;
		gameEventManager.OnGameEvent += this.HandleGameEvent;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<AchievementAsset> OnAchievementCompleted;

	public void HandleGameEvent(GameEvent e)
	{
		foreach (AchievementAsset definition in base.GetAllMetaData())
		{
			this.EvaluateAchievement(e, definition);
		}
	}

	public int GetTotalUnclaimedAchievements()
	{
		int num = 0;
		foreach (AchievementAsset achievementAsset in base.GetAllMetaData())
		{
			bool flag = this.State.IsClaimed(achievementAsset.Id);
			if (this.IsAchievementCompleted(achievementAsset) && !flag)
			{
				num++;
			}
		}
		return num;
	}

	public AchievementReward GetRewardForAchievement(AchievementAsset achievement)
	{
		AchievementReward reward = ConfigurationManager.Get<AchievementsConfig>().GetReward(achievement.Id);
		if (reward != null)
		{
			return reward;
		}
		return new AchievementReward
		{
			ID = achievement.Id,
			Rewards = new List<ItemAmount>()
		};
	}

	public int GetLevelRequiredForNotification()
	{
		return ConfigurationManager.Get<AchievementsConfig>().LevelRequiredForNotification;
	}

	public void ClearProgressForAchievementsWithTag(string tag)
	{
		foreach (AchievementAsset achievementAsset in base.GetAllMetaData())
		{
			int progress = this.State.GetProgress(achievementAsset.Id);
			if (progress < achievementAsset.objectiveThreshold)
			{
				if (achievementAsset.tag == tag)
				{
					this.State.ClearProgress(achievementAsset.Id);
				}
			}
		}
	}

	private AchievementsManager.PersistableState State
	{
		get
		{
			return UserSettingsManager.Instance.GetSettings<AchievementsManager.PersistableState>();
		}
	}

	private void EvaluateAchievement(GameEvent e, AchievementAsset definition)
	{
		if (definition.objective != e.type)
		{
			return;
		}
		int num = this.State.GetProgress(definition.Id);
		if (num >= definition.objectiveThreshold)
		{
			return;
		}
		if (!definition.AllowProgress(e))
		{
			return;
		}
		switch (definition.thresholdType)
		{
		case AchievementAsset.ThresholdType.NumberOfEvents:
			num++;
			break;
		case AchievementAsset.ThresholdType.ValueAccumulatedInEvents:
			num += e.value;
			break;
		case AchievementAsset.ThresholdType.ValueInSingleEvent:
			num = e.value;
			break;
		case AchievementAsset.ThresholdType.ValueInSingleEventExact:
			num = e.value;
			break;
		}
		this.State.SetProgress(definition.Id, num);
		if (definition.thresholdType == AchievementAsset.ThresholdType.ValueInSingleEventExact)
		{
			if (num == definition.objectiveThreshold)
			{
				this.AchievementCompleted(definition);
			}
		}
		else if (num >= definition.objectiveThreshold)
		{
			this.AchievementCompleted(definition);
		}
	}

	public int GetAchievementProgress(AchievementAsset definition)
	{
		int progress = this.State.GetProgress(definition.Id);
		return Mathf.Min(progress, definition.objectiveThreshold);
	}

	public bool IsAchievementCompleted(AchievementAsset definition)
	{
		int progress = this.State.GetProgress(definition.Id);
		return progress >= definition.objectiveThreshold;
	}

	public bool IsAchievementClaimed(AchievementAsset definition)
	{
		return this.State.IsClaimed(definition.Id);
	}

	public void ClaimAchievement(AchievementAsset mission)
	{
		this.State.SetClaimed(mission.Id);
		this.Save();
	}

	private void AchievementCompleted(AchievementAsset definition)
	{
		this.Save();
		this.notificationFibers.Run(this.helper.QueueNotificationView(definition), true);
		if (this.OnAchievementCompleted != null)
		{
			this.OnAchievementCompleted(definition);
		}
	}

	private void Save()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	protected override string MetaDataAssetFolder
	{
		get
		{
			return "Assets/[Achievements]/Resources/Achievements";
		}
	}

	private FiberRunner notificationFibers = new FiberRunner();

	private AchievementsManager.IAchievementsHelper helper;

	[SettingsProvider("ac", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<AchievementsManager.PersistableState>, IPersistableState
	{
		private PersistableState()
		{
			this.Progressions = new Dictionary<string, int>();
			this.Claimed = new Dictionary<string, bool>();
		}

		[JsonSerializable("prg", typeof(int))]
		public Dictionary<string, int> Progressions { get; set; }

		[JsonSerializable("cla", typeof(bool))]
		public Dictionary<string, bool> Claimed { get; set; }

		public void MergeFromOther(AchievementsManager.PersistableState other, AchievementsManager.PersistableState lastCloudState)
		{
			foreach (KeyValuePair<string, int> keyValuePair in other.Progressions)
			{
				if (!this.Progressions.ContainsKey(keyValuePair.Key))
				{
					this.Progressions.Add(keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					this.Progressions[keyValuePair.Key] = Mathf.Max(keyValuePair.Value, this.Progressions[keyValuePair.Key]);
				}
			}
			foreach (KeyValuePair<string, bool> keyValuePair2 in other.Claimed)
			{
				if (keyValuePair2.Value && !this.Claimed.ContainsKey(keyValuePair2.Key))
				{
					this.Claimed[keyValuePair2.Key] = true;
				}
			}
		}

		public int GetProgress(string achievementId)
		{
			if (!this.Progressions.ContainsKey(achievementId))
			{
				return 0;
			}
			return this.Progressions[achievementId];
		}

		public void SetProgress(string achievementId, int progress)
		{
			this.Progressions[achievementId] = progress;
		}

		public bool IsClaimed(string achievementId)
		{
			bool result = false;
			this.Claimed.TryGetValue(achievementId, out result);
			return result;
		}

		public void SetClaimed(string achievementId)
		{
			this.Claimed[achievementId] = true;
		}

		public void ClearProgress(string achievementId)
		{
			if (this.Progressions.ContainsKey(achievementId))
			{
				this.Progressions.Remove(achievementId);
			}
		}
	}

	public interface IAchievementsHelper
	{
		IEnumerator QueueNotificationView(AchievementAsset definition);
	}
}
