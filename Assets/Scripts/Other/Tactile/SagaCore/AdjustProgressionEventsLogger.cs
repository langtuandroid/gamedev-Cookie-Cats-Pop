using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore.Analytics;

namespace Tactile.SagaCore
{
	public class AdjustProgressionEventsLogger
	{
		public AdjustProgressionEventsLogger(IPlayFlowEvents playFlowEvents, IAdjustProgressionEvents eventIds, IAdjustTracking adjustTracking, GateManager gateManager)
		{
			this.adjustLevelEvents = new Dictionary<int, string>
			{
				{
					3,
					eventIds.ADJUST_IO_LEVEL_3_COMPLETED_EVENT_TOKEN
				},
				{
					5,
					eventIds.ADJUST_IO_LEVEL_5_COMPLETED_EVENT_TOKEN
				},
				{
					7,
					eventIds.ADJUST_IO_LEVEL_7_COMPLETED_EVENT_TOKEN
				},
				{
					10,
					eventIds.ADJUST_IO_LEVEL_10_COMPLETED_EVENT_TOKEN
				},
				{
					20,
					eventIds.ADJUST_IO_LEVEL_20_COMPLETED_EVENT_TOKEN
				},
				{
					30,
					eventIds.ADJUST_IO_LEVEL_30_COMPLETED_EVENT_TOKEN
				},
				{
					40,
					eventIds.ADJUST_IO_LEVEL_40_COMPLETED_EVENT_TOKEN
				}
			};
			this.eventIds = eventIds;
			this.adjustTracking = adjustTracking;
			playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
			gateManager.GateUnlocked += this.HandleGateUnlocked;
		}

		private void HandleGateUnlocked(LevelProxy obj)
		{
			LevelDatabase rootDatabase = obj.RootDatabase;
			if (rootDatabase != null && rootDatabase.GetGateIndex(obj.Index) == 0)
			{
				this.adjustTracking.TrackEvent(this.eventIds.ADJUST_IO_GATE_1_UNLOCKED_EVENT_TOKEN);
			}
		}

		private void HandlePlayFlowCreated(ICorePlayFlow runner)
		{
			runner.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelPlayed));
		}

		private IEnumerator HandleLevelPlayed(ILevelAttempt levelAttempt)
		{
			if (!levelAttempt.LevelProxy.IsValid || !(levelAttempt.LevelProxy.RootDatabase is MainLevelDatabase))
			{
				yield break;
			}
			string adjustEventId;
			if (this.adjustLevelEvents.TryGetValue(levelAttempt.LevelProxy.HumanNumber, out adjustEventId))
			{
				this.adjustTracking.TrackEvent(adjustEventId);
				yield break;
			}
			yield break;
		}

		private readonly IAdjustProgressionEvents eventIds;

		private readonly IAdjustTracking adjustTracking;

		private readonly Dictionary<int, string> adjustLevelEvents;
	}
}
