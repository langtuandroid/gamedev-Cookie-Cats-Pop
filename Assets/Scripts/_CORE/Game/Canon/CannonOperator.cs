using System;
using System.Collections;
using Fibers;
using Spine;
using Spine.Unity;
using UnityEngine;

public class CannonOperator : MonoBehaviour
{
	private string AimAnimation
	{
		get
		{
			return this.IsSad ? "Aim2" : "Aim";
		}
	}

	private string IdleAnimation
	{
		get
		{
			return this.IsSad ? "Idle_sad" : "Idle";
		}
	}

	private string ThrowAnimationUp
	{
		get
		{
			return this.IsSad ? "Throwsad" : "Throw";
		}
	}

	private string ThrowAnimationLeft
	{
		get
		{
			return this.IsSad ? "Throwsad2" : "Throw2";
		}
	}

	private string ThrowAnimationRight
	{
		get
		{
			return this.IsSad ? "Throwsad3" : "Throw3";
		}
	}

	private bool IsSad
	{
		get
		{
			return this.ballQueue.BallsLeft <= 5;
		}
	}

	public bool DisableDanceAnimation { get; set; }

	public void StartSpineBlink()
	{
		SpineColorPulsator component = this.catSpine.gameObject.GetComponent<SpineColorPulsator>();
		component.enabled = true;
		component.SetSlotFilter((Slot s) => s.data.name.ToLower().Contains("kikkert") || s.data.name.ToLower().Contains("glass"));
	}

	public void StopAnySpineBlink()
	{
		this.catSpine.gameObject.GetComponent<SpineColorPulsator>().enabled = false;
	}

