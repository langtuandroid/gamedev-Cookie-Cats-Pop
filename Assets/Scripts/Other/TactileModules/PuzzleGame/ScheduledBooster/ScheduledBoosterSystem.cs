using System;
using TactileModules.PuzzleGame.ScheduledBooster.Model;
using TactileModules.PuzzleGame.ScheduledBooster.Views;

namespace TactileModules.PuzzleGame.ScheduledBooster
{
	public class ScheduledBoosterSystem : IScheduledBoosterSystem
	{
		public ScheduledBoosterSystem(ScheduledBoosterHandler handler, ScheduledBoosters scheduledBoosters, IScheduledBoosterViewProvider viewProvider)
		{
			this.Handler = handler;
			this.ScheduledBoosters = scheduledBoosters;
			this.ViewProvider = viewProvider;
		}

		public ScheduledBoosterHandler Handler { get; private set; }

		public ScheduledBoosters ScheduledBoosters { get; private set; }

		public IScheduledBoosterViewProvider ViewProvider { get; private set; }
	}
}
