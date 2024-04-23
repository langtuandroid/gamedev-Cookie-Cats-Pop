using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	public class MainLevelFlow : IFlow, INextMapDot, IPlayFlowContext, IPlayFlowContextCheats, IFiberRunnable
	{
		public MainLevelFlow(IPlayFlowFactory playFlowFactory, LevelProxy levelProxy, MainProgressionManager mainProgressionManager, LeaderboardManager leaderboardManager)
		{
			this.playFlowFactory = playFlowFactory;
			this.levelProxy = levelProxy;
			this.mainProgressionManager = mainProgressionManager;
			this.leaderboardManager = leaderboardManager;
		}

		public int NextDotIndexToOpen { get; private set; }

		public IEnumerator Run()
		{
			ICorePlayFlow corePlayFlow = this.playFlowFactory.CreateCorePlayFlow(this.levelProxy, this);
			corePlayFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.LevelPlayed));
			yield return corePlayFlow;
			this.NextDotIndexToOpen = this.GetLevelToPlayNext(this.levelProxy, corePlayFlow.FinalPostLevelAction).Index;
			yield break;
		}

		private LevelProxy GetLevelToPlayNext(LevelProxy current, PostLevelPlayedAction action)
		{
			if (action == PostLevelPlayedAction.Exit)
			{
				return LevelProxy.Invalid;
			}
			if (action != PostLevelPlayedAction.NextLevel)
			{
				return current;
			}
			return current.NextLevel;
		}

		private IEnumerator LevelPlayed(ILevelAttempt levelAttempt)
		{
			if (levelAttempt.Completed)
			{
				LevelProxy levelProxy = levelAttempt.LevelProxy;
				this.mainProgressionManager.SaveLevelAccomplishments(levelProxy, levelAttempt.Stats.Score);
				this.leaderboardManager.SubmitScore(levelAttempt.Stats.Score, levelProxy.Index);
			}
			yield break;
		}

		public void OnExit()
		{
		}

		bool IPlayFlowContext.ConsumesNormalLives
		{
			get
			{
				return true;
			}
		}

		bool IPlayFlowContext.AllowRetries
		{
			get
			{
				return true;
			}
		}

		public bool SkipLevelStartView
		{
			get
			{
				return false;
			}
		}

		IEnumerator IPlayFlowContext.ShowVictoryScreenAndChooseAction(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> chosenAction)
		{
			IMainLevelVictory victoryImplementation = GameImplementors.Create<IMainLevelVictory>();
			yield return victoryImplementation.ShowVictory(levelAttempt, chosenAction);
			yield break;
		}

		string IPlayFlowContext.GetLevelDescriptionForEndUser()
		{
			return string.Format(L.Get("Level {0}"), this.levelProxy.DisplayName);
		}

		public void CheatCompleteLevel(LevelProxy levelProxy, object accomplishmentData)
		{
			int wantedStars = 3;
			if (accomplishmentData is int)
			{
				wantedStars = (int)accomplishmentData;
			}
			ILevelAccomplishment levelData = this.mainProgressionManager.GetLevelData(true, levelProxy);
			if (levelData.Points > 0)
			{
				this.mainProgressionManager.Developer_CompleteLevels(levelProxy.Index - 1, 3, true);
			}
			else
			{
				this.mainProgressionManager.Developer_CompleteLevels(levelProxy.Index, wantedStars, true);
			}
			this.mainProgressionManager.Save();
		}

		private readonly IPlayFlowFactory playFlowFactory;

		private readonly LevelProxy levelProxy;

		private readonly MainProgressionManager mainProgressionManager;

		private readonly LeaderboardManager leaderboardManager;
	}
}