	public void Initialize(LevelSession session)
	{
		this.ballQueue = session.BallQueue;
		this.turnLogic = session.TurnLogic;
		this.cannon.AimingStarted += this.AimingStarted;
		this.cannon.AimingEnded += this.AimingEnded;
		session.ShotFired += this.HandleShotFired;
		this.catSpine.PlayAnimation(0, this.IdleAnimation, true, false);
		this.catSpine.skeleton.SetSkin("Normal");
		this.celebrationKitten.SetActive(false);
		this.turnLogic.CustomPieceMovement = new TurnLogic.CustomPieceMovementDelegate(this.CustomPieceMovementRetriever);
		session.Cannon.SuperAimActivated += delegate()
		{
			this.StopAnySpineBlink();
			this.catSpine.skeleton.SetSkin("Goggles");
		};
		session.Cannon.SuperAimModified += delegate(GameCannon.SuperAimModification modification)
		{
			if (modification == GameCannon.SuperAimModification.Blink)
			{
				this.StartSpineBlink();
			}
		};
		session.Cannon.SuperAimDeactivated += delegate()
		{
			EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.catSpine.transform.position + new Vector3(-30f, 100f, 0f), this.gameObject.layer, new object[0]);
			EffectPool.Instance.SpawnEffect("BubblePopSmoke", this.catSpine.transform.position + new Vector3(30f, 100f, 0f), this.gameObject.layer, new object[0]);
			this.catSpine.skeleton.SetSkin("Normal");
		};
		this.ballQueue.BallsLeftChanged += this.BallsLeftChanged;
		session.StateChanged += delegate(LevelSession obj)
		{
			if (session.SessionState == LevelSessionState.ReadyForAftermath && !this.DisableDanceAnimation)
			{
				this.catSpine.PlayAnimation(0, "Celebration", true, false);
				this.celebrationKitten.SetActive(true);
				this.ballQueue.BallsLeftChanged -= this.BallsLeftChanged;
			}
		};
	}

	private void BallsLeftChanged()
	{
		if (this.wasSad != this.IsSad)
		{
			this.wasSad = this.IsSad;
			this.catSpine.PlayAnimation(0, this.IdleAnimation, true, false);
		}
	}

	private void AimingStarted(AimingState aim)
	{
		this.aimingState = aim;
		this.catSpine.PlayAnimation(0, this.AimAnimation, true, false);
		this.catSpine.UpdateLocal -= this.CatSpine_UpdateLocal;
		this.catSpine.UpdateLocal += this.CatSpine_UpdateLocal;
		SingletonAsset<SoundDatabase>.Instance.aim.Play();
	}

	private void HandleShotFired()
	{
		this.throwFiber.Start(this.ThrowLogic());
		this.throwFiber.Step();
	}

	private void AimingEnded(AimingState aim)
	{
		this.catSpine.UpdateLocal -= this.CatSpine_UpdateLocal;
		this.aimingState = null;
		if (this.throwFiber.IsTerminated)
		{
			this.catSpine.PlayAnimation(0, this.IdleAnimation, true, false);
		}
	}

	private void Update()
	{
		if (this.throwFiber != null)
		{
			this.throwFiber.Step();
		}
	}

	private IEnumerator ThrowLogic()
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.catSpine.PlayAnimation(0, this.IdleAnimation, true, false);
		});
		IEnumerator enumerator = this.ballBone.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				transform.gameObject.SetActive(false);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		TrackEntry throwUp = this.catSpine.PlayAnimation(0, this.ThrowAnimationUp, false, false);
		TrackEntry throwSideways = this.catSpine.PlayAnimation(1, (this.aimAnimationNormalizedTarget >= 0f) ? this.ThrowAnimationRight : this.ThrowAnimationLeft, false, false);
		float upMix = 0f;
		if (this.aimAnimationNormalizedTarget < 0f)
		{
			upMix = 1f + this.aimAnimationNormalizedTarget;
		}
		else
		{
			upMix = 1f - this.aimAnimationNormalizedTarget;
		}
		throwUp.mix = upMix;
		throwSideways.mix = 1f - upMix;
		SingletonAsset<SoundDatabase>.Instance.releaseForShot.Play();
		yield return FiberHelper.Wait(this.catSpine.state.GetCurrent(0).endTime - 0.4f, (FiberHelper.WaitFlag)0);
		SingletonAsset<SoundDatabase>.Instance.pinkThrow.Play();
		SingletonAsset<SoundDatabase>.Instance.shoot.Play();
		yield return FiberAnimation.Animate(0.2f, delegate(float t)
		{
			throwUp.mix = Mathf.Lerp(upMix, 1f, t);
			throwSideways.mix = Mathf.Lerp(1f - upMix, 0f, t);
		});
		yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
		this.catSpine.PlayAnimation(0, this.IdleAnimation, true, false);
		yield return new Fiber.OnExit(delegate()
		{
			this.catSpine.state.ClearTrack(1);
		});
		yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	private void CatSpine_UpdateLocal(ISkeletonAnimation animatedSkeletonComponent)
	{
		this.aimAnimationNormalizedTarget = Mathf.Lerp(this.aimAnimationNormalizedTarget, this.aimingState.Main.direction.x, Time.deltaTime * ((!this.aimingState.Main.IsValidForShot) ? 5f : 20f));
		TrackEntry current = this.catSpine.state.GetCurrent(0);
		current.time = current.endTime * 0.5f + this.aimAnimationNormalizedTarget * current.endTime * 0.5f;
		current.loop = false;
	}

	private IEnumerator CustomPieceMovementRetriever(int movementIndex, CPPiece piece, Vector3 from, Vector3 to)
	{
		if (movementIndex > 0)
		{
			return null;
		}
		return this.CustomPieceMovement(piece, from, to);
	}

	private IEnumerator CustomPieceMovement(CPPiece piece, Vector3 from, Vector3 to)
	{
		yield return new Fiber.OnExit(delegate()
		{
			piece.gameObject.GetComponent<ZSorter>().enabled = true;
		});
		piece.gameObject.GetComponent<ZSorter>().enabled = false;
		Transform pieceTransform = piece.transform;
		Transform pieceParent = pieceTransform.parent;
		float boardZ = pieceTransform.localPosition.z;
		float boneZ = pieceParent.InverseTransformPoint(this.ballBone.transform.position).z;
		float time = 0.4f;
		while (time > 0f)
		{
			time -= Time.deltaTime;
			pieceTransform.position = this.ballBone.position;
			yield return null;
		}
		float onlineTimer = 0f;
		Vector3 ballPositionInPieceSpace = pieceTransform.localPosition;
		yield return FiberAnimation.Animate((from - to).magnitude / SingletonAsset<LevelVisuals>.Instance.shotSpeed * 1.5f, this.throwCurve, delegate(float t)
		{
			Vector3 a = Vector3.Lerp(ballPositionInPieceSpace, from, onlineTimer / 0.466666669f);
			onlineTimer += Time.deltaTime;
			Vector3 localPosition = Vector3.Lerp(a, to, t);
			localPosition.z = ((t >= 0.5f) ? boardZ : boneZ);
			piece.transform.localPosition = localPosition;
		}, false);
		yield break;
	}

	[SerializeField]
	public SkeletonAnimation catSpine;

	[SerializeField]
	private GameCannon cannon;

	[SerializeField]
	private AnimationCurve throwCurve;

	[SerializeField]
	private Transform ballBone;

	[SerializeField]
	private GameObject celebrationKitten;

	private readonly Fiber throwFiber = new Fiber(FiberBucket.Manual);

	private TurnLogic turnLogic;

	private AimingState aimingState;

	private float aimAnimationNormalizedTarget;

	private GameBallQueue ballQueue;

	private bool wasSad;
}
