using System;

namespace TactileModules.TactilePrefs
{
	public interface ILocalStorageObject<T> where T : class, new()
	{
		void Save(T data);

		bool Exists();

		T Load();

		void Delete();
	}
}
