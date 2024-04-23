using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class PropsManager
	{
		public PropsManager(UserSettingsManager userSettingsManager)
		{
			this.userSettingsManager = userSettingsManager;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action PropStateChanged;



		private PropsManager.PersistableState State
		{
			get
			{
				return this.userSettingsManager.GetSettings<PropsManager.PersistableState>();
			}
		}

		public void SetPropSkin(string mapObjectId, int skin)
		{
			this.SetPropSkinNoSave(mapObjectId, skin);
			this.Save();
		}

		public bool SetPropSkinNoSave(string mapObjectId, int skin)
		{
			int propSkin = this.GetPropSkin(mapObjectId);
			if (propSkin == skin)
			{
				return false;
			}
			this.State.SetPropSkin(mapObjectId, skin);
			return true;
		}

		public int GetPropSkin(string mapObjectId)
		{
			if (this.State.PropSkins.ContainsKey(mapObjectId))
			{
				return this.State.PropSkins[mapObjectId];
			}
			return -1;
		}

		public void Save()
		{
			this.userSettingsManager.SaveLocalSettings();
		}

		public void Clear()
		{
			this.State.PropSkins.Clear();
			this.State.PropChangeTimestamps.Clear();
		}

		public void NotifyPropStateChanged()
		{
			this.PropStateChanged();
		}

		public bool TryRemoveEmptyKeys()
		{
			bool result = false;
			if (this.State.PropSkins.ContainsKey(string.Empty))
			{
				this.State.PropSkins.Remove(string.Empty);
				result = true;
			}
			if (this.State.PropChangeTimestamps.ContainsKey(string.Empty))
			{
				this.State.PropChangeTimestamps.Remove(string.Empty);
				result = true;
			}
			return result;
		}

		private readonly UserSettingsManager userSettingsManager;

		[SettingsProvider("props", false, new Type[]
		{

		})]
		public class PersistableState : IPersistableState<PropsManager.PersistableState>, IPersistableState
		{
			private PersistableState()
			{
				this.PropSkins = new Dictionary<string, int>();
				this.PropChangeTimestamps = new Dictionary<string, DateTime>();
			}

			public void SetPropSkin(string mapObjectId, int skin)
			{
				if (string.IsNullOrEmpty(mapObjectId))
				{
					UnityEngine.Debug.LogError("mapObjectId not allowed to be empty or null");
					return;
				}
				this.PropSkins[mapObjectId] = skin;
				this.PropChangeTimestamps[mapObjectId] = DateTime.UtcNow;
			}

			public void MergeFromOther(PropsManager.PersistableState newest, PropsManager.PersistableState last)
			{
				foreach (KeyValuePair<string, int> keyValuePair in newest.PropSkins)
				{
					string key = keyValuePair.Key;
					bool flag = newest.PropChangeTimestamps.ContainsKey(key);
					bool flag2 = this.PropChangeTimestamps.ContainsKey(key);
					bool flag3 = flag && (!flag2 || newest.PropChangeTimestamps[key] > this.PropChangeTimestamps[key]);
					bool flag4 = !this.PropSkins.ContainsKey(key) || flag3 || (!flag && !flag2);
					if (flag4)
					{
						this.PropSkins[key] = keyValuePair.Value;
						DateTime value = (!flag) ? DateTime.UtcNow : newest.PropChangeTimestamps[key];
						this.PropChangeTimestamps[key] = value;
					}
				}
			}

			[JsonSerializable("sk", typeof(int))]
			public Dictionary<string, int> PropSkins { get; set; }

			[JsonSerializable("ti", typeof(DateTime))]
			public Dictionary<string, DateTime> PropChangeTimestamps { get; set; }
		}
	}
}
