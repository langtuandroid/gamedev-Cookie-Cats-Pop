using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.Lives;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class LevelSessionRunner : ILevelSessionRunner
	{
		public LevelSessionRunner(ICorePlayFlow playFlow, ILivesManager livesManager, IControllerFactory controllerFactory)
		{
			this.SessionId = Guid.NewGuid().ToString();
			this.LevelProxy = playFlow.LevelProxy;
			this.PlayFlow = playFlow;
			this.livesManager = livesManager;
			this.controllerFactory = controllerFactory;
			this.GameImplementation = playFlow.GameImplementation;
		}

		public string SessionId { get; private set; }

		public LevelProxy LevelProxy { get; private set; }

		public ILevelStartInfo LevelStartInfo { get; private set; }

		public PostLevelPlayedAction EndAction { get; private set; }

		public IPlayLevel GameImplementation { get; private set; }

		public ICorePlayFlow PlayFlow { get; private set; }

		public LevelInformation LevelInformation
		{
			get
			{
				return this.GameImplementation.GetLevelInformation();
			}
		}

		public IEnumerator Run(int priorAttempts, EnumeratorResult<ILevelAttempt> attempt, EnumeratorResult<bool> playLevelAgain)
		{
			EnumeratorResult<bool> doPlay = new EnumeratorResult<bool>();
			if (!this.PlayFlow.PlayFlowContext.SkipLevelStartView)
			{
				yield return this.ShowBoosterViewOrOutOfLives(this.PlayFlow.LevelProxy, priorAttempts, doPlay);
			}
			else
			{
				yield return this.StartLevelWithoutStartView(this.PlayFlow.LevelProxy, priorAttempts, doPlay);
			}
			if (doPlay)
			{
				yield return this.PlayFlow.EnsureGameScreen();
				ILevelAttempt levelAttempt = this.controllerFactory.CreateLevelAttempt(this.PlayFlow, this);
				yield return levelAttempt;
				attempt.value = levelAttempt;
				this.ConsumeLife(levelAttempt);
				if (levelAttempt.FinalEndState == LevelEndState.Failed && this.PlayFlow.PlayFlowContext.AllowRetries)
				{
					playLevelAgain.value = true;
				}
				if (levelAttempt.Completed)
				{
					EnumeratorResult<PostLevelPlayedAction> endAction = new EnumeratorResult<PostLevelPlayedAction>();
					yield return this.PlayFlow.PlayFlowContext.ShowVictoryScreenAndChooseAction(levelAttempt, endAction);
					yield return this.PlayFlow.ResultsShownHook.InvokeAll(this.PlayFlow, endAction.value);
					this.EndAction = endAction.value;
					if (this.EndAction == PostLevelPlayedAction.Retry)
					{
						playLevelAgain.value = true;
					}
				}
			}
			yield break;
		}

		private IEnumerator StartLevelWithoutStartView(LevelProxy levelProxy, int priorAttempts, EnumeratorResult<bool> canPlay)
		{
			ILevelStartInfo startInfo = this.GameImplementation.CreateLevelStartInfo();
			EnumeratorResult<bool> hasEnoughLives = new EnumeratorResult<bool>();
			yield return this.EnsureUserHasEnoughLives(hasEnoughLives);
			if (hasEnoughLives.value)
			{
				canPlay.value = true;
				this.LevelStartInfo = startInfo;
			}
			else
			{
				canPlay.value = false;
			}
			yield break;
		}

		private IEnumerator ShowBoosterViewOrOutOfLives(LevelProxy levelProxy, int priorAttempts, EnumeratorResult<bool> wantToPlay)
		{
			ILevelStartInfo startInfo = this.GameImplementation.CreateLevelStartInfo();
			LevelStartViewController controller = this.controllerFactory.CreateLevelStartViewController();
			yield return controller.WaitForLevelStartView(levelProxy, priorAttempts > 0, startInfo, this.PlayFlow);
			wantToPlay.value = startInfo.DidStart;
			EnumeratorResult<bool> hasEnoughLives = new EnumeratorResult<bool>();
			if (wantToPlay.value)
			{
				yield return this.EnsureUserHasEnoughLives(hasEnoughLives);
			}
			if (wantToPlay.value && hasEnoughLives.value)
			{
				this.LevelStartInfo = startInfo;
			}
			else
			{
				wantToPlay.value = false;
			}
			yield break;
		}

		private IEnumerator EnsureUserHasEnoughLives(EnumeratorResult<bool> hasEnoughLives)
		{
			hasEnoughLives.value = true;
			if (!this.HasEnoughLivesForPlay())
			{
				EnumeratorResult<bool> didCancel = new EnumeratorResult<bool>();
				yield return this.GameImplementation.ShowOutOfLivesView(didCancel);
				if (didCancel || !this.HasEnoughLivesForPlay())
				{
					hasEnoughLives.value = false;
				}
				if (!hasEnoughLives)
				{
					yield return this.PlayFlow.UserDismissedOutOfLives.InvokeAll(this);
				}
			}
			yield break;
		}

		private bool HasEnoughLivesForPlay()
		{
			bool flag = this.PlayFlow.PlayFlowContext.ConsumesNormalLives && this.livesManager.IsOutOfLives() && !this.livesManager.HasUnlimitedLives();
			return !flag;
		}

		private void ConsumeLife(ILevelAttempt result)
		{
			if (result.DidPlayAndFail && this.PlayFlow.PlayFlowContext.ConsumesNormalLives)
			{
				this.livesManager.UseLifeIfNotUnlimited(string.Empty);
			}
		}

		private readonly ILivesManager livesManager;

		private readonly IControllerFactory controllerFactory;

		public delegate IEnumerator ShowVictoryCallback(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> action);
	}
}
