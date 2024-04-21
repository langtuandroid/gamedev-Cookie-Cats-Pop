using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class TournamentPlayFlow : IFlow, INextMapDot, IPlayFlowContext, IFiberRunnable
{
	public TournamentPlayFlow(int dotIndexToPlay, IPlayFlowFactory playLevelSystem, TournamentManager tournamentManager)
	{
		this.dotIndexToPlay = dotIndexToPlay;
		this.playLevelSystem = playLevelSystem;
		this.tournamentManager = tournamentManager;
	}

	public int NextDotIndexToOpen { get; private set; }

	public IEnumerator Run()
	{
		LevelProxy levelToPlay = this.tournamentManager.GetLevelFromDotId(this.dotIndexToPlay);
		ICorePlayFlow corePlayFlow = this.playLevelSystem.CreateCorePlayFlow(levelToPlay, this);
		yield return corePlayFlow;
		if (corePlayFlow.CancelledAtFirstBoosterSelection)
		{
			yield break;
		}
		if (corePlayFlow.FinalLevelAttempt.Completed)
		{
			this.tournamentManager.PersistedUnsubmittedScore = new LocalScore
			{
				Leaderboard = levelToPlay.Index,
				Score = corePlayFlow.FinalLevelAttempt.Stats.Score
			};
			this.NextDotIndexToOpen = this.tournamentManager.GetNextLevel(this.dotIndexToPlay);
		}
		else
		{
			this.tournamentManager.UseLife();
			this.NextDotIndexToOpen = this.tournamentManager.GetHighestUnlockedLevel();
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
			return false;
		}
	}

	bool IPlayFlowContext.AllowRetries
	{
		get
		{
			return true;
		}
	}

	IEnumerator IPlayFlowContext.ShowVictoryScreenAndChooseAction(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> chosenAction)
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
		return string.Format(L.Get("Level {0}"), this.dotIndexToPlay + 1);
	}

	private readonly int dotIndexToPlay;

	private readonly IPlayFlowFactory playLevelSystem;

	private readonly TournamentManager tournamentManager;
}
