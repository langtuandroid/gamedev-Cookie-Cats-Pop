using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class AfterMath : MonoBehaviour
{
	public static IEnumerator DetachRemainingPieces(LevelSession session)
	{
		session.Powers.ChargingEnabled += false;
		yield return new Fiber.OnExit(delegate()
		{
			session.Powers.ChargingEnabled += true;
		});
		ResolveState resolveState = session.TurnLogic.CreateResolveStateForAftermath();
		Floaters floaters = new Floaters(session);
		List<int> tileIndices = new List<int>();
		foreach (Tile tile in session.TurnLogic.Board.GetOccupiedTiles())
		{
			tileIndices.Add(tile.Index);
		}
		yield return floaters.ResolveTileList(resolveState, tileIndices);
		yield break;
	}

	private static PieceId GetQueuedPiece(LevelSession session, int index)
	{
		PieceId pieceId = PieceId.Empty;
		if (index == 0)
		{
			pieceId = session.BallQueue.QueuedPiece;
		}
		else if (index == 1)
		{
			pieceId = session.BallQueue.SecondQueuedPiece;
		}
		return (!(pieceId == PieceId.Empty)) ? pieceId : session.GetRandomSpawnPieceId();
	}

	public static IEnumerator Run(LevelSession session, CannonBallQueue visualBallQueue)
	{
		SingletonAsset<SoundDatabase>.Instance.bubblePop.ResetSequential();
		session.Powers.ChargingEnabled += false;
		yield return new Fiber.OnExit(delegate()
		{
			session.Powers.ChargingEnabled += true;
		});
		ResolveState resolveState = session.TurnLogic.CreateResolveStateForAftermath();
		int ballsToResolve = session.BallQueue.BallsLeft;
		if (ballsToResolve <= 0)
		{
			yield break;
		}
		float interval = 2f / (float)ballsToResolve;
		float delay = 0f;
		int index = 0;
		if (session.IsDeathTriggered && !session.ShieldActive)
		{
			List<IEnumerator> anims = new List<IEnumerator>();
			while (ballsToResolve > 0)
			{
				SingletonAsset<SoundDatabase>.Instance.aftermathCannon.Play();
				anims.Add(FiberHelper.RunSerial(new IEnumerator[]
				{
					FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0),
					AfterMath.AnimateFlyingPieceInked(session, visualBallQueue, index)
				}));
				if (ballsToResolve > 0 && ballsToResolve % 10 == 0)
				{
					anims.Add(FiberHelper.RunSerial(new IEnumerator[]
					{
						FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0),
						visualBallQueue.HighlightBoxAnimation(false)
					}));
				}
				delay += interval;
				ballsToResolve--;
				index++;
			}
			yield return FiberHelper.RunParallel(anims.ToArray());
		}
		else
		{
			while (ballsToResolve > 0)
			{
				SingletonAsset<SoundDatabase>.Instance.aftermathCannon.Play();
				CPPiece cppiece = session.TurnLogic.Board.SpawnPiece(AfterMath.GetQueuedPiece(session, index)) as CPPiece;
				cppiece.transform.position = visualBallQueue.queueSlot.transform.position + Vector3.down * 20f;
				cppiece.ZSorter().enabled = false;
				cppiece.gameObject.SetActive(false);
				IHitResolver hitResolver = resolveState.CreatePieceResolver(cppiece);
				hitResolver.MarkForRemoval(0f, 500);
				hitResolver.QueueAnimation(AfterMath.AnimateFlyingPiece(session, visualBallQueue, cppiece), delay);
				ballsToResolve--;
				delay += interval;
				index++;
			}
		}
		yield return resolveState.ApplyAllHits(null);
		yield break;
	}

	private static IEnumerator AnimateFlyingPieceInked(LevelSession session, CannonBallQueue ballQueue, int index)
	{
		session.BallQueue.ConsumeTopBall();
		ballQueue.UpdateLabel();
		if (index % 2 == 0)
		{
			EffectPool.Instance.SpawnEffect("InkSplatter", ballQueue.queueSlot.transform.position + Vector3.down * -10f, ballQueue.gameObject.layer, new object[0]);
		}
		yield return null;
		yield break;
	}

	private static IEnumerator AnimateFlyingPiece(LevelSession session, CannonBallQueue ballQueue, CPPiece piece)
	{
		piece.gameObject.SetActive(true);
		session.BallQueue.ConsumeTopBall();
		ballQueue.UpdateLabel();
		piece.DisableSpring();
		float drift = UnityEngine.Random.Range(-300f, 500f);
		Vector3 origPos = piece.transform.position;
		float destY = UnityEngine.Random.Range(50f, 130f);
		AnimationCurve curve = SingletonAsset<LevelVisuals>.Instance.aftermathCurve;
		Vector3 wobbleScale = new Vector3(1.1f, 0.9f, 1f);
		yield return FiberAnimation.Animate(curve.Duration(), null, delegate(float t)
		{
			float t2 = curve.EvaluateNormalized(t);
			float y = Mathf.LerpUnclamped(origPos.y, destY, t2);
			piece.transform.position = new Vector3(origPos.x + t * drift, y, origPos.z);
			piece.transform.localScale = FiberAnimation.LerpNoClamp(Vector3.one, wobbleScale, Mathf.Sin(t * 20f));
		}, false);
		piece.transform.localScale = Vector3.zero;
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", piece.transform.position, piece.gameObject.layer, new object[0]);
		yield break;
	}
}
