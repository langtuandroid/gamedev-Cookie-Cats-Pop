using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;

public class GateManager
{
	private GateManager(GateManager.IGateDataProvider helper)
	{
		this.helper = helper;
	}

	public bool HasPendingKey { get; set; }

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<LevelProxy> GateUnlocked = delegate (LevelProxy A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<LevelProxy> GateReached = delegate (LevelProxy A_0)
    {
    };





    public static GateManager Instance { get; private set; }

	public static GateManager CreateInstance(GateManager.IGateDataProvider helper)
	{
		GateManager.Instance = new GateManager(helper);
		return GateManager.Instance;
	}

	public bool PlayerOnGate
	{
		get
		{
			return this.GetCurrentGate().IsValid;
		}
	}

	private int farthestUnlockedLevelIndex
	{
		get
		{
			return PuzzleGame.PlayerState.FarthestUnlockedLevelIndex;
		}
	}

	private GateManager.PersistableState GetState()
	{
		return this.helper.GetPersistableState();
	}

	public int CurrentGateKeys
	{
		get
		{
			return this.GetState().NumberOfKeys;
		}
	}

	public int CurrentFriendsGateKeys
	{
		get
		{
			return this.GetState().FacebookIDs.Count;
		}
	}

	public bool CurrentGateComplete
	{
		get
		{
			return this.CurrentGateKeys >= 3;
		}
	}

	public LevelProxy CurrentGateLevel
	{
		get
		{
			int currentGateKeys = this.CurrentGateKeys;
			LevelProxy currentGate = this.GetCurrentGate();
			if (currentGate == LevelProxy.Invalid)
			{
				throw new Exception("Trying to get current gate level when player isn't on a gate.");
			}
			if (currentGate.LevelMetaData is GateMetaData)
			{
				return currentGate.CreateChildProxy(currentGateKeys);
			}
			return LevelProxy.Invalid;
		}
	}

	public LevelProxy GetCurrentGate()
	{
		LevelProxy levelProxy = new LevelProxy(this.helper.GetMainLevelDatabase(), new int[]
		{
			PuzzleGame.PlayerState.FarthestUnlockedLevelIndex
		});
		if (levelProxy.IsValid && levelProxy.LevelMetaData is GateMetaData)
		{
			return levelProxy;
		}
		return LevelProxy.Invalid;
	}

	public int GetCurrentGateIndex()
	{
		return this.helper.GetMainLevelDatabase().GetGateIndex(this.GetCurrentGate().Index);
	}

	public bool IsUserAlreadyAGateKeyGiver(string user)
	{
		return this.GetState().FacebookIDs.Contains(user);
	}

	public void UpdateGate()
	{
		if (PuzzleGame.PlayerState.FarthestUnlockedLevelIndex >= this.helper.GetMainLevelDatabase().NumberOfAvailableLevels)
		{
			return;
		}
		int currentGateIndex = this.GetCurrentGateIndex();
		if (currentGateIndex == -1)
		{
			this.ResetGateState();
			return;
		}
		int gateIdentifier = this.GetState().GateIdentifier;
		if (this.IsFirstVisit(gateIdentifier, currentGateIndex))
		{
			this.StartNewGate(currentGateIndex);
		}
		if (this.GetState().NumberOfKeys >= 3)
		{
			this.SaveGateScore();
		}
	}

	private bool IsFirstVisit(int persistedActiveGateId, int currentGateId)
	{
		return currentGateId != -1 && (persistedActiveGateId == -1 || currentGateId != persistedActiveGateId);
	}

	private void StartNewGate(int currentGateId)
	{
		this.ResetGateState();
		this.GetState().GateIdentifier = currentGateId;
		this.Save();
		this.GateReached(this.GetCurrentGate());
	}

	public bool AddKey(string fromUserID = "")
	{
		if (!this.PlayerOnGate)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(fromUserID) && !this.GetState().FacebookIDs.Contains(fromUserID))
		{
			this.GetState().NumberOfKeys++;
			this.GetState().FacebookIDs.Add(fromUserID);
			this.HasPendingKey = true;
			if (this.CurrentGateKeys >= 3)
			{
				this.SaveGateScore();
			}
			this.Save();
			return true;
		}
		if (string.IsNullOrEmpty(fromUserID))
		{
			this.GetState().NumberOfKeys++;
			this.HasPendingKey = true;
			if (this.CurrentGateKeys >= 3)
			{
				this.SaveGateScore();
			}
			this.Save();
			return true;
		}
		return false;
	}

