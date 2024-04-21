using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using NinjaUI;
using UnityEngine;

public class CannonBallQueue : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action MinusPieceMoveFinished;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action DeathPieceMoveFinished;

	public bool IsAnimatingEffect { get; private set; }

	public void Initialize(LevelSession levelSession)
	{
		this.levelSession = levelSession;
		levelSession.NextMovePrepared += this.HandleNextMovePrepared;
		levelSession.BallQueue.QueueSwapped += this.HandleQueueSwapped;
		levelSession.BallQueue.TripleQueueActivated += this.HandleTripleQueueActivated;
		levelSession.BallQueue.TripleQueueDeactivated += this.HandleTripleQueueDeactivated;
		levelSession.BallQueue.TripleQueueModified += this.HandleTripleQueueModified;
		levelSession.BallQueue.NonMatchingBallChanged += this.HandleNonMatchingBallChanged;
		levelSession.SpecialShotLoaded += this.HandleSpecialSlotLoaded;
		levelSession.TurnLogic.PieceCleared += this.HandlePieceCleared;
		levelSession.ShotFired += this.HandleShotFired;
		levelSession.StateChanged += this.HandleStateChanged;
		levelSession.BallQueue.ExtraBallsGiven += this.HandleExtraBallsGiven;
		levelSession.Cheat += this.UpdateLabel;
		GamePowers powers = levelSession.Powers;
		powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Combine(powers.PowerActivated, new Action<GamePowers.Power>(this.HandlePowerActivated));
		this.inkCoverSprite.gameObject.SetActive(false);
		this.FillBallsPositions();
		this.UpdateLabel();
	}

	public void Destroy()
	{
		if (this.levelSession != null)
		{
			this.levelSession.NextMovePrepared -= this.HandleNextMovePrepared;
			this.levelSession.BallQueue.QueueSwapped -= this.HandleQueueSwapped;
			this.levelSession.BallQueue.TripleQueueActivated -= this.HandleTripleQueueActivated;
			this.levelSession.BallQueue.TripleQueueDeactivated -= this.HandleTripleQueueDeactivated;
			this.levelSession.BallQueue.TripleQueueModified -= this.HandleTripleQueueModified;
			this.levelSession.BallQueue.NonMatchingBallChanged -= this.HandleNonMatchingBallChanged;
			this.levelSession.SpecialShotLoaded -= this.HandleSpecialSlotLoaded;
			this.levelSession.TurnLogic.PieceCleared -= this.HandlePieceCleared;
			this.levelSession.ShotFired -= this.HandleShotFired;
			this.levelSession.StateChanged -= this.HandleStateChanged;
			this.levelSession.BallQueue.ExtraBallsGiven -= this.HandleExtraBallsGiven;
			this.levelSession.Cheat -= this.UpdateLabel;
			GamePowers powers = this.levelSession.Powers;
			powers.PowerActivated = (Action<GamePowers.Power>)Delegate.Remove(powers.PowerActivated, new Action<GamePowers.Power>(this.HandlePowerActivated));
		}
		if (this.animateFiber != null)
		{
			this.animateFiber.Terminate();
		}
	}

	private void Update()
	{
		if (this.animateFiber != null)
		{
			this.animateFiber.Step();
		}
		this.idleRotationPivot.transform.localRotation = Quaternion.Euler(0f, 0f, this.idleRotationCurve.Evaluate(Time.time));
	}

	[UsedImplicitly]
	private void OnPressed(UIEvent e)
	{
		this.levelSession.BallQueue.SwapQueuedPieces();
	}

	private void HandlePowerActivated(GamePowers.Power p)
	{
		this.specialSlot.Clear();
	}

	private void HandleExtraBallsGiven(bool waitForResult)
	{
		if ((waitForResult && !this.levelSession.IsTurnResolving) || !waitForResult)
		{
			this.FillBallsPositions();
			this.UpdateLabel();
		}
	}

	private void HandleShotFired()
	{
		this.UpdateLabel();
	}

	private void HandleStateChanged(LevelSession session)
	{
		if (this.levelSession.SessionState == LevelSessionState.Playing)
		{
			this.inkCoverSprite.gameObject.SetActive(false);
		}
	}

	private void HandleNextMovePrepared()
	{
		this.mainSlot.Clear();
		this.specialSlot.Clear();
		this.AnimateQueue();
	}

	private GameObject CreateBall(Transform location, PieceId pieceId)
	{
		PieceInfo piece = SingletonAsset<PieceDatabase>.Instance.GetPiece(pieceId);
		Piece piece2 = UnityEngine.Object.Instantiate<Piece>(piece.gamePrefab);
		piece2.ZSorter().enabled = false;
		piece2.transform.localScale = Vector3.one * this.levelSession.TurnLogic.Board.Root.localScale.x;
		piece2.gameObject.SetLayerRecursively(base.gameObject.layer);
		piece2.transform.parent = location;
		piece2.transform.localPosition = Vector3.zero;
		piece2.GetComponent<CPPiece>().Initialize(this.levelSession);
		return piece2.gameObject;
	}

	private void HandleTripleQueueActivated()
	{
		if (this.levelSession.BallQueue.SecondQueuedPiece != PieceId.Empty)
		{
			this.secondQueueSlot.Clear();
			this.secondQueueSlot.ball = this.CreateBall(this.secondQueueSlot.transform, this.levelSession.BallQueue.SecondQueuedPiece);
		}
		this.StopAnyBoxBlink();
		this.boxFrontSprite.SpriteName = this.tripleQueueBoxFrontSpriteName;
	}

	private void HandleTripleQueueDeactivated()
	{
		this.StopAnyBoxBlink();
		this.secondQueueSlot.Clear();
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.boxFrontBlinking.transform.position + new Vector3(-20f, 0f, 0f), base.gameObject.layer, new object[0]);
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.boxFrontBlinking.transform.position + new Vector3(20f, 0f, 0f), base.gameObject.layer, new object[0]);
		this.boxFrontSprite.SpriteName = this.defaultFrontSpriteName;
	}

	private void HandleTripleQueueModified(GameBallQueue.TripleQueueModification modification)
	{
		if (modification == GameBallQueue.TripleQueueModification.Blink)
		{
			this.StartBoxBlink();
		}
	}

	private void StartBoxBlink()
	{
		this.boxFrontBlinking.SetActive(true);
		this.boxFrontBlinking.gameObject.GetComponent<ColorPulsator>().enabled = true;
	}

	private void StopAnyBoxBlink()
	{
		this.boxFrontBlinking.SetActive(false);
		this.boxFrontBlinking.gameObject.GetComponent<ColorPulsator>().enabled = false;
	}

	private void HandleQueueSwapped()
	{
		this.AnimateQueue();
	}

	private void HandlePieceCleared(CPPiece piece, int pointsToGive, HitMark hit)
	{
		bool wasShieldActive = this.levelSession.ShieldActive;
		Transform transform = this.queueSlot.transform;
		SpawnedEffect spawnedEffect = null;
		if (hit.cause != HitCause.Unknown)
		{
			if (piece is MinusPiece)
			{
				transform = ((!(this.minusPieceTarget != null)) ? transform : this.minusPieceTarget);
				spawnedEffect = EffectPool.Instance.SpawnEffect("FlyingMoves", piece.transform.position, piece.gameObject.layer, new object[]
				{
					"-2",
					transform,
					Color.red
				});
			}
			else if (piece is PlusPiece)
			{
				spawnedEffect = EffectPool.Instance.SpawnEffect("FlyingPlusMoves", piece.transform.position, piece.gameObject.layer, new object[]
				{
					"+2",
					transform,
					Color.green
				});
			}
			else if (piece is DeathPiece)
			{
				transform = ((!(this.minusPieceTarget != null)) ? transform : this.minusPieceTarget);
				spawnedEffect = EffectPool.Instance.SpawnEffect("FlyingInk", piece.transform.position, piece.gameObject.layer, new object[]
				{
					transform
				});
			}
		}
		if (spawnedEffect != null)
		{
			this.IsAnimatingEffect = true;
			SpawnedEffect spawnedEffect2 = spawnedEffect;
			spawnedEffect2.onFinished = (Action)Delegate.Combine(spawnedEffect2.onFinished, new Action(delegate()
			{
				this.UpdateLabel();
				if (piece is MinusPiece)
				{
					if (this.MinusPieceMoveFinished != null)
					{
						this.MinusPieceMoveFinished();
					}
					SingletonAsset<SoundDatabase>.Instance.minusTwo.Play();
					if (!wasShieldActive)
					{
						this.animateFiber.Start(this.HighlightBoxAnimation(false));
					}
				}
				else if (piece is PlusPiece)
				{
					SingletonAsset<SoundDatabase>.Instance.plusTwo.Play();
					this.animateFiber.Start(this.HighlightBoxAnimation(true));
				}
				else if (piece is DeathPiece)
				{
					if (this.DeathPieceMoveFinished != null)
					{
						this.DeathPieceMoveFinished();
					}
					SingletonAsset<SoundDatabase>.Instance.deathBubbleSquish.PlaySequential();
					if (!wasShieldActive)
					{
						this.inkCoverSprite.gameObject.SetActive(true);
						this.animateFiber.Start(this.HighlightBoxAnimation(false));
					}
				}
				this.IsAnimatingEffect = false;
			}));
		}
	}

	public void HandleNonMatchingBallChanged()
	{
		Transform transform = this.queueSlot.transform;
		transform = ((!(this.noMatchTextTarget != null)) ? transform : this.noMatchTextTarget);
		EffectPool.Instance.SpawnEffect("SummaryPoints", transform.position, base.gameObject.layer, new object[]
		{
			string.Empty,
			string.Empty,
			SingletonAsset<LevelVisuals>.Instance.NoMatchBallSwapText
		});
	}

	private void FillBallsPositions()
	{
		if (this.levelSession.BallQueue.BallsLeft > 0 && this.mainSlot.ball == null)
		{
			this.mainSlot.ball = this.CreateBall(this.mainSlot.transform, this.levelSession.BallQueue.NextPieceToShoot);
			this.mainSlot.pieceId = this.levelSession.BallQueue.NextPieceToShoot;
		}
		if (this.levelSession.BallQueue.BallsLeft > 1 && this.queueSlot.ball == null)
		{
			this.queueSlot.ball = this.CreateBall(this.queueSlot.transform, this.levelSession.BallQueue.QueuedPiece);
			this.queueSlot.pieceId = this.levelSession.BallQueue.QueuedPiece;
		}
		if (this.levelSession.BallQueue.HasTripleQueue && this.levelSession.BallQueue.BallsLeft > 2 && this.secondQueueSlot.ball == null)
		{
			this.secondQueueSlot.ball = this.CreateBall(this.secondQueueSlot.transform, this.levelSession.BallQueue.SecondQueuedPiece);
			this.secondQueueSlot.pieceId = this.levelSession.BallQueue.SecondQueuedPiece;
		}
	}

	public IEnumerator HighlightBoxAnimation(bool good)
	{
		Color highlightColor = (!good) ? Color.red : (Color.grey * 1.5f);
		Vector3 boxPos = this.queueSlot.transform.localPosition;
		yield return new Fiber.OnExit(delegate()
		{
			this.queueSlot.transform.localPosition = boxPos;
			this.boxFrontSprite.Color = Color.gray;
		});
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.Animate(0.5f, delegate(float t)
			{
				this.boxFrontSprite.Color = Color.Lerp(highlightColor, Color.gray, t);
			}),
			FiberAnimation.MoveLocalTransform(this.queueSlot.transform, boxPos, boxPos + Vector3.up * 100f, SingletonAsset<LevelVisuals>.Instance.boxJumpCurve, 0f)
		});
		yield break;
	}

	private void AnimateQueue()
	{
		List<IEnumerator> list = new List<IEnumerator>();
		if (this.levelSession.BallQueue.BallsLeft == 1)
		{
			if (this.queueSlot.ball != null)
			{
				list.Add(this.AnimateSlot(this.queueSlot, this.mainSlot, this.levelSession.BallQueue.NextPieceToShoot, true, 70f));
				this.queueSlot.Clear();
				this.secondQueueSlot.Clear();
			}
		}
		else if (this.levelSession.BallQueue.HasTripleQueue)
		{
			SingletonAsset<SoundDatabase>.Instance.swapBubbles.Play();
			if (this.levelSession.BallQueue.BallsLeft > 2)
			{
				list.Add(this.AnimateSlot(this.queueSlot, this.mainSlot, this.levelSession.BallQueue.NextPieceToShoot, true, 70f));
				list.Add(this.AnimateSlot(this.secondQueueSlot, this.queueSlot, this.levelSession.BallQueue.QueuedPiece, false, 70f));
				if (this.mainSlot.ball == null)
				{
					list.Add(this.AnimateSlot(this.hiddenSlot, this.secondQueueSlot, this.levelSession.BallQueue.SecondQueuedPiece, false, 70f));
				}
				else
				{
					list.Add(this.AnimateSlot(this.mainSlot, this.secondQueueSlot, this.levelSession.BallQueue.SecondQueuedPiece, true, -30f));
				}
			}
			else if (this.secondQueueSlot.ball != null)
			{
				list.Add(this.AnimateSlot(this.queueSlot, this.mainSlot, this.levelSession.BallQueue.NextPieceToShoot, true, 70f));
				list.Add(this.AnimateSlot(this.secondQueueSlot, this.queueSlot, this.levelSession.BallQueue.QueuedPiece, false, 70f));
				this.secondQueueSlot.Clear();
			}
			else
			{
				list.Add(this.AnimateSlot(this.queueSlot, this.mainSlot, this.levelSession.BallQueue.NextPieceToShoot, true, 70f));
				list.Add(this.AnimateSlot(this.mainSlot, this.queueSlot, this.levelSession.BallQueue.QueuedPiece, true, -30f));
			}
		}
		else
		{
			SingletonAsset<SoundDatabase>.Instance.swapBubbles.Play();
			list.Add(this.AnimateSlot(this.queueSlot, this.mainSlot, this.levelSession.BallQueue.NextPieceToShoot, true, 70f));
			if (this.mainSlot.ball == null)
			{
				list.Add(this.AnimateSlot(this.hiddenSlot, this.queueSlot, this.levelSession.BallQueue.QueuedPiece, false, 70f));
			}
			else
			{
				list.Add(this.AnimateSlot(this.mainSlot, this.queueSlot, this.levelSession.BallQueue.QueuedPiece, true, -30f));
			}
		}
		Vector3 eulerAngles = this.rotatingPivot.eulerAngles;
		Vector3 destAngles = eulerAngles;
		destAngles.z -= 180f;
		list.Add(FiberAnimation.RotateTransform(this.rotatingPivot, eulerAngles, destAngles, this.rotationCurve, 0.4f));
		this.animateFiber.Start(FiberHelper.RunParallel(list.ToArray()));
	}

	public void UpdateLabel()
	{
		this.movesLabel.text = Mathf.Max(0, this.levelSession.BallQueue.BallsLeft).ToString();
	}

	private void HandleSpecialSlotLoaded()
	{
		if (this.levelSession.Powers.AnyActive)
		{
			return;
		}
		this.animateFiber.Start(this.AnimateSlot(this.queueSlot, this.specialSlot, this.levelSession.SpecialPieceToShoot, true, 70f));
	}

	private IEnumerator AnimateSlot(CannonBallQueue.Slot source, CannonBallQueue.Slot dest, PieceId pieceId, bool sineCurveEnabled = false, float sineCurveHeight = 70f)
	{
		dest.Clear();
		if (SingletonAsset<PieceDatabase>.Instance.GetPiece(pieceId) == null)
		{
			yield break;
		}
		yield return new Fiber.OnExit(delegate()
		{
			if (dest.ball != null)
			{
				dest.ball.transform.position = dest.transform.position + Vector3.back * dest.offsetZ;
			}
		});
		dest.ball = this.CreateBall(dest.transform, pieceId);
		dest.pieceId = pieceId;
		yield return FiberAnimation.Animate(0.4f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			Vector3 position = source.transform.position;
			Vector3 position2 = dest.transform.position;
			Vector3 position3 = Vector3.Lerp(position, position2, t);
			if (sineCurveEnabled)
			{
				position3.y += ((sineCurveHeight <= 0f) ? (Mathf.Max(Mathf.Sin(3.14159274f * t * 1.5f), 0f) * sineCurveHeight) : (Mathf.Sin(3.14159274f * t) * ((!this.levelSession.ShieldActive || sineCurveHeight <= 0f) ? sineCurveHeight : (sineCurveHeight * 0.5f))));
			}
			position3.z = ((t >= ((!sineCurveEnabled || !this.levelSession.ShieldActive || sineCurveHeight <= 0f) ? 0.5f : 0.25f)) ? (position2.z - dest.offsetZ) : (position.z - source.offsetZ));
			if (dest.ball != null)
			{
				dest.ball.transform.position = position3;
			}
		}, false);
		yield break;
	}

	public void HideAllBallsExceptMain()
	{
		this.queueSlot.SetActive(false);
		this.secondQueueSlot.SetActive(false);
		this.hiddenSlot.SetActive(false);
		this.specialSlot.SetActive(false);
	}

	public CannonBallQueue.Slot mainSlot;

	public CannonBallQueue.Slot queueSlot;

	public CannonBallQueue.Slot secondQueueSlot;

	public CannonBallQueue.Slot hiddenSlot;

	public CannonBallQueue.Slot specialSlot;

	[SerializeField]
	public UILabel movesLabel;

	public Transform minusPieceTarget;

	[SerializeField]
	private Transform rotatingPivot;

	[SerializeField]
	private AnimationCurve rotationCurve;

	[SerializeField]
	private Transform idleRotationPivot;

	[SerializeField]
	private AnimationCurve idleRotationCurve;

	[SerializeField]
	private UISprite boxFrontSprite;

	[SerializeField]
	private GameObject boxFrontBlinking;

	[SerializeField]
	private UISprite inkCoverSprite;

	[SerializeField]
	private string defaultFrontSpriteName;

	[SerializeField]
	private string tripleQueueBoxFrontSpriteName;

	[SerializeField]
	private Transform noMatchTextTarget;

	private LevelSession levelSession;

	private readonly Fiber animateFiber = new Fiber(FiberBucket.Manual);

	[Serializable]
	public class Slot
	{
		public void Clear()
		{
			if (this.ball != null)
			{
				UnityEngine.Object.Destroy(this.ball);
				this.ball = null;
			}
		}

		public void SetActive(bool active)
		{
			if (this.ball == null)
			{
				return;
			}
			this.ball.SetActive(active);
		}

		public Transform transform;

		[NonSerialized]
		public GameObject ball;

		[NonSerialized]
		public PieceId pieceId;

		public float offsetZ;
	}
}
