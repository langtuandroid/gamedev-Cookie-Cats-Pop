using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.PuzzleGame.MainLevels;
using UnityEngine;

public class VipManager
{
	private VipManager()
	{
		new VipReminderPopup();
		new VipUnlockedPopup();
	}

	public static VipManager Instance { get; private set; }

	public static VipManager CreateInstance()
	{
		VipManager.Instance = new VipManager();
		return VipManager.Instance;
	}

	private VipProgramConfig Config
	{
		get
		{
			return ConfigurationManager.Get<VipProgramConfig>();
		}
	}

	private void OnVipStateChanged()
	{
		if (this.VipStateChanged != null)
		{
			this.VipStateChanged(this.UserIsVip());
		}
	}

	private int GetNextBonusDayId()
	{
		return this.State.LatestConsumedBonusDayId + 1;
	}

	private int GetBonusId(int dayId)
	{
		int value = dayId - this.State.StartDayId;
		return Mathf.Clamp(value, 0, 30);
	}

	public int GetDaysLeft()
	{
		int num = this.State.LatestConsumedBonusDayId - this.State.StartDayId;
		return Mathf.Clamp(30 - num, 0, 30);
	}

	public int GetSecondsLeftUntilNextBonus()
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime d = DateHelper.GetLocalTimeFromDayID(this.GetNextBonusDayId()).ToUniversalTime();
		return (int)(d - utcNow).TotalSeconds;
	}

	public string GetSecondsLeftUntilNextBonusStr()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.GetSecondsLeftUntilNextBonus());
		if (this.GetSecondsLeftUntilNextBonus() < 86400)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", new object[]
		{
			timeSpan.Days,
			timeSpan.Hours,
			timeSpan.Minutes,
			timeSpan.Seconds
		});
	}

	public bool IsBonusPending()
	{
		return this.UserIsVip() && (this.instantBonusPending || DateHelper.GetDayID() >= this.GetNextBonusDayId());
	}

	public int GetNumberOfPendingBonuses()
	{
		int bonusId = this.GetBonusId(this.State.LatestConsumedBonusDayId);
		int bonusId2 = this.GetBonusId(DateHelper.GetDayID());
		int num = bonusId2 - bonusId;
		return num + ((!this.instantBonusPending) ? 0 : 1);
	}

	public IEnumerable<ItemAmount> PendingBonus()
	{
		if (!this.IsBonusPending())
		{
			yield break;
		}
		if (this.instantBonusPending)
		{
			foreach (ItemAmount reward in this.Config.StartReward.Rewards)
			{
				yield return reward;
			}
		}
		int bonusIdLastTimeConsuming = this.GetBonusId(this.State.LatestConsumedBonusDayId);
		int bonusId = this.GetBonusId(DateHelper.GetDayID());
		for (int i = bonusIdLastTimeConsuming; i < bonusId; i++)
		{
			int dayId = i % this.Config.DailyRewards.Count;
			VipProgramConfigDailyReward dailyRewards = this.Config.DailyRewards[dayId];
			foreach (ItemAmount reward2 in dailyRewards.Rewards)
			{
				yield return reward2;
			}
		}
		yield break;
	}

	public void ConsumePendingBonus()
	{
		this.State.LatestConsumedBonusDayId = DateHelper.GetDayID();
		this.instantBonusPending = false;
		if (this.GetDaysLeft() <= 0)
		{
			this.Reset();
			this.OnVipStateChanged();
		}
		this.Save();
		this.BonusClaimed();
	}

	public void DeveloperReset()
	{
		this.Reset();
	}

	private void Reset()
	{
		this.State.Reset();
		this.Save();
	}

	private void Save()
	{
		UserSettingsManager.Get<PublicUserSettings>().UserIsVip = this.UserIsVip();
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	private VipManager.PersistableState State
	{
		get
		{
			return UserSettingsManager.Instance.GetSettings<VipManager.PersistableState>();
		}
	}

	public bool UserIsVip()
	{
		return this.State.StartDayId != -1;
	}

	public bool IsInUse()
	{
		return this.UserIsVip() || this.Config.LevelRequiredForVip > 0;
	}

	public bool IsVipProgramUnlocked()
	{
		return this.IsInUse() && MainProgressionManager.Instance.GetFarthestUnlockedLevelHumanNumber() >= this.Config.LevelRequiredForVip;
	}

	public void UserBoughtVipSubscription()
	{
		this.Reset();
		this.instantBonusPending = true;
		this.State.StartDayId = DateHelper.GetDayID();
		this.State.LatestConsumedBonusDayId = DateHelper.GetDayID();
		this.OnVipStateChanged();
		UserSettingsManager.Instance.SyncUserSettings();
		this.Save();
	}

	public string DebugGetStateInfo()
	{
		return this.State.GetDebugInfo();
	}

	private const int ONE_DAY_IN_SECONDS = 86400;

	private const int DAY_ID_NOT_DEFINED = -1;

	public const int TOTAL_SUBSCRIPTION_DAYS = 30;

	private bool instantBonusPending;

	public Action<bool> VipStateChanged;

	public Action BonusClaimed = delegate()
	{
	};

	[SettingsProvider("vi", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<VipManager.PersistableState>, IPersistableState
	{
		public PersistableState()
		{
			this.Reset();
		}

		[JsonSerializable("lcrdi", null)]
		public int LatestConsumedBonusDayId { get; set; }

		[JsonSerializable("sdi", null)]
		public int StartDayId { get; set; }

		public void MergeFromOther(VipManager.PersistableState otherState, VipManager.PersistableState lastCloudState)
		{
			if (otherState.StartDayId > this.StartDayId)
			{
				this.StartDayId = otherState.StartDayId;
				this.LatestConsumedBonusDayId = otherState.LatestConsumedBonusDayId;
			}
		}

		public string GetDebugInfo()
		{
			return string.Concat(new object[]
			{
				"StartDay: ",
				this.StartDayId,
				" Latest: ",
				this.LatestConsumedBonusDayId
			});
		}

		public void Reset()
		{
			this.LatestConsumedBonusDayId = -1;
			this.StartDayId = -1;
		}
	}
}
