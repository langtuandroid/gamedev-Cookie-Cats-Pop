using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;

public class DailyQuestPlayFlow : IFiberRunnable, IPlayFlowContext
{
	public DailyQuestPlayFlow(DailyQuestManager dailyQuestManager, IPlayFlowFactory playFlowFactory, LevelProxy levelProxy)
	{
		this.dailyQuestManager = dailyQuestManager;
		this.playFlowFactory = playFlowFactory;
		this.levelToPlay = levelProxy;
	}

	public IEnumerator Run()
	{
		ICorePlayFlow playFlow = this.playFlowFactory.CreateCorePlayFlow(this.levelToPlay, this);
		yield return playFlow;
		if (playFlow.CancelledAtFirstBoosterSelection)
		{
			yield break;
		}
		if (playFlow.FinalLevelAttempt.Completed)
		{
			this.dailyQuestManager.SubmitCurrentDailyQuestCompletion();
			this.levelToPlay.SaveSessionAccomplishment(playFlow.FinalLevelAttempt.Stats.Score, false);
			UIViewManager.UIViewStateGeneric<DailyQuestCompletedView> vs = UIViewManager.Instance.ShowView<DailyQuestCompletedView>(new object[0]);
			yield return vs.WaitForClose();
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
			return false;
		}
	}

	public bool AllowRetries
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

	public IEnumerator ShowVictoryScreenAndChooseAction(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> chosenAction)
	{
		yield break;
	}

	public string GetLevelDescriptionForEndUser()
	{
		return L.Get("Daily Quest");
	}

	private readonly DailyQuestManager dailyQuestManager;

	private readonly IPlayFlowFactory playFlowFactory;

	private readonly LevelProxy levelToPlay;
}
