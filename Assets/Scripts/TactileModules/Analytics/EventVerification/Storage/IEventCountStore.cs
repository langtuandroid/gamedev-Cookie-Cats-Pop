using System;
using System.Collections.Generic;

namespace TactileModules.Analytics.EventVerification.Storage
{
	public interface IEventCountStore
	{
		void Add(EventStorageKeys eventData);

		Dictionary<string, int> GetEvents();

		bool IsEmpty();

		void Save();

		void Load();

		void Clear();
	}
}
