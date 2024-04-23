using System;
using TactileModules.TactilePrefs;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class PersistableStateCache : IPersistableStateHandler
	{
		public PersistableStateCache(ILocalStorageObject<PersistableState> localStorageObject)
		{
			this.localStorageObject = localStorageObject;
			this.persistableState = localStorageObject.Load();
		}

		public PersistableState Get()
		{
			return this.persistableState;
		}

		public void Save()
		{
			this.localStorageObject.Save(this.persistableState);
		}

		private readonly ILocalStorageObject<PersistableState> localStorageObject;

		private readonly PersistableState persistableState;
	}
}
