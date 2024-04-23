using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class PowerArea : MonoBehaviour
{
	public void Initialize(LevelSession levelSession)
	{
		this.levelSession = levelSession;
		this.powers = levelSession.Powers;
		foreach (Instantiator instantiator in this.slots)
		{
			PowerSlot instance = instantiator.GetInstance<PowerSlot>();
			GamePowers.Power power = this.powers[instance.MatchFlag];
			if (power != null)
			{
				instance.Initialize(power, levelSession);
			}
			else
			{
				instantiator.gameObject.SetActive(false);
			}
		}
		this.powerInstructionPlankOnScreenPos = this.powerInstructionPlank.transform.position;
		this.powerInstructionPlank.transform.position = new Vector3(this.powerInstructionPlankOnScreenPos.x, this.powerInstructionPlankOnScreenPos.y - this.powerInstructionPlank.Size.y, this.powerInstructionPlankOnScreenPos.z);
		GamePowers gamePowers = this.powers;
		gamePowers.PowerActivated = (Action<GamePowers.Power>)Delegate.Combine(gamePowers.PowerActivated, new Action<GamePowers.Power>(this.PowerActivated));
		GamePowers gamePowers2 = this.powers;
		gamePowers2.OnReset = (Action)Delegate.Combine(gamePowers2.OnReset, new Action(this.ClearSpecialPiece));
		GamePowers gamePowers3 = this.powers;
		gamePowers3.OnAwardPointsForFull = (Action)Delegate.Combine(gamePowers3.OnAwardPointsForFull, new Action(this.AwardPointsForFullCats));
		this.animationFiber = new Fiber(FiberBucket.Manual);
		this.levelSession.Cannon.AimingStarted += delegate(AimingState s)
		{
			while (this.animationFiber.Step())
			{
			}
		};
		this.chargeEffects.Stop();
	}

	private void Update()
	{
		if (this.animationFiber != null)
		{
			this.animationFiber.Step();
		}
	}

	public IEnumerable<PowerSlot> GetEnabledPowers()
	{
		foreach (Instantiator slot in this.slots)
		{
			if (slot.gameObject.activeSelf)
			{
				yield return slot.GetInstance<PowerSlot>();
			}
		}
		yield break;
	}

	public IEnumerable<PowerSlot> GetChargedPowers()
	{
		foreach (Instantiator slot in this.slots)
		{
			if (slot.gameObject.activeSelf)
			{
				PowerSlot powerSlot = slot.GetInstance<PowerSlot>();
				if (powerSlot.Power.IsCharged)
				{
					yield return powerSlot;
				}
			}
		}
		yield break;
	}

	private void PowerActivated(GamePowers.Power power)
	{
		while (this.animationFiber.Step())
		{
		}
		foreach (Instantiator instantiator in this.slots)
		{
			if (instantiator.gameObject.activeSelf)
			{
				List<GamePowers.Power> activatedPowers = this.powers.ActivatedPowers;
				PowerSlot instance = instantiator.GetInstance<PowerSlot>();
				if (!activatedPowers.Contains(instance.Power))
				{
					instance.PowerStarted();
				}
			}
		}
		this.animationFiber.Start(this.AnimatePowerActivated(power));
	}

	private PowerSlot GetPowerSlotFromPower(GamePowers.Power power)
	{
		foreach (Instantiator instantiator in this.slots)
		{
			PowerSlot instance = instantiator.GetInstance<PowerSlot>();
			if (instance.gameObject.activeSelf)
			{
				if (instance.Power == power)
				{
					return instance;
				}
			}
		}
		return null;
	}

	private GameObject CreatePieceObject(PieceId pieceId)
	{
		PieceInfo piece = SingletonAsset<PieceDatabase>.Instance.GetPiece(pieceId);
		GameObject gameObject = UnityEngine.Object.Instantiate<Piece>(piece.gamePrefab).gameObject;
		gameObject.GetComponent<ZSorter>().enabled = false;
		gameObject.SetLayerRecursively(base.gameObject.layer);
		gameObject.transform.parent = this.piecePivot;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return gameObject;
	}

	private IEnumerator AnimatePowerActivated(GamePowers.Power power)
	{
		PieceId powerPieceId = this.powers.GetPowerPiece();
		PowerCombination powerCombination = this.powers.CurrentCombination;
		PowerSlot powerSlot = this.GetPowerSlotFromPower(power);
		if (powerSlot == null)
		{
			yield break;
		}
		PowerCat powerCat = powerSlot.CatSpine;
		this.piecePivot.transform.localScale = Vector3.one * this.levelSession.TurnLogic.Board.Root.localScale.x;
		GameObject piece = new GameObject();
		piece.transform.parent = this.piecePivot;
		piece.transform.localPosition = Vector3.zero;
		yield return new Fiber.OnExit(delegate()
		{
			UnityEngine.Object.Destroy(piece);
		});
		if (!this.vignette.gameObject.activeSelf)
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.TweenInPlank(),
				this.FadeInVignette(),
				FiberAnimation.ScaleTransform(piece.transform, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.pieceGrowCurve, 0f),
				powerCat.AnimatePowerPiece(power.GetPieceId(), piece.transform, delegate
				{
					this.chargeEffects.Play();
				})
			});
		}
		else
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.ScaleTransform(piece.transform, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.pieceGrowCurve, 0f),
				powerCat.AnimatePowerPiece(power.GetPieceId(), piece.transform, delegate
				{
					this.chargeEffects.Play();
				})
			});
		}
		this.chargeEffects.Stop();
		Vector3 startPosition = piece.transform.position;
		piece.transform.parent = this.cannonOperatorLocation;
		float chargeZ = piece.transform.position.z;
		float operatorZ = this.cannonOperatorLocation.position.z - 10f;
		yield return FiberAnimation.Animate(0f, SingletonAsset<PowerVisualSettings>.Instance.pieceMoveCurve, delegate(float t)
		{
			Vector3 position = FiberAnimation.LerpNoClamp(startPosition, this.cannonOperatorLocation.position, t);
			position.z = ((t >= 0.5f) ? operatorZ : chargeZ);
			piece.transform.position = position;
		}, false);
		this.ClearSpecialPiece();
		this.specialPiece = this.CreatePieceObject(powerPieceId);
		this.specialPiece.transform.parent = this.cannonOperatorLocation;
		this.specialPiece.transform.localPosition = Vector3.zero + Vector3.back * 10f;
		CPPiece boardPiece = this.specialPiece.GetComponent<CPPiece>();
		ComboPowerPiece comboPowerPiece = boardPiece as ComboPowerPiece;
		if (comboPowerPiece != null)
		{
			comboPowerPiece.SetPowerCombination(powerCombination);
		}
		else
		{
			boardPiece.Initialize(this.levelSession);
		}
		this.gameCannon.Shoot -= this.OnShoot;
		this.gameCannon.Shoot += this.OnShoot;
		yield break;
	}

	private void ClearSpecialPiece()
	{
		if (this.specialPiece != null)
		{
			UnityEngine.Object.Destroy(this.specialPiece);
		}
		this.specialPiece = null;
	}

	private void OnShoot(AimingState aim)
	{
		if (!aim.Main.IsValidForShot)
		{
			return;
		}
		this.gameCannon.AimingEnded -= this.OnShoot;
		GameEventManager.Instance.Emit(26, this.levelSession, 1);
		this.ClearSpecialPiece();
		this.levelSession.Stats.ClearPowerComboStats();
		if (this.vignette.gameObject.activeSelf)
		{
			this.animationFiber.Start(FiberHelper.RunParallel(new IEnumerator[]
			{
				this.TweenOutPlank(),
				this.FadeOutVignette()
			}));
		}
		foreach (Instantiator instantiator in this.slots)
		{
			if (instantiator.gameObject.activeSelf)
			{
				PowerSlot instance = instantiator.GetInstance<PowerSlot>();
				instance.PowerEnded();
			}
		}
		this.gameCannon.Shoot -= this.OnShoot;
	}

	private void AwardPointsForFullCats()
	{
		List<GamePowers.Power> list = new List<GamePowers.Power>(this.powers.AvailablePowers);
		for (int i = 0; i < list.Count; i++)
		{
			GamePowers.Power power = list[i];
			PowerSlot powerSlotFromPower = this.GetPowerSlotFromPower(list[i]);
			if (power.IsCharged)
			{
				SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("DriftingPoints", Vector3.zero, powerSlotFromPower.gameObject.layer, new object[]
				{
					2000,
					powerSlotFromPower.MatchFlag
				});
				spawnedEffect.transform.position = powerSlotFromPower.transform.position + Vector3.back * 6f;
			}
		}
	}

	private IEnumerator FadeInVignette()
	{
		this.vignette.gameObject.SetActive(true);
		yield return UIFiberAnimations.FadeAlpha(this.vignette, 0f, 0.333333343f, 1.5f, null);
		yield break;
	}

	private IEnumerator FadeOutVignette()
	{
		yield return UIFiberAnimations.FadeAlpha(this.vignette, 0.333333343f, 0f, 1f, null);
		this.vignette.gameObject.SetActive(false);
		yield break;
	}

	private IEnumerator TweenInPlank()
	{
		TutorialStep step = this.levelSession.Tutorial.CurrentStep as TutorialStep;
		List<GamePowers.Power> available = new List<GamePowers.Power>(this.powers.AvailablePowers);
		if (available.Count > 1 && (step == null || step.dismissType != TutorialStep.DismissType.UsePower))
		{
			Vector3 startPosition = this.powerInstructionPlank.transform.position;
			Vector3 endTargetPosition = this.powerInstructionPlankOnScreenPos;
			yield return FiberAnimation.Animate(0.3f, null, delegate(float t)
			{
				Vector3 position = FiberAnimation.LerpNoClamp(startPosition, endTargetPosition, t);
				this.powerInstructionPlank.transform.position = position;
			}, false);
		}
		yield break;
	}

	private IEnumerator TweenOutPlank()
	{
		Vector3 startPosition = this.powerInstructionPlank.transform.position;
		Vector3 endTargetPosition = new Vector3(this.powerInstructionPlankOnScreenPos.x, this.powerInstructionPlankOnScreenPos.y - this.powerInstructionPlank.Size.y, this.powerInstructionPlankOnScreenPos.z);
		yield return FiberAnimation.Animate(0.2f, null, delegate(float t)
		{
			Vector3 position = FiberAnimation.LerpNoClamp(startPosition, endTargetPosition, t);
			this.powerInstructionPlank.transform.position = position;
		}, false);
		yield break;
	}

	private const float VIGNETTE_ALPHA_IN_AMOUNT = 0.333333343f;

	[SerializeField]
	public Transform chargeLocation;

	[SerializeField]
	private GameCannon gameCannon;

	[SerializeField]
	private Transform cannonOperatorLocation;

	[SerializeField]
	private ParticleSystem chargeEffects;

	[SerializeField]
	private Transform piecePivot;

	[SerializeField]
	private UIWidget vignette;

	[SerializeField]
	private UIElement powerInstructionPlank;

	[SerializeField]
	private List<Instantiator> slots;

	private Vector3 powerInstructionPlankOnScreenPos;

	private GamePowers powers;

	private Fiber animationFiber;

	private GameObject specialPiece;

	private LevelSession levelSession;
}
