using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.PlayablePostcard.Controllers;
using TactileModules.PuzzleGame.PlayablePostcard.Model;
using TactileModules.PuzzleGame.PlayablePostcard.Module.Controllers;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class PlayablePostcardPlayFlow : IFlow, INextMapDot, IPlayFlowContext, IFiberRunnable
{
	public PlayablePostcardPlayFlow(IPlayFlowFactory playFlowFactory, LevelProxy levelProxy, PlayablePostcardProgress progress, PlayablePostcardControllerFactory controllerFactory)
	{
		this.playFlowFactory = playFlowFactory;
		this.levelProxy = levelProxy;
		this.progress = progress;
		this.controllerFactory = controllerFactory;
	}

	public IEnumerator Run()
	{
		ICorePlayFlow corePlayFlow = this.playFlowFactory.CreateCorePlayFlow(this.levelProxy, this);
		corePlayFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
		yield return corePlayFlow;
		yield break;
	}

	private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
	{
		if (attempt.Completed)
		{
			this.progress.CompletedLevel();
		}
		yield break;
	}

	public void OnExit()
	{
	}

	public int NextDotIndexToOpen
	{
		get
		{
			return this.progress.GetFarthestCompletedLevelIndex();
		}
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
		PhotoBoothController photoBoothController = this.controllerFactory.CreatePhotoBoothController(this.progress);
		yield return photoBoothController.ShowBooth(levelAttempt.LevelSession.SessionId);
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

	private readonly IPlayFlowFactory playFlowFactory;

	private readonly LevelProxy levelProxy;

	private readonly PlayablePostcardProgress progress;

	private readonly PlayablePostcardControllerFactory controllerFactory;
}