	public void UnlockGate()
	{
		this.UnlockGateInternal();
	}

	public void UnlockGateWithoutNotification()
	{
		this.UnlockGateInternal();
	}

	private void UnlockGateInternal()
	{
		this.GetState().NumberOfKeys = 3;
		this.SaveGateScore();
		this.Save();
	}

	private void DispatchGateUnlocked()
	{
		this.GateUnlocked(this.GetCurrentGate());
	}

	public void ResetGateState()
	{
		this.GetState().Reset();
		this.Save();
	}

	private void SaveGateScore()
	{
		LevelProxy currentGate = this.GetCurrentGate();
		if (!currentGate.IsValid)
		{
			return;
		}
		ILevelAccomplishment levelData = currentGate.GetLevelData(true);
		levelData.Points = 1337;
		levelData.Stars = 1;
		this.Save();
		this.DispatchGateUnlocked();
	}

	public int SecondsUntilNextPlay
	{
		get
		{
			DateTime utcNow = DateTime.UtcNow;
			return Convert.ToInt32((this.GetState().NextPlay - utcNow).TotalSeconds);
		}
	}

	public bool NextPlayReady
	{
		get
		{
			return this.SecondsUntilNextPlay < 0;
		}
	}

	public void ResetTimer()
	{
		this.GetState().NextPlay = DateTime.UtcNow.AddSeconds((double)this.GetPlayInterval());
		this.helper.SaveUsersettings();
	}

	public void Developer_SetTimerNow()
	{
		this.GetState().NextPlay = DateTime.UtcNow;
		this.Save();
	}

	public string GetFormattedTimeUntilNextPlay()
	{
		float num = (float)this.SecondsUntilNextPlay;
		TimeSpan timeSpan = new TimeSpan(0, 0, (int)num);
		return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	private float GetPlayInterval()
	{
		return (float)this.helper.GetConfig().QuestRegenerationTime;
	}

	private void Save()
	{
		this.helper.SaveUsersettings();
	}

	private const int GATE_COMPLETE_POINTS = 1337;

	private const int GATE_COMPLETE_STARS = 1;

	private GateManager.IGateDataProvider helper;

	[SettingsProvider("gm", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<GateManager.PersistableState>, IPersistableState
	{
		public PersistableState()
		{
			this.Reset();
		}

		[JsonSerializable("inv", typeof(string))]
		public List<string> FacebookIDs { get; set; }

		[JsonSerializable("ke", null)]
		public int NumberOfKeys { get; set; }

		[JsonSerializable("np", null)]
		public DateTime NextPlay { get; set; }

		[JsonSerializable("gi", null)]
		public int GateIdentifier { get; set; }

		public void MergeFromOther(GateManager.PersistableState newState, GateManager.PersistableState lastCloudState)
		{
			if (this.GateIdentifier <= newState.GateIdentifier)
			{
				if (newState.GateIdentifier > this.GateIdentifier)
				{
					this.FacebookIDs = new List<string>(newState.FacebookIDs);
					this.NextPlay = newState.NextPlay;
					this.NumberOfKeys = newState.NumberOfKeys;
					this.GateIdentifier = newState.GateIdentifier;
				}
				else if (newState.GateIdentifier == this.GateIdentifier)
				{
					if (newState.NumberOfKeys > this.NumberOfKeys)
					{
						this.FacebookIDs = new List<string>(newState.FacebookIDs);
						this.NextPlay = newState.NextPlay;
						this.NumberOfKeys = newState.NumberOfKeys;
						this.GateIdentifier = newState.GateIdentifier;
					}
					else if (this.NumberOfKeys >= newState.NumberOfKeys)
					{
					}
				}
			}
		}

		public void Reset()
		{
			this.FacebookIDs = new List<string>();
			this.NextPlay = DateTime.UtcNow;
			this.GateIdentifier = -1;
			this.NumberOfKeys = 0;
		}
	}

	public interface IGateDataProvider
	{
		LevelDatabase GetMainLevelDatabase();

		GateManager.PersistableState GetPersistableState();

		GateConfig GetConfig();

		void SaveUsersettings();
	}
}
