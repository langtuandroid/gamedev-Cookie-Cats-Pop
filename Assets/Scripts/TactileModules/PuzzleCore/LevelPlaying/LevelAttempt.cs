using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class LevelAttempt : ILevelAttempt, IFiberRunnable
	{
		public LevelAttempt(LevelProxy levelProxy, IPlayLevel gameImplementation, ILevelAttemptResponder responder, ILevelSessionRunner levelSession)
		{
			this.LevelProxy = levelProxy;
			this.GameImplementation = gameImplementation;
			this.LevelSession = levelSession;
			this.responder = responder;
			this.WasCompletedBefore = levelProxy.IsCompleted;
			this.FinalEndState = LevelEndState.Undetermined;
		}

		public bool Completed
		{
			get
			{
				return this.FinalEndState == LevelEndState.Completed;
			}
		}

		public ILevelSessionRunner LevelSession { get; private set; }

		public double SecondsPlayed { get; private set; }

		public ILevelSessionStats Stats { get; private set; }

		public int NumberOfContinuesUsed { get; private set; }

		public LevelEndState FinalEndState { get; private set; }

		public LevelProxy LevelProxy { get; private set; }

		public int LevelSeed
		{
			get
			{
				return this.levelAttemptInfo.LevelSeed;
			}
		}

		public bool WasCompletedBefore { get; private set; }

		public bool WasCompletedForTheFirstTime
		{
			get
			{
				return this.Completed && !this.WasCompletedBefore;
			}
		}

		public bool DidPlayAndFail
		{
			get
			{
				return this.Stats.MovesUsed > 0 && !this.Completed;
			}
		}

		public IPlayLevel GameImplementation { get; private set; }

		public IEnumerator Run()
		{
			DateTime timeWhenLevelStarted = DateTime.UtcNow;
			this.levelAttemptInfo = this.GameImplementation.StartAttempt();
			yield return this.responder.AttemptStarted(this);
			EnumeratorResult<LevelEndState> finalEndState = new EnumeratorResult<LevelEndState>();
			yield return this.PlayLevelUntilNoContinues(finalEndState);
			this.FinalEndState = finalEndState.value;
			this.SecondsPlayed = (DateTime.UtcNow - timeWhenLevelStarted).TotalSeconds;
			this.Stats = this.GameImplementation.ConcludeAttemptAndGetStats();
			yield return this.responder.AttemptEnded(this);
			yield break;
		}

		public void OnExit()
		{
		}

		private IEnumerator PlayLevelUntilNoContinues(EnumeratorResult<LevelEndState> outLevelEndState)
		{
			this.NumberOfContinuesUsed = 0;
			EnumeratorResult<LevelEndResult> endResult = new EnumeratorResult<LevelEndResult>();
			yield return this.GameImplementation.PlayUntilImmediateEndState(endResult);
			while (endResult.value.state == LevelEndState.Failed && !endResult.value.unableToContinue)
			{
				EnumeratorResult<bool> keepPlaying = new EnumeratorResult<bool>();
				while (!keepPlaying.value)
				{
					keepPlaying.value = false;
					yield return this.GameImplementation.RunContinueFlow(keepPlaying);
					if (keepPlaying)
					{
						break;
					}
					yield return this.responder.ContinueDismissed(this, keepPlaying);
					if (!keepPlaying)
					{
						break;
					}
				}
				if (!keepPlaying)
				{
					break;
				}
				this.NumberOfContinuesUsed++;
				yield return this.GameImplementation.PlayUntilImmediateEndState(endResult);
			}
			outLevelEndState.value = endResult.value.state;
			yield break;
		}

		private readonly ILevelAttemptResponder responder;

		private LevelAttemptInfo levelAttemptInfo;
	}
}
