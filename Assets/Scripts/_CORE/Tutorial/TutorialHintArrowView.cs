using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class TutorialHintArrowView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.session = (parameters[0] as LevelSession);
		this.powerActivated = false;
		this.activePowerSlots = new List<PowerSlot>();
	}

	protected override void ViewWillAppear()
	{
		TutorialStep tutorialStep = this.session.Tutorial.CurrentStep as TutorialStep;
		GameBoardMasker.Begin(this.session.TurnLogic.Board);
		this.gameView = UIViewManager.Instance.FindView<GameView>();
		if (tutorialStep.pointAtTile >= 0)
		{
			Tile tile = this.session.TurnLogic.Board.GetTile(tutorialStep.pointAtTile);
			if (tutorialStep.showTraceLine)
			{
				this.finger.Begin(tile.WorldPosition, this.gameView.cannon, tutorialStep.slowAiming, null);
			}
			else
			{
				this.finger.Begin(tile.WorldPosition, null, false, null);
			}
			this.MoveFrame(tile.WorldPosition.y + 150f);
			if (tutorialStep.highlightFinger)
			{
				GameBoardMasker.HighlightTarget(tile);
			}
		}
		else
		{
			this.finger.End();
		}
		if (tutorialStep.highlightBoss)
		{
			Transform bossTransform = this.session.BossLevelController.BossCharacterController.BossTransform;
			GameBoardMasker.HighlightPos(bossTransform.position, 350f);
			this.MoveFrame(bossTransform.position.y - 500f);
		}
		this.frame.GetInstance<TutorialSpeechBubble>().Message = tutorialStep.Message;
		GameBoardMasker.HighlightTiles(tutorialStep.activeTiles, this.session.TurnLogic.Board);
		bool active = tutorialStep.dismissType == TutorialStep.DismissType.TapToContinueTight || tutorialStep.dismissType == TutorialStep.DismissType.BossNewStage;
		this.button.SetActive(active);
		if (tutorialStep.dismissType == TutorialStep.DismissType.SwapQueue)
		{
			this.FingerAndHighlight(tutorialStep.highlightFinger, this.gameView.cannonBallQueue.movesLabel.transform.position, 300f, 0f, null);
		}
		else if (tutorialStep.dismissType == TutorialStep.DismissType.UsePower || tutorialStep.dismissType == TutorialStep.DismissType.WaitForFreePowerClaimed)
		{
			this.activePowerSlots.AddRange(this.gameView.powerArea.GetEnabledPowers());
			foreach (PowerSlot powerSlot in this.activePowerSlots)
			{
				this.FingerAndHighlight(tutorialStep.highlightFinger, powerSlot.gameObject.transform.position, 400f, 200f, (!powerSlot.Power.IsCharged) ? null : this.WaitForPower(powerSlot));
			}
			if (this.activePowerSlots.Count > 1)
			{
				GamePowers powers = this.session.Powers;
				powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Combine(powers.PowerActivated, new Action<GamePowers.Power>(this.PowerActivated));
			}
		}
		else if (tutorialStep.dismissType == TutorialStep.DismissType.UseBooster)
		{
			BoosterButton boosterButton = this.gameView.gameHud.BoosterBar.FindButton(tutorialStep.useBoosterType);
			if (boosterButton != null)
			{
				this.FingerAndHighlight(tutorialStep.highlightFinger, boosterButton.transform.position, 200f, 0f, null);
			}
		}
		else
		{
			if (tutorialStep.highlightShooter)
			{
				GameBoardMasker.HighlightShooter();
			}
			if (tutorialStep.highlightPowercats)
			{
				foreach (PowerSlot powerSlot2 in this.gameView.powerArea.GetEnabledPowers())
				{
					GameBoardMasker.HighlightPos(powerSlot2.gameObject.transform.position, 400f);
				}
			}
			if (tutorialStep.highlightBasket)
			{
				Vector3 position = this.gameView.cannonBallQueue.movesLabel.transform.position;
				GameBoardMasker.HighlightPos(position, 200f);
				this.MoveFrame(position.y + 200f);
			}
			if (!MaskOverlay.Instance.HasAnyCutouts())
			{
				GameBoardMasker.End();
			}
		}
	}

	private IEnumerator WaitForPower(PowerSlot powerSlot)
	{
		while (!powerSlot.CatSpine.ChargedAnimIsLooping)
		{
			yield return null;
		}
		yield break;
	}

	private void ButtonClicked(UIEvent e)
	{
		this.button.SetActive(false);
		base.Close(0);
	}

	private void MoveFrame(float y)
	{
		Vector2 elementSize = this.frame.GetElementSize();
		Vector2 elementSize2 = base.GetElementSize();
		Vector3 position = this.frame.transform.position;
		float num = (elementSize2.y - elementSize.y) * 0.5f;
		y += 200f;
		position.y = Mathf.Clamp(y, -num, num);
		this.frame.transform.position = position;
	}

	private void FingerAndHighlight(bool doHighlight, Vector3 wp, float size, float frameMoveAmount = 0f, IEnumerator customWaitCode = null)
	{
		if (doHighlight)
		{
			GameBoardMasker.HighlightPos(wp, size);
		}
		this.finger.Begin(wp, null, false, customWaitCode);
		this.MoveFrame(wp.y + frameMoveAmount);
	}

	protected override void ViewWillDisappear()
	{
		GamePowers powers = this.session.Powers;
		powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Remove(powers.PowerActivated, new Action<GamePowers.Power>(this.PowerActivated));
		GameBoardMasker.End();
		this.finger.End();
	}

	private void PowerActivated(GamePowers.Power power)
	{
		foreach (PowerSlot powerSlot in this.activePowerSlots)
		{
			if (powerSlot.Power == power)
			{
				this.activePowerSlots.Remove(powerSlot);
				break;
			}
		}
		if (this.activePowerSlots.Count == 0)
		{
			GamePowers powers = this.session.Powers;
			powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Remove(powers.PowerActivated, new Action<GamePowers.Power>(this.PowerActivated));
		}
		else
		{
			this.finger.Begin(this.activePowerSlots[0].gameObject.transform.position, null, false, null);
		}
		if (!this.powerActivated)
		{
			GameBoardMasker.HighlightPos(this.gameView.powerArea.chargeLocation.position, 600f);
			this.powerActivated = true;
		}
	}

	public override Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
	{
		float wantedAspect = screenSize.Aspect();
		Vector2 originalSize = base.OriginalSize;
		return UIUtility.CorrectSizeToAspect(base.OriginalSize, wantedAspect, AspectCorrection.Fill);
	}

	public UIInstantiator frame;

	public GameObject button;

	public TutorialFinger finger;

	private LevelSession session;

	private GameView gameView;

	private List<PowerSlot> activePowerSlots;

	private bool powerActivated;
}
