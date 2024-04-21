using System;

namespace TactileModules.TactileCloud
{
	public interface ICloudClientState
	{
		event Action<int> OnServerTimeUpdated;

		event Action<CloudUser> UserUpdated;

		event Action<CloudUser> UserChanged;

		bool HasValidDevice { get; }

		bool HasValidUser { get; }

		int LastReceivedServerTimeUnixEpocUTC { get; }

		int ClientAdjustedServerTimeUnixEpocUTC { get; }

		CloudDevice CachedDevice { get; }

		CloudUser CachedMe { get; }
	}
}
