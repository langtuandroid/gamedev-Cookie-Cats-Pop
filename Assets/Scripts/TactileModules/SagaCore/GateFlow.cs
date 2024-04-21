using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	public class GateFlow : INextMapDot, INotifiedFlow, IPlayFlowContext, IFlow, IFiberRunnable
	{
		public GateFlow(IPlayFlowFactory playFlowFactory, GateManager gateManager, MainProgressionManager mainProgressionManager)
		{
			this.playFlowFactory = playFlowFactory;
			this.gateManager = gateManager;
			this.mainProgressionManager = mainProgressionManager;
		}

		public IEnumerator Run()
		{
			UIViewManager.UIViewStateGeneric<SagaGateView> vs = UIViewManager.Instance.ShowView<SagaGateView>(new object[0]);
			yield return vs.WaitForClose();
			SagaGateView.Result viewResult = (SagaGateView.Result)vs.ClosingResult;
			if (viewResult == SagaGateView.Result.PlayLevel)
			{
				LevelProxy levelToPlay = this.gateManager.CurrentGateLevel;
				ICorePlayFlow playLevelFlow = this.playFlowFactory.CreateCorePlayFlow(levelToPlay, this);
				playLevelFlow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.LevelPlayed));
				yield return playLevelFlow;
				LevelProxy gateProxy = this.gateManager.GetCurrentGate();
				if (this.gateManager.CurrentGateComplete)
				{
					this.NextDotIndexToOpen = gateProxy.NextLevel.Index;
				}
				else
				{
					this.NextDotIndexToOpen = gateProxy.Index;
				}
			}
			else if (viewResult == SagaGateView.Result.CloseWithProgression)
			{
				LevelProxy currentGate = this.gateManager.GetCurrentGate();
				if (this.gateManager.CurrentGateComplete)
				{
					this.NextDotIndexToOpen = currentGate.NextLevel.Index;
				}
				else
				{
					this.NextDotIndexToOpen = currentGate.Index;
				}
			}
			yield break;
		}

		private IEnumerator LevelPlayed(ILevelAttempt levelAttempt)
		{
			if (levelAttempt.Completed)
			{
				this.gateManager.AddKey(string.Empty);
				this.gateManager.ResetTimer();
				this.mainProgressionManager.Save();
			}
			yield break;
		}

		public void OnExit()
		{
		}

		public int NextDotIndexToOpen { get; private set; }

		public void Enter(IFlow previousFlow)
		{
		}

		public void Leave(IFlow nextFlow)
		{
			MapFlow mapFlow = nextFlow as MapFlow;
			if (mapFlow != null && mapFlow.MapContentController != null)
			{
				mapFlow.MapContentController.Refresh();
			}
		}

		bool IPlayFlowContext.ConsumesNormalLives
		{
			get
			{
				return true;
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
			IGateVictory gateVictory = GameImplementors.Create<IGateVictory>();
			yield return gateVictory.ShowVictory(this.gateManager, levelAttempt, chosenAction);
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
			return L.Get("Gate Level");
		}

		private readonly IPlayFlowFactory playFlowFactory;

		private readonly GateManager gateManager;

		private readonly MainProgressionManager mainProgressionManager;
	}
}
