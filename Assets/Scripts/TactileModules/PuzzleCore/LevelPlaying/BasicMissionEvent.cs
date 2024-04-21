using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class BasicMissionEvent : BasicMissionEventBase
	{
		protected BasicMissionEvent(ILevelAttempt levelAttempt) : base(levelAttempt.LevelSession)
		{
			ILevelSessionRunner levelSession = levelAttempt.LevelSession;
			if (levelSession.LevelStartInfo != null)
			{
				if (levelSession.LevelStartInfo.SelectedPregameBoosters != null)
				{
					this.PregameBoostersUsed = levelSession.LevelStartInfo.SelectedPregameBoosters.Count;
				}
				this.LevelSeed = levelAttempt.LevelSeed;
			}
		}

		private TactileAnalytics.RequiredParam<int> PregameBoostersUsed { get; set; }

		private TactileAnalytics.OptionalParam<int> LevelSeed { get; set; }
	}
}
