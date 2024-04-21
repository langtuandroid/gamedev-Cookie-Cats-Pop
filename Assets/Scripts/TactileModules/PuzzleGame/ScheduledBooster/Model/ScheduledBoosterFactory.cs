using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGame.ScheduledBooster.Data;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public class ScheduledBoosterFactory : IScheduledBoosterFactory
	{
		public ScheduledBoosterFactory(IFeatureManager featureManager, IScheduledBoosterDefinitions definitionsUtility)
		{
			this.featureManager = featureManager;
			this.definitionsUtility = definitionsUtility;
		}

		public IScheduledBooster Create(ActivatedFeatureInstanceData featureInstanceData)
		{
			string scheduledBoosterType = featureInstanceData.GetMetaData<ScheduledBoosterMetaData>().ScheduledBoosterType;
			return new ScheduledBooster(this.featureManager, featureInstanceData, this.definitionsUtility.GetDefinition(scheduledBoosterType));
		}

		private readonly IFeatureManager featureManager;

		private readonly IScheduledBoosterDefinitions definitionsUtility;
	}
}
