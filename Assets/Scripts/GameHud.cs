using System;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class GameHud : MonoBehaviour
{
	public BoosterBar BoosterBar
	{
		get
		{
			return this.boosterBar.GetInstance<BoosterBar>();
		}
	}

	public GameObject PauseButton
	{
		get
		{
			return this.pauseButton;
		}
	}

	private void Update()
	{
		this.bounceFiber.Step();
		if (!this.uiIsDirty)
		{
			return;
		}
		this.uiIsDirty = false;
		this.UpdateUI();
	}

	public void Initialize(LevelSession session)
	{
        this.levelSession = session;
        this.levelSession.TurnLogic.PieceCleared += this.HandlePieceCleared;
        this.InitializeBoosterBar(this.boosterBar.GetInstance<BoosterBar>());
        this.UpdateUI();
        this.UpdateGoalLabel();
        if (this.levelSession.Level.LevelAsset is EndlessLevel)
        {
            this.levelSession.TurnLogic.PieceCleared -= this.HandlePieceCleared;
            this.savedKittenEffectTarget.gameObject.SetActive(false);
            this.levelSession.TurnLogic.TurnCompleted += this.UpdateEndlessChallengeGoalLabel;
            this.UpdateEndlessChallengeGoalLabel();
            this.starsParent.SetActive(false);
            this.scoreLabel.gameObject.SetActive(false);
            this.goalLabel.GetElement().LocalPosition = new Vector2(0f, this.goalLabel.GetElement().LocalPosition.y);
        }
        else if (this.levelSession.Level.LevelAsset is BossLevel)
        {
            this.levelSession.BossLevelController.OnBossHit += this.OnBossHit;
        }
        this.safeAreaPosition.HandleSafeAreaPositioning();
        string spriteName = (this.levelSession.Level.LevelDifficulty != LevelDifficulty.Hard) ? this.normalStarSpriteName : this.hardStarSpriteName;
        foreach (UIFilledSprite uifilledSprite in this.filledStars)
        {
            uifilledSprite.SpriteName = spriteName;
        }
    }

    private void InitializeBoosterBar(BoosterBar boosterBar)
	{
		boosterBar.Initialize(this.levelSession.GetAvailableIngameBoostersForThisLevel(), this.levelSession);
        boosterBar.ButtonsEnabled = (() => !this.levelSession.IsTurnResolving && this.levelSession.BallQueue.BallsLeft > 0 && !this.levelSession.IsDeathTriggered);
        this.levelSession.BallQueue.TripleQueueActivated += delegate ()
        {
            boosterBar.DisableButtonWithBoosterId("BoosterSuperQueue");
        };
        this.levelSession.Cannon.SuperAimActivated += delegate ()
        {
            boosterBar.DisableButtonWithBoosterId("BoosterSuperAim");
        };
        this.levelSession.ShieldActivated += delegate ()
        {
            boosterBar.DisableButtonWithBoosterId("BoosterShield");
        };
        GamePowers powers = this.levelSession.Powers;
        powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Combine(powers.PowerActivated, new Action<GamePowers.Power>(delegate (GamePowers.Power power)
        {
            if (this.levelSession.Tutorial.CurrentStep == null)
            {
                boosterBar.DisableAllButtons();
            }
        }));
        GamePowers powers2 = this.levelSession.Powers;
        powers2.OnReset = (Action)Delegate.Combine(powers2.OnReset, new Action(delegate ()
        {
            if (this.levelSession.Tutorial.CurrentStep == null)
            {
                boosterBar.EnableAllButtons();
            }
        }));
    }

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		this.uiIsDirty = true;
		GoalPiece goalPiece = piece as GoalPiece;
		if (goalPiece != null)
		{
			this.SavedKitten(goalPiece.transform.position);
		}
	}

	private void OnBossHit(Vector3 position)
	{
		this.AnimateGoalLabelIncrease();
	}

	private void SavedKitten(Vector3 position)
	{
		EffectPool.Instance.SpawnEffect("SavedKittenEffect", position, base.gameObject.layer, new object[]
		{
			new SavedKittenEffect.Parameters
			{
				worldStart = position,
				worldEnd = this.savedKittenEffectTarget.position,
				onFinished = delegate()
				{
					if (this == null)
					{
						return;
					}
					this.AnimateGoalLabelIncrease();
				}
			}
		});
	}

	private void AnimateGoalLabelIncrease()
	{
		this.UpdateGoalLabel();
		SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("CollectEffect", this.kittenIconBouncePivot.position, base.gameObject.layer, new object[0]);
		spawnedEffect.transform.position = this.kittenIconBouncePivot.position + Vector3.back * 10f;
		this.bounceFiber.Start(FiberAnimation.ScaleTransform(this.kittenIconBouncePivot, Vector3.zero, Vector3.one, this.kittenIconBounceCurve, 0f));
	}

	private void UpdateUI()
	{
		this.scoreLabel.text = L.FormatNumber(this.levelSession.Points);
		int[] starThresholds = this.levelSession.Level.StarThresholds;
		this.UpdateStarFill(this.filledStars[0], 0, starThresholds[0]);
		this.UpdateStarFill(this.filledStars[1], starThresholds[0], starThresholds[1]);
		this.UpdateStarFill(this.filledStars[2], starThresholds[1], starThresholds[2]);
	}

	public void UpdateStarFill(UIFilledSprite spr, int from, int to)
	{
		float num = Mathf.InverseLerp((float)from, (float)to, (float)this.levelSession.Points);
		if (num > 0f)
		{
			spr.fillAmount = num;
		}
		else
		{
			spr.fillAmount = 0f;
		}
	}

	private void UpdateGoalLabel()
	{
		this.goalLabel.text = string.Format("{0} / {1}", this.levelSession.GoalPiecesCollected, this.levelSession.TotalGoalPieces);
	}

	private void UpdateEndlessChallengeGoalLabel()
	{
		this.goalLabel.text = string.Format(L.Get("{0} rows"), this.levelSession.TurnLogic.Board.GetNumberOfRowsClearedByPlayer());
	}

	[SerializeField]
	private UIInstantiator boosterBar;

	[SerializeField]
	private GameObject starsParent;

	[SerializeField]
	private UILabel scoreLabel;

	[SerializeField]
	private UILabel goalLabel;

	[SerializeField]
	private SafeAreaPositioning safeAreaPosition;

	[SerializeField]
	private List<UIInstantiator> powercats;

	[SerializeField]
	private List<UIFilledSprite> filledStars;

	[SerializeField]
	private GameObject pauseButton;

	[SerializeField]
	private Transform savedKittenEffectTarget;

	[SerializeField]
	private Transform kittenIconBouncePivot;

	[SerializeField]
	private AnimationCurve kittenIconBounceCurve;

	[SerializeField]
	[UISpriteName]
	private string normalStarSpriteName;

	[SerializeField]
	[UISpriteName]
	private string hardStarSpriteName;

	private bool uiIsDirty;

	private readonly Fiber bounceFiber = new Fiber(FiberBucket.Manual);

	private LevelSession levelSession;
}
