using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class SlidesAndLaddersPlayFlow : IFlow, INextMapDot, IPlayFlowContext, IFiberRunnable
{
	public SlidesAndLaddersPlayFlow(IPlayFlowFactory playLevelFacade, LevelProxy levelToPlay, ISlidesAndLaddersFeatureProgression featureProgression)
	{
		this.playLevelFacade = playLevelFacade;
		this.levelToPlay = levelToPlay;
		this.featureProgression = featureProgression;
	}

	public IEnumerator Run()
	{
		ICorePlayFlow flow = this.playLevelFacade.CreateCorePlayFlow(this.levelToPlay, this);
		yield return flow;
		ResultState resultState = ResultState.None;
		if (flow.FinalLevelAttempt != null)
		{
			if (flow.FinalLevelAttempt.DidPlayAndFail)
			{
				resultState = ResultState.Failed;
			}
			else if (flow.FinalLevelAttempt.Completed)
			{
				resultState = ResultState.Completed;
			}
			this.PlayedLevelSessionId = flow.FinalLevelAttempt.LevelSession.SessionId;
			this.PlayedLevelNumber = this.levelToPlay.Index;
		}
		this.featureProgression.SetResultState(resultState);
		yield break;
	}

	public void OnExit()
	{
	}

	public string PlayedLevelSessionId { get; private set; }

	public int PlayedLevelNumber { get; private set; }

	public int NextDotIndexToOpen { get; private set; }

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
		yield return null;
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
		return string.Format(L.Get("Level {0}"), this.levelToPlay.HumanNumber - 1);
	}

	private readonly IPlayFlowFactory playLevelFacade;

	private readonly LevelProxy levelToPlay;

	private readonly ISlidesAndLaddersFeatureProgression featureProgression;
}
