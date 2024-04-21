using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.Timing.Interfaces;
using UnityEngine;

public class TimeStampManager : SingleInstance<TimeStampManager>, ITimingManager
{
	public TimeStampManager()
	{
		this.Load();
		FiberCtrl.Pool.Run(this.UpdateCr(), false);
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string> TimeDone;

	public static TimeStampManager Instance
	{
		get
		{
			return SingleInstance<TimeStampManager>.instance;
		}
	}

	public void CreateTimeStamp(string name, int durationInSeconds)
	{
		TimeStampManager.TimeStamp value = new TimeStampManager.TimeStamp(DateTime.Now, durationInSeconds);
		if (this.TimeStampExist(name))
		{
			this.state.TimeStamps[name] = value;
		}
		else
		{
			this.state.TimeStamps.Add(name, value);
		}
		this.Save();
	}

	public int GetTimePassedInSeconds(string name)
	{
		if (!this.TimeStampExist(name))
		{
			return 0;
		}
		return this.state.TimeStamps[name].GetTimePassedInSeconds();
	}

	public int GetTimeDuration(string name)
	{
		if (!this.TimeStampExist(name))
		{
			return 0;
		}
		return this.state.TimeStamps[name].TimerDuration;
	}

	public int GetTimeLeftInSeconds(string name)
	{
		if (!this.TimeStampExist(name))
		{
			return 0;
		}
		return this.state.TimeStamps[name].GetTimeLeftInSeconds();
	}

	public bool TimeStampExist(string name)
	{
		return this.state.TimeStamps.ContainsKey(name);
	}

	public void RemoveTimeStampIfItExist(string name)
	{
		if (this.TimeStampExist(name))
		{
			this.state.TimeStamps[name].Remove = true;
		}
	}

	private IEnumerator UpdateCr()
	{
		List<string> itemsToHandle = new List<string>();
		for (;;)
		{
			DateTime timeNow = DateTime.Now;
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			if (this.state.TimeStamps.Count > 0)
			{
				itemsToHandle.Clear();
			}
			foreach (KeyValuePair<string, TimeStampManager.TimeStamp> keyValuePair in this.state.TimeStamps)
			{
				if (keyValuePair.Value.Remove)
				{
					itemsToHandle.Add(keyValuePair.Key);
				}
			}
			foreach (string key in itemsToHandle)
			{
				this.state.TimeStamps.Remove(key);
				this.Save();
			}
			TimeSpan span = DateTime.Now - timeNow;
			itemsToHandle.Clear();
			foreach (KeyValuePair<string, TimeStampManager.TimeStamp> keyValuePair2 in this.state.TimeStamps)
			{
				keyValuePair2.Value.InGameSecondsPassed += Math.Max(0.0, span.TotalSeconds);
				this.needSave = true;
				bool done = keyValuePair2.Value.Done;
				keyValuePair2.Value.Update();
				bool done2 = keyValuePair2.Value.Done;
				if (done2 && done != done2)
				{
					itemsToHandle.Add(keyValuePair2.Key);
				}
			}
			foreach (string text in itemsToHandle)
			{
				this.state.TimeStamps[text].Remove = true;
				if (this.TimeDone != null)
				{
					this.TimeDone(text);
				}
			}
		}
		yield break;
	}

	private void Load()
	{
		string securedString = TactilePlayerPrefs.GetSecuredString("PersistedTimeStampManager", string.Empty);
		if (securedString.Length > 0)
		{
			this.state = JsonSerializer.HashtableToObject<TimeStampManager.PersistableState>(securedString.hashtableFromJson());
		}
		else
		{
			this.state = new TimeStampManager.PersistableState();
		}
	}

	public void PerformSaveIfNeeded()
	{
		if (this.needSave)
		{
			this.needSave = false;
			this.Save();
		}
	}

	private void Save()
	{
		if (this.state != null)
		{
			TactilePlayerPrefs.SetSecuredString("PersistedTimeStampManager", JsonSerializer.ObjectToHashtable(this.state).toJson());
		}
		else
		{
			TactilePlayerPrefs.SetSecuredString("PersistedTimeStampManager", string.Empty);
		}
	}

	private const string PREFS_TIME_STAMP_MANAGER = "PersistedTimeStampManager";

	private TimeStampManager.PersistableState state;

	private bool needSave;

	public class PersistableState
	{
		public PersistableState()
		{
			this.TimeStamps = new Dictionary<string, TimeStampManager.TimeStamp>();
		}

		[JsonSerializable("timeStamps", typeof(TimeStampManager.TimeStamp))]
		public Dictionary<string, TimeStampManager.TimeStamp> TimeStamps { get; set; }
	}

	public sealed class TimeStamp
	{
		public TimeStamp()
		{
		}

		public TimeStamp(DateTime ts, int timerDurationInSeconds)
		{
			this.timeStamp = ts;
			this.TimerDuration = timerDurationInSeconds;
			this.Done = false;
			this.Remove = false;
		}

		[JsonSerializable("ts", null)]
		private string PersistTimeStamp
		{
			get
			{
				return this.timeStamp.ToString("yyyy-MM-dd HH:mm:ss");
			}
			set
			{
				this.timeStamp = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
			}
		}

		[JsonSerializable("du", null)]
		public int TimerDuration { get; set; }

		[JsonSerializable("do", null)]
		public bool Done { get; set; }

		[JsonSerializable("sp", null)]
		public double InGameSecondsPassed { get; set; }

		public bool Remove { get; set; }

		public int GetTimePassedInSeconds()
		{
			int val = (int)(DateTime.Now - this.timeStamp).TotalSeconds;
			return Math.Max(val, (int)this.InGameSecondsPassed);
		}

		public int GetTimeLeftInSeconds()
		{
			int timePassedInSeconds = this.GetTimePassedInSeconds();
			return Mathf.Clamp(this.TimerDuration - timePassedInSeconds, 0, this.TimerDuration);
		}

		public void Update()
		{
			if (!this.Done)
			{
				this.Done = (this.GetTimeLeftInSeconds() <= 0);
			}
		}

		private DateTime timeStamp = DateTime.MinValue;
	}
}
