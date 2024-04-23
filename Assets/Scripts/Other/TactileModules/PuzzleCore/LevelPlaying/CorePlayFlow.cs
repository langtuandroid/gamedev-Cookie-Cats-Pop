using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class CorePlayFlow : ICorePlayFlow, IFiberRunnable, IFullScreenOwner, ILevelAttemptResponder
	{
		public CorePlayFlow(IFullScreenManager fullScreenManager, IPlayLevelImplementationFactory implementorFactory, LevelProxy levelToPlay, IPlayFlowContext playFlowContext, IControllerFactory controllerFactory)
		{
			this.LevelProxy = levelToPlay;
			this.ContinueDismissedHook = new BreakableHookList<ILevelAttempt>();
			this.LevelEndedHook = new HookList<ILevelAttempt>();
			this.LevelStartedHook = new HookList<ILevelAttempt>();
			this.ResultsShownHook = new HookList<ICorePlayFlow, PostLevelPlayedAction>();
			this.UserDismissedOutOfLives = new HookList<ILevelSessionRunner>();
			this.StartViewInitializedHook = new HookList<LevelProxy, bool>();
			this.PlayFlowContext = playFlowContext;
			this.fullScreenManager = fullScreenManager;
			this.implementorFactory = implementorFactory;
			this.controllerFactory = controllerFactory;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ILevelSessionRunner> LevelSessionStarted;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ILevelSessionRunner> LevelSessionEnded;



		public IHookList<LevelProxy, bool> StartViewInitializedHook { get; private set; }

		public IHookList<ILevelAttempt> LevelStartedHook { get; private set; }

		public IBreakableHookList<ILevelAttempt> ContinueDismissedHook { get; private set; }

		public IHookList<ILevelAttempt> LevelEndedHook { get; private set; }

		public IHookList<ICorePlayFlow, PostLevelPlayedAction> ResultsShownHook { get; private set; }

		public IHookList<ILevelSessionRunner> UserDismissedOutOfLives { get; private set; }

		public LevelProxy LevelProxy { get; private set; }

		public IPlayLevel GameImplementation { get; private set; }

		public bool CancelledAtFirstBoosterSelection
		{
			get
			{
				return this.latestAttempt == null;
			}
		}

		public ILevelAttempt FinalLevelAttempt
		{
			get
			{
				return this.latestAttempt;
			}
		}

		public PostLevelPlayedAction FinalPostLevelAction { get; private set; }

		public IPlayFlowContext PlayFlowContext { get; private set; }

		IEnumerator IFiberRunnable.Run()
		{
			this.GameImplementation = this.implementorFactory.CreatePlayLevelImplementor();
			this.GameImplementation.Initialize(this.LevelProxy);
			int numPriorSessions = 0;
			EnumeratorResult<bool> playLevelAgain;
			do
			{
				this.currentSession = this.controllerFactory.CreateLevelSessionRunner(this);
				this.LevelSessionStarted(this.currentSession);
				EnumeratorResult<ILevelAttempt> playedAttempt = new EnumeratorResult<ILevelAttempt>();
				playLevelAgain = new EnumeratorResult<bool>();
				yield return this.currentSession.Run(numPriorSessions, playedAttempt, playLevelAgain);
				numPriorSessions++;
				if (playedAttempt.value != null)
				{
					this.latestAttempt = playedAttempt.value;
				}
				this.LevelSessionEnded(this.currentSession);
			}
			while (playLevelAgain);
			this.FinalPostLevelAction = this.currentSession.EndAction;
			yield return this.RemoveGameScreenIfAny();
			yield break;
		}

		void IFiberRunnable.OnExit()
		{
		}

		private IEnumerator RemoveGameScreenIfAny()
		{
			if (this.fullScreenManager.Top == this)
			{
				yield return this.fullScreenManager.Pop();
			}
			yield break;
		}

		public IEnumerator EnsureGameScreen()
		{
			if (this.fullScreenManager.Top == this)
			{
				yield return this.fullScreenManager.ChangeToSameScreen();
			}
			else
			{
				yield return this.fullScreenManager.Push(this);
			}
			yield break;
		}

		IEnumerator IFullScreenOwner.ScreenAcquired()
		{
			this.GameImplementation.CreateViews(this.currentSession.LevelStartInfo);
			yield break;
		}

		void IFullScreenOwner.ScreenLost()
		{
			this.GameImplementation.DestroyViews();
		}

		void IFullScreenOwner.ScreenReady()
		{
		}

		IEnumerator ILevelAttemptResponder.AttemptStarted(ILevelAttempt attempt)
		{
			yield return this.LevelStartedHook.InvokeAll(attempt);
			yield break;
		}

		IEnumerator ILevelAttemptResponder.AttemptEnded(ILevelAttempt attempt)
		{
			yield return this.LevelEndedHook.InvokeAll(attempt);
			yield break;
		}

		IEnumerator ILevelAttemptResponder.ContinueDismissed(ILevelAttempt attempt, EnumeratorResult<bool> keepPlaying)
		{
			yield return this.ContinueDismissedHook.InvokeAll(attempt, keepPlaying);
			yield break;
		}

		private readonly IFullScreenManager fullScreenManager;

		private readonly IPlayLevelImplementationFactory implementorFactory;

		private readonly IControllerFactory controllerFactory;

		private ILevelAttempt latestAttempt;

		private ILevelSessionRunner currentSession;
	}
}
