using System;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class LevelSessionStats
{
	public LevelSessionStats(LevelSession session)
	{
		this.session = session;
		this.timeStarted = Time.time;
		this.MovesLeftBeforeAftermath = session.BallQueue.BallsLeft;
		this.FreebieType = null;
		this.MovesAddedByGamePiece = 0;
		GameEventManager.Instance.OnGameEvent += this.HandleGameEvent;
		session.TurnLogic.ShotFired += this.HandleShotFired;
		InventoryManager.Instance.InventoryChanged += this.HandleInventoryChanged;
	}

	public int BoosterContinue { get; private set; }

	public int BoosterContinueSpecial { get; private set; }

	public int BoosterContinueAfterDeath { get; private set; }

	public int BoosterContinueAfterDeathWithBooster { get; private set; }

	public bool FreebiePowercatUsed { get; private set; }

	public bool FreebieExtraMovesUsed { get; private set; }

	public bool FreebieCatVision { get; private set; }

	public bool FreebieTripleSwap { get; private set; }

	public bool FreebiePaid { get; private set; }

	public string FreebieType { get; private set; }

	public bool FreeBeeVideosWatched { get; private set; }

	public int BonusDropsCollected { get; private set; }

	public int MovesAddedByGamePiece { get; set; }

	public int MovesLeftBeforeAftermath { get; private set; }

	public int StarsAchieved
	{
		get
		{
			if (this.session.Points >= this.session.AdjustedStarThresholds[2])
			{
				return 3;
			}
			if (this.session.Points >= this.session.AdjustedStarThresholds[1])
			{
				return 2;
			}
			return 1;
		}
	}

	public float GetSecondsPlayed()
	{
		return this.timeEnded - this.timeStarted;
	}

	public int GetTimesPregameBoostersWasUsed()
	{
		int num = 0;
		InventoryItem[] array = new InventoryItem[]
		{
			"BoosterShield",
			"BoosterSuperAim",
			"BoosterSuperQueue"
		};
		foreach (InventoryItem boosterType in array)
		{
			num += this.GetTimesBoosterWasUsed(boosterType);
		}
		return num;
	}

	public int GetTimesIngameBoostersWasUsed()
	{
		int num = 0;
		InventoryItem[] array = new InventoryItem[]
		{
			"BoosterRainbow",
			"BoosterFinalPower"
		};
		foreach (InventoryItem boosterType in array)
		{
			num += this.GetTimesBoosterWasUsed(boosterType);
		}
		return num;
	}

	public int GetTimesBoosterWasUsed(InventoryItem boosterType)
	{
		if (this.boostersUsed.ContainsKey(boosterType))
		{
			return this.boostersUsed[boosterType];
		}
		return 0;
	}

	public int GetTimesPowerCombosUsed(int withNumCats)
	{
		if (this.powerCombinationsUsed.ContainsKey(withNumCats))
		{
			return this.powerCombinationsUsed[withNumCats];
		}
		return 0;
	}

	private void HandleGameEvent(GameEvent obj)
	{
		if (obj.type == 31)
		{
			this.FreeBeeVideosWatched = true;
		}
		if (obj.type == 41)
		{
			this.FreebieType = obj.context.GetType().ToString();
			if (obj.context is FreebieExtraMovesLogic)
			{
				this.FreebieExtraMovesUsed = true;
			}
			else if (obj.context is CookieJarBoosterLogic)
			{
				this.FreebiePowercatUsed = true;
			}
			else if (obj.context is SuperAimBoosterLogic)
			{
				this.FreebieCatVision = true;
			}
			else if (obj.context is TripleQueueBoosterLogic)
			{
				this.FreebieTripleSwap = true;
			}
			this.FreebiePaid = (obj.value == 0);
		}
	}

	private void HandleShotFired(TurnLogic.Shot[] shots, ResolveState resolveState)
	{
		int num = 0;
		num += ((!this.session.Powers.CurrentCombination.IsColorEnabled(PowerColor.Blue)) ? 0 : 1);
		num += ((!this.session.Powers.CurrentCombination.IsColorEnabled(PowerColor.Red)) ? 0 : 1);
		num += ((!this.session.Powers.CurrentCombination.IsColorEnabled(PowerColor.Yellow)) ? 0 : 1);
		num += ((!this.session.Powers.CurrentCombination.IsColorEnabled(PowerColor.Green)) ? 0 : 1);
		if (!this.powerCombinationsUsed.ContainsKey(num))
		{
			this.powerCombinationsUsed[num] = 1;
		}
		else
		{
			Dictionary<int, int> dictionary;
			int key;
			(dictionary = this.powerCombinationsUsed)[key = num] = dictionary[key] + 1;
		}
	}

	private void HandleInventoryChanged(InventoryManager.ItemChangeInfo info)
	{
		if (info.ChangeByAmount < 0 && BoosterManagerBase<BoosterManager>.Instance.IsInventoryItemABooster(info.Item))
		{
			this.AddBoosterUsage(info.Item, Mathf.Abs(info.ChangeByAmount));
		}
	}

	private void AddBoosterUsage(InventoryItem item, int amount)
	{
		if (!this.boostersUsed.ContainsKey(item))
		{
			this.boostersUsed[item] = 0;
		}
		Dictionary<InventoryItem, int> dictionary;
		(dictionary = this.boostersUsed)[item] = dictionary[item] + amount;
	}

	public void End()
	{
		GameEventManager.Instance.OnGameEvent -= this.HandleGameEvent;
		this.session.TurnLogic.ShotFired -= this.HandleShotFired;
		InventoryManager.Instance.InventoryChanged -= this.HandleInventoryChanged;
	}

	public void MarkEndValues()
	{
		this.timeEnded = Time.time;
	}

	public void MarkEndValuesPreAftermath()
	{
		this.MovesLeftBeforeAftermath = this.session.BallQueue.BallsLeft;
	}

	public void IncrementBonusDropsCollected()
	{
		this.BonusDropsCollected++;
	}

	public void IncrementContinueUsed()
	{
		this.BoosterContinue++;
	}

	public void IncrementSpecialContinueUsed()
	{
		this.BoosterContinueSpecial++;
	}

	public void IncrementContinueAfterDeathUsed()
	{
		this.BoosterContinueAfterDeath++;
	}

	public void IncrementConinueAfterDeathWithShieldUsed()
	{
		this.BoosterContinueAfterDeathWithBooster++;
	}

	public PowerComboStats GetPowercomboStats()
	{
		if (this.powerComboStats == null)
		{
			this.powerComboStats = new PowerComboStats();
		}
		return this.powerComboStats;
	}

	public void ClearPowerComboStats()
	{
		this.powerComboStats = null;
	}

	private readonly Dictionary<InventoryItem, int> boostersUsed = new Dictionary<InventoryItem, int>();

	private readonly Dictionary<int, int> powerCombinationsUsed = new Dictionary<int, int>();

	private readonly float timeStarted;

	private readonly LevelSession session;

	private float timeEnded;

	private PowerComboStats powerComboStats;
}
