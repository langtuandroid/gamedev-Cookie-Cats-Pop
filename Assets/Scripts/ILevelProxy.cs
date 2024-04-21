using System;

public interface ILevelProxy
{
	int Index { get; }

	string[] AnalyticsDescriptors { get; }

	LevelProxy PreviousLevel { get; }

	LevelProxy NextLevel { get; }

	int HumanNumber { get; }

	LevelCollectionEntry LevelAsset { get; }
}
