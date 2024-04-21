using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floaters
{
	public Floaters(LevelSession session)
	{
		this.session = session;
	}

	public IEnumerator ResolveTileList(ResolveState resolveState, List<int> floating)
	{
		GameBoard board = this.session.TurnLogic.Board;
		if (floating.Count != 0)
		{
			HashSet<Piece> hashSet = new HashSet<Piece>();
			foreach (int index in floating)
			{
				Tile tile = board.GetTile(index);
				float delay = (float)Mathf.Abs(tile.Coord.x - 6) * 0.05f;
				if (!(tile.Piece == null))
				{
					IHitResolver hitResolver = resolveState.CreatePieceResolver(tile.Piece as CPPiece);
					hitResolver.MarkForRemoval(0f, 20);
					hitResolver.QueueAnimation(Floaters.AnimateFallingPiece(tile.Piece as CPPiece, this.session.NonMovingEffectRoot, null), delay);
					hashSet.Add(tile.Piece);
					foreach (Piece piece in tile.PieceAttachments)
					{
						IHitResolver hitResolver2 = resolveState.CreatePieceResolver(piece as CPPiece);
						hitResolver2.MarkForRemoval(0f, 20);
						hitResolver2.QueueAnimation(Floaters.AnimateFallingPiece(piece as CPPiece, this.session.NonMovingEffectRoot, null), delay);
						hashSet.Add(tile.Piece);
					}
				}
			}
			foreach (Piece piece2 in hashSet)
			{
				piece2.DetachFromBoard();
			}
		}
		yield return resolveState.ApplyAllHits(null);
		yield break;
	}

	public IEnumerator Resolve(ResolveState resolveState)
	{
		GameBoard board = this.session.TurnLogic.Board;
		HashSet<Piece> piecesToDetach = new HashSet<Piece>();
		List<int> clouds = Clusters.FindUnattachedClouds(board);
		foreach (int index in clouds)
		{
			Tile tile = board.GetTile(index);
			IHitResolver hitResolver = resolveState.CreatePieceResolver(tile.Piece as CPPiece);
			hitResolver.MarkForRemoval(0f, 20);
			hitResolver.QueueAnimation((tile.Piece as CPPiece).AnimatePop(), 0f);
			piecesToDetach.Add(tile.Piece);
			foreach (Piece piece in tile.PieceAttachments)
			{
				IHitResolver hitResolver2 = resolveState.CreatePieceResolver(piece as CPPiece);
				hitResolver2.MarkForRemoval(0f, 20);
				piecesToDetach.Add(piece);
			}
		}
		List<int> floating = Clusters.FindUnattachedTilesSemiOptimized(board, clouds);
		if (floating.Count != 0)
		{
			foreach (int index2 in floating)
			{
				Tile tile2 = board.GetTile(index2);
				float delay = (float)Mathf.Abs(tile2.Coord.x - 6) * 0.05f;
				if (!(tile2.Piece == null))
				{
					IHitResolver hitResolver3 = resolveState.CreatePieceResolver(tile2.Piece as CPPiece);
					hitResolver3.MarkForRemoval(0f, 20);
					hitResolver3.QueueAnimation(Floaters.AnimateFallingPiece(tile2.Piece as CPPiece, this.session.NonMovingEffectRoot, null), delay);
					piecesToDetach.Add(tile2.Piece);
					foreach (Piece piece2 in tile2.PieceAttachments)
					{
						IHitResolver hitResolver4 = resolveState.CreatePieceResolver(piece2 as CPPiece);
						hitResolver4.MarkForRemoval(0f, 20);
						hitResolver4.QueueAnimation(Floaters.AnimateFallingPiece(piece2 as CPPiece, this.session.NonMovingEffectRoot, null), delay);
						piecesToDetach.Add(piece2);
					}
				}
			}
		}
		foreach (Piece piece3 in piecesToDetach)
		{
			piece3.DetachFromBoard();
		}
		yield return resolveState.ApplyAllHits(null);
		yield break;
	}

	public static IEnumerator AnimateFallingPiece(CPPiece p, Transform effectParent = null, Func<float, Vector3, float, Vector4> transformFunction = null)
	{
		p.DisableSpring();
		Vector3 org = p.transform.localPosition;
		if (!(p is CloudPiece) && !(p is GoalPiece) && !(p is PropellerPiece))
		{
			float drift = UnityEngine.Random.Range(-100f, 100f);
			float randomY = (float)UnityEngine.Random.Range(0, 80);
			float destY = p.transform.parent.InverseTransformPoint(Floaters.FloorWorldPositionFunction()).y + randomY;
			float duration = (org.y - destY) * 0.001f;
			AnimationCurve curve = SingletonAsset<LevelVisuals>.Instance.fallingCurve;
			float rot = p.transform.localRotation.eulerAngles.z;
			yield return FiberAnimation.Animate(duration, null, delegate(float t)
			{
				destY = p.transform.parent.InverseTransformPoint(Floaters.FloorWorldPositionFunction()).y + randomY;
				float num = curve.EvaluateNormalized(t);
				float y = Mathf.LerpUnclamped(org.y, destY, num);
				Vector3 vector = new Vector3(org.x + t * drift, y, org.z);
				float num2 = rot + num * drift;
				if (transformFunction != null)
				{
					Vector4 vector2 = transformFunction(t, vector, num2);
					vector = new Vector3(vector2.x, vector2.y, vector2.z);
					num2 = vector2.w;
				}
				p.transform.localPosition = vector;
				p.transform.localRotation = Quaternion.Euler(0f, 0f, num2);
			}, false);
		}
		Vector3 pos = p.transform.position;
		pos.z = org.z - 5f;
		SpawnedEffect effect = EffectPool.Instance.SpawnEffect("BubblePopSmoke", pos, p.gameObject.layer, new object[0]);
		effect.transform.position = pos;
		for (int i = 0; i < 2; i++)
		{
			yield return null;
		}
		p.transform.localScale = Vector3.zero;
		effect.transform.parent = effectParent;
		yield break;
	}

	private LevelSession session;

	public static Func<Vector3> FloorWorldPositionFunction;
}
