using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.ScheduledBooster.Data;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public class ScheduledBoosterDefinitions : IScheduledBoosterDefinitions
	{
		public ScheduledBoosterDefinitions(List<ScheduledBoosterDefinition> boosterDefinitions)
		{
			this.boosterDefinitions = boosterDefinitions;
		}

		public ScheduledBoosterDefinition GetDefinition(string scheduledBoosterType)
		{
			foreach (ScheduledBoosterDefinition scheduledBoosterDefinition in this.boosterDefinitions)
			{
				if (scheduledBoosterDefinition.type.ID == scheduledBoosterType)
				{
					return scheduledBoosterDefinition;
				}
			}
			return null;
		}

		private readonly List<ScheduledBoosterDefinition> boosterDefinitions;
	}
}
