using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.FacebookExtras;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.SagaCore;

public class MainLeaderBoardScoresRecorder
{
	public MainLeaderBoardScoresRecorder(IPlayFlowEvents playFlowEvents, LeaderboardManager leaderboardManager, FacebookLoginManager facebookLoginManager)
	{
		this.leaderboardManager = leaderboardManager;
		this.facebookLoginManager = facebookLoginManager;
		playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
	}

	private void HandlePlayFlowCreated(ICorePlayFlow playFlow)
	{
		if (this.facebookLoginManager.IsLoggedInAndUserRegistered && playFlow.PlayFlowContext is MainLevelFlow)
		{
			playFlow.LevelStartedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelStarted));
		}
	}

	private IEnumerator HandleLevelStarted(ILevelAttempt attempt)
	{
		int index = attempt.LevelProxy.Index;
		List<CloudScore> cachedCloudScores = this.leaderboardManager.GetCachedCloudScores(index);
		this.scoresWhenLevelStarted = new List<CloudScore>();
		if (cachedCloudScores != null)
		{
			this.scoresWhenLevelStarted.AddRange(cachedCloudScores);
		}
		yield break;
	}

	public List<CloudScore> GetCloudScoresFromWhenSessionStarted()
	{
		return this.scoresWhenLevelStarted;
	}

	private readonly LeaderboardManager leaderboardManager;

	private readonly FacebookLoginManager facebookLoginManager;

	private List<CloudScore> scoresWhenLevelStarted = new List<CloudScore>();
}
