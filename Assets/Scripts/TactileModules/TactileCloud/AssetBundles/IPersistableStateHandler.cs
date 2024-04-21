using System;

namespace TactileModules.TactileCloud.AssetBundles
{
	public interface IPersistableStateHandler
	{
		PersistableState Get();

		void Save();
	}
}
