using System;
using System.Collections.Generic;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;

namespace TactileModules.InstallTimeTracking
{
	public class InstallTime : IInstallTime
	{
		public InstallTime(ITactileDateTime tactileDateTime, ILocalStorageString installTimeStore, params ILocalStorageString[] oldInstallTimeStores)
		{
			this.tactileDateTime = tactileDateTime;
			this.installTimeStore = installTimeStore;
			this.oldInstallTimeStores = oldInstallTimeStores;
			string value = installTimeStore.Load();
			if (string.IsNullOrEmpty(value))
			{
				this.SetFirstInstallTime();
			}
		}

		private void SetFirstInstallTime()
		{
			List<DateTime> list = new List<DateTime>();
			foreach (ILocalStorageString localStorageString in this.oldInstallTimeStores)
			{
				string text = localStorageString.Load();
				if (!string.IsNullOrEmpty(text))
				{
					DateTime item = DateTime.ParseExact(text, "yyyy-MM-dd HH:mm:ss", null);
					list.Add(item);
				}
			}
			DateTime utcNow = this.tactileDateTime.UtcNow;
			list.Add(utcNow);
			list.Sort((DateTime a, DateTime b) => (int)(a - b).TotalSeconds);
			string data = list[0].ToString("yyyy-MM-dd HH:mm:ss");
			this.installTimeStore.Save(data);
		}

		public int GetSecondsSinceFirstInstall()
		{
			string s = this.installTimeStore.Load();
			DateTime d = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss", null);
			DateTime utcNow = this.tactileDateTime.UtcNow;
			return (int)(utcNow - d).TotalSeconds;
		}

		private readonly ITactileDateTime tactileDateTime;

		private readonly ILocalStorageString installTimeStore;

		private readonly ILocalStorageString[] oldInstallTimeStores;

		public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
	}
}
