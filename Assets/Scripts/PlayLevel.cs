using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;

public class PlayLevel : IPlayLevel
{
	public LevelSession Session { get; private set; }

	public GameView GameView { get; private set; }

	public LevelInformation GetLevelInformation()
	{
		int totalGoalPieces = (this.Session == null) ? 0 : this.Session.TotalGoalPieces;
		bool isTutorial = this.Session != null && this.Session.Tutorial != null && this.Session.Tutorial.HasSteps;
		string levelType = string.Empty;
		LevelCollectionEntry levelAsset = this.levelProxy.LevelAsset;
		if (levelAsset is LevelAsset)
		{
			levelType = (levelAsset as LevelAsset).LevelType;
		}
		return new LevelInformation
		{
			IsTutorial = isTutorial,
			TotalGoalPieces = totalGoalPieces,
			LevelType = levelType
		};
	}

	public void Initialize(ILevelProxy levelProxy)
	{
		this.levelProxy = levelProxy;
	}

	public void CreateViews(ILevelStartInfo levelStartInfo)
	{
		Boot.IsRequestsBlocked += true;
		this.Session = new LevelSession(this.levelProxy as LevelProxy);
		this.Session.SetPreGameBoosters(levelStartInfo.SelectedPregameBoosters);
		GameEventManager.Instance.Emit(24, this.levelProxy.Index, 1);
		UIViewManager.Instance.ShowView<GameView>(new object[]
		{
			this.Session
		});
		this.GameView = UIViewManager.Instance.FindView<GameView>();
	}

	public void CreateViews(LevelProxy levelProxy, ILevelStartInfo levelStartInfo)
	{
		Boot.IsRequestsBlocked += true;
		this.Session = new LevelSession(levelProxy);
		this.Session.SetPreGameBoosters(levelStartInfo.SelectedPregameBoosters);
		GameEventManager.Instance.Emit(24, levelProxy.Index, 1);
		UIViewManager.Instance.ShowView<GameView>(new object[]
		{
			this.Session
		});
		this.GameView = UIViewManager.Instance.FindView<GameView>();
	}

	public void DestroyViews()
	{
		UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(this.GameView);
		if (viewLayerWithView != null)
		{
			viewLayerWithView.CloseInstantly();
		}
	}

	public LevelAttemptInfo StartAttempt()
	{
		return new LevelAttemptInfo
		{
			LevelSeed = 0
		};
	}

	public IEnumerator PlayUntilImmediateEndState(EnumeratorResult<LevelEndResult> outResult)
	{
		while (this.Session.SessionState == LevelSessionState.Playing || this.Session.SessionState == LevelSessionState.ReadyForAftermath)
		{
			yield return null;
		}
		Boot.IsRequestsBlocked += false;
		AudioManager.Instance.SetMusic(null, true);
		GameEventManager.Instance.Emit(25, this.Session.Level.Index, 1);
		switch (this.Session.SessionState)
		{
		case LevelSessionState.NoMoreMoves:
			outResult.value.state = LevelEndState.Failed;
			break;
		case LevelSessionState.Failed:
			outResult.value.state = LevelEndState.Failed;
			break;
		case LevelSessionState.Abandoned:
			outResult.value.state = LevelEndState.Abandoned;
			break;
		case LevelSessionState.Completed:
			outResult.value.state = LevelEndState.Completed;
			break;
		case LevelSessionState.DeathTriggered:
			outResult.value.state = LevelEndState.Failed;
			break;
		}
		yield break;
	}

	public ILevelSessionStats ConcludeAttemptAndGetStats()
	{
		return new PlayLevel.PlayedLevelResult
		{
			Score = this.Session.Points,
			Stars = this.Session.Level.NumberOfStarsFromPoints(this.Session.Points),
			MovesUsed = this.Session.MovesUsed,
			FreebiePaid = this.Session.Stats.FreebiePaid,
			FreebieType = this.Session.Stats.FreebieType,
			PresentsCollected = this.Session.Stats.BonusDropsCollected,
			IngameBoostersUsed = this.Session.Stats.GetTimesIngameBoostersWasUsed(),
			FreebieVideoWatched = this.Session.Stats.FreeBeeVideosWatched,
			GoalPiecesCollected = this.Session.GoalPiecesCollected,
			MovesAddedByGamePiece = this.Session.Stats.MovesAddedByGamePiece,
			MovesAddedByContinue = this.Session.Stats.BoosterContinue * 5,
			MovesLeftBeforeAftermath = this.Session.Stats.MovesLeftBeforeAftermath
		};
	}

	public IEnumerator RunContinueFlow(EnumeratorResult<bool> outDidContinue)
	{
		if (this.Session.SessionState == LevelSessionState.NoMoreMoves)
		{
			yield return this.GameView.OutOfMovesFlow();
		}
		else
		{
			yield return this.GameView.DeathTriggeredFlow();
		}
		bool didContinue = this.Session.SessionState == LevelSessionState.Playing;
		if (didContinue)
		{
			AudioManager.Instance.SetMusic(SingletonAsset<SoundDatabase>.Instance.ingameMusic, true);
		}
		outDidContinue.value = didContinue;
		yield break;
	}

	public IEnumerator ShowOutOfLivesView(EnumeratorResult<bool> outDidCancel)
	{
		UIViewManager.UIViewStateGeneric<NoMoreLivesView> vs = UIViewManager.Instance.ShowView<NoMoreLivesView>(new object[0]);
		yield return vs.WaitForClose();
		outDidCancel.value = ((int)vs.ClosingResult == 0);
		yield break;
	}

	public ILevelStartInfo CreateLevelStartInfo()
	{
		return new PlayLevel.LevelStartInfo();
	}

	private ILevelProxy levelProxy;

	public class LevelStartInfo : ILevelStartInfo
	{
		public LevelStartInfo()
		{
			this.SelectedPregameBoosters = new List<SelectedBooster>();
		}

		public int PregameBoostersUsed { get; private set; }

		public List<SelectedBooster> SelectedPregameBoosters { get; set; }

		public bool DidStart { get; set; }
	}

	private class PlayedLevelResult : ILevelSessionStats
	{
		public int Score { get; set; }

		public int Stars { get; set; }

		public int IngameBoostersUsed { get; set; }

		public int PresentsCollected { get; set; }

		public bool FreebiePaid { get; set; }

		public bool FreebieVideoWatched { get; set; }

		public string FreebieType { get; set; }

		public int MovesAddedByContinue { get; set; }

		public int MovesAddedByFreebie { get; set; }

		public int MovesAddedByGamePiece { get; set; }

		public int MovesAddedByInGameBooster { get; set; }

		public int MovesAddedByPreGameBooster { get; set; }

		public int MovesLeftBeforeAftermath { get; set; }

		public int MovesUsed { get; set; }

		public int GoalPiecesCollected { get; set; }

		public string LevelGuid { get; set; }

		public int Shuffles { get; set; }

		public Dictionary<string, int> GoalAmountsLeft { get; set; }

		public LevelEndState EndState { get; set; }
	}
}
