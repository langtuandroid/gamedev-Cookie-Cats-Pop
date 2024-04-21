using System;

namespace TactileModules.TactilePrefs
{
	public interface ILocalStorageString
	{
		void Save(string data);

		string Load();

		bool Exists();

		void Delete();
	}
}
