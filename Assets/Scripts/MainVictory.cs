using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class MainVictory : IMainLevelVictory, IGameInterface
{
	public IEnumerator ShowVictory(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> action)
	{
		PlayLevel implementation = levelAttempt.GameImplementation as PlayLevel;
		if (implementation == null)
		{
			action.value = ((!levelAttempt.Completed) ? PostLevelPlayedAction.Exit : PostLevelPlayedAction.NextLevel);
			yield break;
		}
		LevelProxy level = implementation.Session.Level;
		MainLeaderBoardScoresRecorder mainLeaderBoardPlacementRecorder = ManagerRepository.Get<MainLeaderBoardScoresRecorder>();
		List<CloudScore> oldScores = mainLeaderBoardPlacementRecorder.GetCloudScoresFromWhenSessionStarted();
		if (ManagerRepository.Get<FacebookLoginManager>().IsLoggedInAndUserRegistered)
		{
			yield return UIViewManager.Instance.FadeCameraFrontFill(1f, 0.2f, 0);
			yield return LeaderboardManager.Instance.UpdateCachedCloudScoresCr(level.Index);
			List<CloudScore> newScores = LeaderboardManager.Instance.GetCachedCloudScores(level.Index);
			if (MainVictory.ShowLeaderboard(implementation.Session, oldScores, newScores))
			{
				LevelLeaderboardView.LevelLeaderboardData data = new LevelLeaderboardView.LevelLeaderboardData();
				data.Level = level;
				data.OldScores = oldScores;
				UIViewManager.Instance.FadeAndSwitchView<LevelLeaderboardView>(new object[]
				{
					data
				});
				yield return UIViewManager.Instance.WaitForFadeDown();
				LeaderboardView leaderBoardView = UIViewManager.Instance.FindView<LeaderboardView>();
				while (leaderBoardView.ClosingResult == null)
				{
					yield return null;
				}
			}
		}
		UIViewManager.Instance.FadeAndSwitchView<LevelResultView>(new object[]
		{
			implementation.Session
		});
		yield return UIViewManager.Instance.WaitForFadeDown();
		LevelResultView resultView = UIViewManager.Instance.FindView<LevelResultView>();
		while (resultView.ClosingResult == null)
		{
			yield return null;
		}
		action.value = (PostLevelPlayedAction)resultView.ClosingResult;
		yield break;
	}

	private void HandleClickedAction(PostLevelPlayedAction action)
	{
		this.clickedAction = new PostLevelPlayedAction?(action);
	}

	private static bool ShowLeaderboard(LevelSession levelSession, List<CloudScore> oldScores, List<CloudScore> newScores)
	{
		return true;
	}

	private PostLevelPlayedAction? clickedAction;
}
