using System;
using System.Collections;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGames.GameCore.Analytics;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class LevelEventsLogger
	{
		public LevelEventsLogger(IPlayFlowEvents playFlowEvents, string adjustMissionStartedEvent, IAdjustTracking adjustTracking, IMainProgressionForAnalytics mainProgression)
		{
			this.adjustMissionStartedEvent = adjustMissionStartedEvent;
			this.adjustTracking = adjustTracking;
			this.mainProgression = mainProgression;
			playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
		}

		private void HandlePlayFlowCreated(ICorePlayFlow playFlow)
		{
			playFlow.LevelStartedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelStarted));
			playFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
			playFlow.LevelSessionStarted += this.HandleLevelSessionStarted;
		}

		private void HandleLevelSessionStarted(ILevelSessionRunner sessionInfo)
		{
		}

		private IEnumerator HandleLevelStarted(ILevelAttempt levelAttempt)
		{
			this.adjustTracking.TrackEvent(this.adjustMissionStartedEvent); ;
			yield break;
		}

		private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
		{
			LevelEndState finalEndState = attempt.FinalEndState;
			yield break;
		}

		private readonly string adjustMissionStartedEvent;

		private readonly IAdjustTracking adjustTracking;

		private readonly IMainProgressionForAnalytics mainProgression;
	}
}
