using System;

namespace TactileModules.Foundation.CloudSynchronization
{
	public class CloudSynchronizableRegistryEntry : IComparable<CloudSynchronizableRegistryEntry>
	{
		public ICloudSynchronizable Synchronizeable { get; set; }

		public int Priority { get; set; }

		public int CompareTo(CloudSynchronizableRegistryEntry other)
		{
			return this.Priority.CompareTo(other.Priority);
		}
	}
}
