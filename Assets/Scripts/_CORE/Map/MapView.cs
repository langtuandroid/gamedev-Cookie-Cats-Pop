using System;
using TactileModules.Foundation;

public abstract class MapView : MapViewBase
{
	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}
	
	protected abstract string LevelType { get; }

	protected override bool LastPlayedLevelBelongsToCurrentMap()
	{
		return base.lastLevelPlayedType == this.LevelType;
	}

	protected override LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	protected override MapStreamerCollection MapStreamerCollection
	{
		get
		{
			return ManagerRepository.Get<MapStreamerCollection>();
		}
	}
}
