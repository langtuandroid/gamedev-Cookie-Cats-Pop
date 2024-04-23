using System;

namespace TactileModules.TactilePrefs
{
	public class PlayerPrefsUnsignedString : ILocalStorageString
	{
		public PlayerPrefsUnsignedString(string domainNamespace, string key)
		{
			this.key = domainNamespace + key;
		}

		public void Save(string data)
		{
			TactilePlayerPrefs.SetString(this.key, data);
			TactilePlayerPrefs.Save();
		}

		public string Load()
		{
			return TactilePlayerPrefs.GetString(this.key, string.Empty);
		}

		public bool Exists()
		{
			return TactilePlayerPrefs.HasKey(this.key);
		}

		public void Delete()
		{
			TactilePlayerPrefs.DeleteKey(this.key);
			TactilePlayerPrefs.Save();
		}

		private readonly string key;
	}
}
