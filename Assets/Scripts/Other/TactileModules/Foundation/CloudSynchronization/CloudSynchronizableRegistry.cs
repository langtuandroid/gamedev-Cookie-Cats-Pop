using System;
using System.Collections.Generic;

namespace TactileModules.Foundation.CloudSynchronization
{
	public class CloudSynchronizableRegistry
	{
		public void Register(ICloudSynchronizable synchronizable, int priority)
		{
			CloudSynchronizableRegistryEntry entry = new CloudSynchronizableRegistryEntry
			{
				Synchronizeable = synchronizable,
				Priority = priority
			};
			this.Register(entry);
		}

		public void Register(List<ICloudSynchronizable> cloudSynchronizables)
		{
			int count = cloudSynchronizables.Count;
			for (int i = 0; i < count; i++)
			{
				this.Register(new CloudSynchronizableRegistryEntry
				{
					Synchronizeable = cloudSynchronizables[i],
					Priority = i
				});
			}
		}

		public void Register(CloudSynchronizableRegistryEntry entry)
		{
			this.synchronizables.Add(entry);
		}

		public void SortOnPriority()
		{
			this.synchronizables.Sort();
		}

		public List<CloudSynchronizableRegistryEntry> GetEntries()
		{
			return this.synchronizables;
		}

		private List<CloudSynchronizableRegistryEntry> synchronizables = new List<CloudSynchronizableRegistryEntry>();
	}
}
