using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.EndlessChallenge;
using TactileModules.PuzzleGames.GameCore;

public class EndlessChallengePlayFlow : IFlow, IPlayFlowContext, IFiberRunnable
{
	public EndlessChallengePlayFlow(EndlessChallengeHandler endlessChallengeHandler, LevelProxy levelToPlay, IPlayFlowFactory playFlowFactory)
	{
		this.endlessChallengeHandler = endlessChallengeHandler;
		this.levelToPlay = levelToPlay;
		this.playFlowFactory = playFlowFactory;
	}

	public IEnumerator Run()
	{
		ICorePlayFlow corePlayFlow = this.playFlowFactory.CreateCorePlayFlow(this.levelToPlay, this);
		corePlayFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
		yield return corePlayFlow;
		if (!this.endlessChallengeHandler.ShouldDeactivateFeature())
		{
			EnumeratorResult<UIViewManager.UIViewState> result = new EnumeratorResult<UIViewManager.UIViewState>();
			yield return this.endlessChallengeHandler.TryShowLeaderboard(result);
			if (result.value != null)
			{
				yield return result.value.WaitForClose();
			}
		}
		yield break;
	}

	private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
	{
		PlayLevel gameImplementation = attempt.GameImplementation as PlayLevel;
		LevelSession playedLevelSession = gameImplementation.Session;
		this.numRowsClearedByPlayer = playedLevelSession.TurnLogic.Board.GetNumberOfRowsClearedByPlayer();
		this.endlessChallengeHandler.TrySetHighestRow(this.numRowsClearedByPlayer);
		yield return UIViewManager.Instance.FadeCameraFrontFill(1f, 0f, 0);
		UIViewManager.UIViewStateGeneric<EndlessChallengeEndView> vs = UIViewManager.Instance.ShowView<EndlessChallengeEndView>(new object[]
		{
			this.numRowsClearedByPlayer
		});
		yield return UIViewManager.Instance.FadeCameraFrontFill(0f, 0f, 0);
		yield return vs.WaitForClose();
		FiberCtrl.Pool.Run(this.endlessChallengeHandler.SubmitAndUpdateAfterPlayedLevel(attempt.LevelSession.SessionId, playedLevelSession, this.numRowsClearedByPlayer), false);
		yield break;
	}

	public IEnumerator ShowVictoryScreenAndChooseAction(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> chosenAction)
	{
		yield break;
	}

	public bool ConsumesNormalLives
	{
		get
		{
			return false;
		}
	}

	public bool AllowRetries
	{
		get
		{
			return false;
		}
	}

	public LevelStartView AlternateLevelStartViewPrefab { get; private set; }

	public bool SkipLevelStartView
	{
		get
		{
			return false;
		}
	}

	public string GetLevelDescriptionForEndUser()
	{
		return L.Get("Endless Level");
	}

	public void OnExit()
	{
	}

	private readonly EndlessChallengeHandler endlessChallengeHandler;

	private readonly LevelProxy levelToPlay;

	private readonly IPlayFlowFactory playFlowFactory;

	private int numRowsClearedByPlayer;
}
