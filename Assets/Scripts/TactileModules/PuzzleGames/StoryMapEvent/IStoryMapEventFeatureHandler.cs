using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IStoryMapEventFeatureHandler : IFeatureTypeHandler<FeatureInstanceCustomData, StoryMapEventMetaData, FeatureTypeCustomData>, IFeatureNotifications, IFeatureAssetBundleHandler, IFeatureTypeHandler
	{
	}
}
