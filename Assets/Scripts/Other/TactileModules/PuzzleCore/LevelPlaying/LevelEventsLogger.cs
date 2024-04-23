using System;
using System.Collections;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGames.GameCore.Analytics;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class LevelEventsLogger
	{
		public LevelEventsLogger(IAnalytics analytics, IPlayFlowEvents playFlowEvents, string adjustMissionStartedEvent, IAdjustTracking adjustTracking, IMainProgressionForAnalytics mainProgression)
		{
			this.analytics = analytics;
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
			this.analytics.LogEvent(new LevelSessionCreatedEvent(sessionInfo), -1.0, null);
		}

		private IEnumerator HandleLevelStarted(ILevelAttempt levelAttempt)
		{
			this.adjustTracking.TrackEvent(this.adjustMissionStartedEvent);
			this.analytics.LogEvent(new MissionStartedEvent(levelAttempt), -1.0, null);
			yield break;
		}

		private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
		{
			LevelEndState finalEndState = attempt.FinalEndState;
			if (finalEndState != LevelEndState.Completed)
			{
				if (finalEndState != LevelEndState.Failed)
				{
					if (finalEndState == LevelEndState.Abandoned)
					{
						this.analytics.LogEvent(new MissionAbandonedEvent(attempt), -1.0, null);
					}
				}
				else
				{
					this.analytics.LogEvent(new MissionFailedEvent(attempt), -1.0, null);
				}
			}
			else
			{
				this.analytics.LogEvent(new MissionCompletedEvent(attempt, this.mainProgression), -1.0, null);
			}
			yield break;
		}

		private readonly IAnalytics analytics;

		private readonly string adjustMissionStartedEvent;

		private readonly IAdjustTracking adjustTracking;

		private readonly IMainProgressionForAnalytics mainProgression;
	}
}
