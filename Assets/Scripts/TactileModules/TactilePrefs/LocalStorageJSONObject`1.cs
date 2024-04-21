using System;

namespace TactileModules.TactilePrefs
{
	public class LocalStorageJSONObject<T> : ILocalStorageObject<T> where T : class, new()
	{
		public LocalStorageJSONObject(ILocalStorageString localStorageString)
		{
			this.localStorageString = localStorageString;
		}

		public void Save(T data)
		{
			string data2 = JsonSerializer.Encode(data);
			this.localStorageString.Save(data2);
		}

		public T Load()
		{
			string json = this.localStorageString.Load();
			T t = JsonSerializer.Decode<T>(json);
			T result;
			if ((result = t) == null)
			{
				result = Activator.CreateInstance<T>();
			}
			return result;
		}

		public bool Exists()
		{
			return this.localStorageString.Exists();
		}

		public void Delete()
		{
			this.localStorageString.Delete();
		}

		private readonly ILocalStorageString localStorageString;
	}
}
