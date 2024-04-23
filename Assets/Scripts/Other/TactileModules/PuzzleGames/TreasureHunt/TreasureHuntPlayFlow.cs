using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	public class TreasureHuntPlayFlow : IFlow, INextMapDot, IPlayFlowContext, IFiberRunnable
	{
		public TreasureHuntPlayFlow(TreasureHuntManager manager, LevelProxy levelProxy, IPlayFlowFactory playLevelSystem)
		{
			this.manager = manager;
			this.levelProxy = levelProxy;
			this.playLevelSystem = playLevelSystem;
		}

		public int NextDotIndexToOpen { get; private set; }

		public IEnumerator Run()
		{
			ICorePlayFlow flow = this.playLevelSystem.CreateCorePlayFlow(this.levelProxy, this);
			flow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelPlayed));
			yield return flow;
			if (flow.CancelledAtFirstBoosterSelection)
			{
				yield break;
			}
			this.NextDotIndexToOpen = this.manager.GetNextLevel.Index;
			yield break;
		}

		private IEnumerator HandleLevelPlayed(ILevelAttempt levelAttempt)
		{
			if (levelAttempt.Completed)
			{
				this.manager.LevelCompleted(this.levelProxy);
			}
			yield break;
		}

		public void OnExit()
		{
		}

		public bool ConsumesNormalLives
		{
			get
			{
				return true;
			}
		}

		public bool AllowRetries
		{
			get
			{
				return true;
			}
		}

		public IEnumerator ShowVictoryScreenAndChooseAction(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> chosenAction)
		{
			yield break;
		}

		public LevelStartView AlternateLevelStartViewPrefab
		{
			get
			{
				return null;
			}
		}

		public bool SkipLevelStartView
		{
			get
			{
				return false;
			}
		}

		public string GetLevelDescriptionForEndUser()
		{
			return string.Format(L.Get("Level {0}"), this.levelProxy.DisplayName);
		}

		private readonly TreasureHuntManager manager;

		private readonly LevelProxy levelProxy;

		private readonly IPlayFlowFactory playLevelSystem;
	}
}
