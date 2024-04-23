using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace Shared.OneLifeChallenge
{
	internal class OneLifeChallengePlayFlow : IFlow, INextMapDot, IPlayFlowContext, IFiberRunnable
	{
		public OneLifeChallengePlayFlow(OneLifeChallengeManager oneLifeChallengeManager, IPlayFlowFactory playLevelSystem)
		{
			this.oneLifeChallengeManager = oneLifeChallengeManager;
			this.playLevelSystem = playLevelSystem;
		}

		public int NextDotIndexToOpen { get; private set; }

		public IEnumerator Run()
		{
			LevelProxy levelProxy = this.oneLifeChallengeManager.GetNextLevel;
			ICorePlayFlow flow = this.playLevelSystem.CreateCorePlayFlow(levelProxy, this);
			flow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
			yield return flow;
			if (flow.FinalLevelAttempt == null)
			{
				yield break;
			}
			yield return this.oneLifeChallengeManager.provider.ShowLevelResultView();
			this.NextDotIndexToOpen = this.oneLifeChallengeManager.GetNextLevel.Index;
			yield break;
		}

		private IEnumerator HandleLevelEnded(ILevelAttempt levelAttempt)
		{
			if (levelAttempt.Completed)
			{
				this.oneLifeChallengeManager.LevelCompleted(levelAttempt.LevelProxy);
			}
			else
			{
				this.oneLifeChallengeManager.FailedLevelProxy = levelAttempt.LevelProxy;
				this.oneLifeChallengeManager.ResetProgress();
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
				return false;
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
			LevelProxy getNextLevel = this.oneLifeChallengeManager.GetNextLevel;
			return string.Format(L.Get("Level {0}"), getNextLevel.DisplayName);
		}

		private readonly OneLifeChallengeManager oneLifeChallengeManager;

		private readonly IPlayFlowFactory playLevelSystem;
	}
}
