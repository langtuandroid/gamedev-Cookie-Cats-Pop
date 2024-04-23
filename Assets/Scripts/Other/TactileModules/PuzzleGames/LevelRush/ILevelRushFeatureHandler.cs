using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface ILevelRushFeatureHandler : IFeatureTypeHandler<LevelRushInstanceCustomData, FeatureMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureTypeHandler
	{
		LevelRushInstanceCustomData CustomData { get; }
	}
}
