using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public interface IScheduledBoosterFactory
	{
		IScheduledBooster Create(ActivatedFeatureInstanceData featureInstanceData);
	}
}
