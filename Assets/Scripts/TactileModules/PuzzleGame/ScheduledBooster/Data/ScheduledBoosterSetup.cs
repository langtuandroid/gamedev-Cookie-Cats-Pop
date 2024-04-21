using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.ScheduledBooster.Data
{
	[SingletonAssetPath("Assets/[Database]/Resources/ScheduledBooster/ScheduledBoosterSetup.asset")]
	public class ScheduledBoosterSetup : SingletonAsset<ScheduledBoosterSetup>
	{
		public List<ScheduledBoosterDefinition> scheduledBoosterDefinitions;
	}
}
