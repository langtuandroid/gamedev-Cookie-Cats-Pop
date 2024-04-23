using System;
using System.Collections;
using System.Collections.Generic;

public class TutorialLogic : TutorialLogicBase
{
	protected override IEnumerator WaitForStep(ITutorialStep step)
	{
		TutorialStep s = step as TutorialStep;
		switch (s.dismissType)
		{
		case TutorialStep.DismissType.TapToContinue:
			yield return this.WaitForDismiss(s);
			break;
		case TutorialStep.DismissType.Shoot:
			yield return this.WaitForStartingShoot(s);
			break;
		case TutorialStep.DismissType.SwapQueue:
			yield return this.WaitForSwapQueue(s);
			break;
		case TutorialStep.DismissType.UsePower:
			yield return this.WaitForUsePower(s);
			break;
		case TutorialStep.DismissType.WaitATurn:
			yield return this.WaitATurn(s);
			break;
		case TutorialStep.DismissType.UseBooster:
			yield return this.WaitForUseBooster(s);
			break;
		case TutorialStep.DismissType.TapToContinueTight:
			yield return this.WaitForDismissTight(s);
			break;
		case TutorialStep.DismissType.WaitForFreePowerClaimed:
			yield return this.WaitForFreePowerClaimed(s);
			break;
		case TutorialStep.DismissType.BossNewStage:
			yield return this.WaitForBossNewStage(s);
			break;
		default:
			yield break;
		}
		yield break;
	}

	private LevelSession LevelSession
	{
		get
		{
			return base.Session as LevelSession;
		}
	}

	private IEnumerator WaitForDismiss(TutorialStep step)
	{
		UIViewManager.UIViewStateGeneric<TutorialMessageView> vs = UIViewManager.Instance.ShowView<TutorialMessageView>(new object[]
		{
			base.Session
		});
		yield return vs.WaitForClose();
		yield break;
	}

	private IEnumerator WaitForDismissTight(TutorialStep step)
	{
		this.LevelSession.Cannon.InputEnabled += false;
		this.LevelSession.BallQueue.SwappingEnabled += false;
		this.LevelSession.Powers.ChargingEnabled += false;
		UIViewManager.UIViewStateGeneric<TutorialHintArrowView> vs = UIViewManager.Instance.ShowView<TutorialHintArrowView>(new object[]
		{
			base.Session
		});
		yield return vs.WaitForClose();
		this.LevelSession.BallQueue.SwappingEnabled += true;
		this.LevelSession.Cannon.InputEnabled += true;
		this.LevelSession.Powers.ChargingEnabled += true;
		yield break;
	}

	private IEnumerator WaitForStartingShoot(TutorialStep step)
	{
		this.LevelSession.BallQueue.SwappingEnabled += false;
		this.LevelSession.Powers.ChargingEnabled += false;
		UIViewManager.UIViewStateGeneric<TutorialHintArrowView> vs = UIViewManager.Instance.ShowView<TutorialHintArrowView>(new object[]
		{
			base.Session
		});
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			8
		});
		vs.View.Close(0);
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			7
		});
		this.LevelSession.BallQueue.SwappingEnabled += true;
		this.LevelSession.Powers.ChargingEnabled += true;
		yield break;
	}

	private IEnumerator WaitATurn(TutorialStep step)
	{
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			7
		});
		yield break;
	}

	private IEnumerator WaitForSwapQueue(TutorialStep step)
	{
		this.LevelSession.Powers.ChargingEnabled += false;
		this.LevelSession.Cannon.InputEnabled += false;
		UIViewManager.UIViewStateGeneric<TutorialHintArrowView> vs = UIViewManager.Instance.ShowView<TutorialHintArrowView>(new object[]
		{
			base.Session
		});
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			1
		});
		vs.View.Close(0);
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		this.LevelSession.Cannon.InputEnabled += true;
		this.LevelSession.Powers.ChargingEnabled += true;
		yield break;
	}

	private IEnumerator WaitForUseBooster(TutorialStep step)
	{
		this.LevelSession.Cannon.InputEnabled += false;
		this.LevelSession.BallQueue.SwappingEnabled += false;
		this.LevelSession.Powers.ChargingEnabled += false;
		UIViewManager.UIViewStateGeneric<TutorialHintArrowView> vs = UIViewManager.Instance.ShowView<TutorialHintArrowView>(new object[]
		{
			base.Session
		});
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			4
		});
		vs.View.Close(0);
		yield return FiberHelper.Wait(2f, (FiberHelper.WaitFlag)0);
		this.LevelSession.BallQueue.SwappingEnabled += true;
		this.LevelSession.Cannon.InputEnabled += true;
		this.LevelSession.Powers.ChargingEnabled += true;
		yield break;
	}

	private IEnumerator WaitForFreePowerClaimed(TutorialStep step)
	{
		this.LevelSession.Cannon.InputEnabled += false;
		this.LevelSession.BallQueue.SwappingEnabled += false;
		UIViewManager.UIViewStateGeneric<TutorialHintArrowView> vs = UIViewManager.Instance.ShowView<TutorialHintArrowView>(new object[]
		{
			base.Session
		});
		GameView gameView = UIViewManager.Instance.FindView<GameView>();
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			51
		});
		vs.View.Close(0);
		List<PowerSlot> p;
		do
		{
			yield return base.WaitForGameEvents(new GameEventType[]
			{
				13
			});
			p = new List<PowerSlot>(gameView.powerArea.GetChargedPowers());
		}
		while (p.Count != 0);
		if (!base.IsInLastStep)
		{
			yield return FiberHelper.Wait(3.5f, (FiberHelper.WaitFlag)0);
		}
		this.LevelSession.BallQueue.SwappingEnabled += true;
		this.LevelSession.Cannon.InputEnabled += true;
		yield break;
	}

	private IEnumerator WaitForUsePower(TutorialStep step)
	{
		this.LevelSession.Cannon.InputEnabled += false;
		this.LevelSession.BallQueue.SwappingEnabled += false;
		UIViewManager.UIViewStateGeneric<TutorialHintArrowView> vs = UIViewManager.Instance.ShowView<TutorialHintArrowView>(new object[]
		{
			base.Session
		});
		GameView gameView = UIViewManager.Instance.FindView<GameView>();
		List<PowerSlot> p;
		do
		{
			yield return base.WaitForGameEvents(new GameEventType[]
			{
				13
			});
			p = new List<PowerSlot>(gameView.powerArea.GetChargedPowers());
		}
		while (p.Count != 0);
		vs.View.Close(0);
		if (!base.IsInLastStep)
		{
			yield return FiberHelper.Wait(3.5f, (FiberHelper.WaitFlag)0);
		}
		this.LevelSession.BallQueue.SwappingEnabled += true;
		this.LevelSession.Cannon.InputEnabled += true;
		yield break;
	}

	private IEnumerator WaitForBossNewStage(TutorialStep step)
	{
		this.LevelSession.BossLevelController.BossCharacterController.SetBossState(BossState.ACTIVE);
		yield return base.WaitForGameEvents(new GameEventType[]
		{
			62
		});
		if (step.highlightBoss)
		{
			this.LevelSession.BossLevelController.BossCharacterController.SetBossState(BossState.INACTIVE);
		}
		yield return this.WaitForDismissTight(step);
		if (step.highlightBoss)
		{
			this.LevelSession.BossLevelController.BossCharacterController.SetBossState(BossState.ACTIVE);
		}
		yield break;
	}
}
