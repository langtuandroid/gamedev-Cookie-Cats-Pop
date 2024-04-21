using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class SquidModule : LogicModule
{
	public override void Begin(LevelSession session)
	{
		this.session = session;
		foreach (Tile tile in session.TurnLogic.Board.GetOccupiedTiles())
		{
			SquidPiece squidPiece = tile.FindPiece<SquidPiece>();
			if (squidPiece != null)
			{
				this.squidPieces.Add(squidPiece);
			}
		}
		if (this.squidPieces.Count > 0)
		{
			session.TurnLogic.ShotFired += this.Session_TurnLogic_ShotFired;
			session.TurnLogic.HitsMarked += this.Session_TurnLogic_HitsMarked;
			session.TurnLogic.PieceCleared += this.Session_TurnLogic_PieceCleared;
			session.TurnLogic.ForceSquidRetreat += this.Session_TurnLogic_ForceSquidRetreat;
		}
	}

	public override void End(LevelSession session)
	{
		session.TurnLogic.ShotFired -= this.Session_TurnLogic_ShotFired;
		session.TurnLogic.HitsMarked -= this.Session_TurnLogic_HitsMarked;
		session.TurnLogic.PieceCleared -= this.Session_TurnLogic_PieceCleared;
		session.TurnLogic.ForceSquidRetreat -= this.Session_TurnLogic_ForceSquidRetreat;
	}

	private void Session_TurnLogic_ForceSquidRetreat(SquidPiece squid)
	{
		HashSet<Tile> validRetreatTiles = this.GetValidRetreatTiles();
		this.Retreat(squid, validRetreatTiles);
	}

	private void Session_TurnLogic_PieceCleared(CPPiece piece, int points, HitMark hitMark)
	{
		if (piece is SquidPiece)
		{
			this.squidPieces.Remove(piece as SquidPiece);
		}
	}

	private bool IsSquidPieceVisible(SquidPiece squidPiece, int lowestRow)
	{
		int num = lowestRow - squidPiece.GetTile().Coord.y;
		return num < 11;
	}

	private void Session_TurnLogic_ShotFired(TurnLogic.Shot[] shots, ResolveState resolveState)
	{
		this.resolveState = resolveState;
	}

	public override void TurnCompleted(LevelSession session)
	{
		if (this.squidPieces.Count == 0)
		{
			return;
		}
		int lowestRowIndex = session.TurnLogic.Board.GetLowestRowIndex();
		float num = 0f;
		foreach (SquidPiece squidPiece in this.squidPieces)
		{
			bool flag = this.resolveState != null && this.resolveState.IsPieceMarkedForRemoval(squidPiece);
			if (!flag)
			{
				switch (squidPiece.CurrentState)
				{
				case SquidPiece.State.Idle:
					if (this.IsSquidPieceVisible(squidPiece, lowestRowIndex))
					{
						this.SpreadInk(squidPiece, num += 0.1f);
					}
					break;
				case SquidPiece.State.Stunned:
					squidPiece.CurrentState = SquidPiece.State.Idle;
					squidPiece.Laugh();
					break;
				case SquidPiece.State.DirectHit:
					squidPiece.CurrentState = SquidPiece.State.Stunned;
					break;
				case SquidPiece.State.ClusterHit:
					squidPiece.CurrentState = SquidPiece.State.Idle;
					squidPiece.Dizzy();
					break;
				}
			}
		}
	}

	private bool IsTileAvailableForRetreat(Tile tile)
	{
		if (tile.Piece == null)
		{
			return false;
		}
		if (tile.FindPiece<SquidPiece>() != null)
		{
			return false;
		}
		if (this.resolveState.IsPieceMarkedForRemoval(tile.Piece))
		{
			return false;
		}
		InkPiece inkPiece = tile.FindPiece<InkPiece>();
		return !(inkPiece == null) && !this.resolveState.IsPieceMarkedForRemoval(inkPiece);
	}

	private void Session_TurnLogic_HitsMarked()
	{
		HashSet<Tile> validRetreatTiles = this.GetValidRetreatTiles();
		for (int i = 0; i < this.squidPieces.Count; i++)
		{
			SquidPiece squidPiece = this.squidPieces[i];
			if ((squidPiece.CurrentState == SquidPiece.State.DirectHit || squidPiece.CurrentState == SquidPiece.State.ClusterHit || squidPiece.CurrentState == SquidPiece.State.PowerHit) && this.Retreat(squidPiece, validRetreatTiles))
			{
				this.squidPieces.RemoveAt(i);
				i--;
			}
		}
	}

	private HashSet<Tile> GetValidRetreatTiles()
	{
		HashSet<Tile> hashSet = new HashSet<Tile>();
		foreach (Tile tile in this.session.TurnLogic.Board.GetOccupiedTiles())
		{
			if (this.IsTileAvailableForRetreat(tile))
			{
				hashSet.Add(tile);
			}
		}
		return hashSet;
	}

	private bool Retreat(SquidPiece squid, HashSet<Tile> inkTiles)
	{
		Tile tile = (squid.CurrentState != SquidPiece.State.PowerHit) ? this.FindRetreatTile(squid, inkTiles) : Tile.Invalid;
		if (tile != Tile.Invalid)
		{
			inkTiles.Remove(tile);
			this.runner.Run(this.AnimateRetreat(squid, tile), false);
			return false;
		}
		this.runner.Run(this.KillSquid(squid), false);
		int num = (squid.CurrentState != SquidPiece.State.PowerHit) ? squid.PointsForRemoving : 200;
		this.session.TurnLogic.pointsThisTurn.totalPoints += num;
		this.session.TurnLogic.pointsThisTurn.areaOfGivenPoints.Encapsulate(squid.transform.position);
		SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("DriftingPoints", Vector3.zero, squid.gameObject.layer, new object[]
		{
			num.ToString(),
			squid.MatchFlag
		});
		spawnedEffect.transform.position = squid.transform.position + Vector3.back * 6f;
		spawnedEffect.transform.parent = this.session.NonMovingEffectRoot;
		return true;
	}

	private Tile FindRetreatTile(SquidPiece p, HashSet<Tile> inkTiles)
	{
		Tile origin = p.GetTile();
		Coord squidLocation = origin.Coord;
		int num = 16;
		List<Tile> list = new List<Tile>();
		foreach (Tile item in inkTiles)
		{
			Coord coord = item.Coord - squidLocation;
			int num2 = coord.x * coord.x + coord.y * coord.y;
			if (num2 <= num)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			list.Sort(delegate(Tile a, Tile b)
			{
				Coord coord2 = a.Coord - squidLocation;
				int num3 = coord2.x * coord2.x + coord2.y * coord2.y;
				Coord coord3 = b.Coord - squidLocation;
				int num4 = coord3.x * coord3.x + coord3.y * coord3.y;
				if (num3 == num4)
				{
					Vector2 from = a.LocalPosition - origin.LocalPosition;
					Vector2 from2 = b.LocalPosition - origin.LocalPosition;
					float num5 = Vector2.Angle(from, p.RetreatDir);
					float value = Vector2.Angle(from2, p.RetreatDir);
					return num5.CompareTo(value);
				}
				return num3.CompareTo(num4);
			});
			return list[0];
		}
		return Tile.Invalid;
	}

	private IEnumerator KillSquid(SquidPiece squid)
	{
		squid.DetachFromBoard();
		yield return FiberHelper.RunDelayed(squid.HitTime, new Action(squid.Death));
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		if (squid == null)
		{
			yield break;
		}
		yield return Floaters.AnimateFallingPiece(squid, null, delegate(float t, Vector3 position, float rotation)
		{
			if (t < 0.2f)
			{
				rotation = 0f;
			}
			else
			{
				rotation += (t - 0.2f) * 1.25f * 180f;
			}
			return new Vector4(position.x, position.y, position.z, rotation);
		});
		yield break;
	}

	private void ActivateSpring(Tile tile, float amount)
	{
		foreach (Piece piece in tile.Pieces)
		{
			CPPiece cppiece = piece as CPPiece;
			cppiece.ActivateSpring(piece.transform.position + Vector3.up, amount);
		}
	}

	private IEnumerator AnimateRetreat(SquidPiece squid, Tile target)
	{
		squid.TileIndex = target.Index;
		yield return new Fiber.OnExit(delegate()
		{
			squid.AlignToTile();
			Tile tile = squid.GetTile();
			this.ActivateSpring(tile, 10f);
			foreach (Tile tile2 in tile.GetNeighbours())
			{
				this.ActivateSpring(tile2, 5f);
			}
		});
		if (squid.HitTime > 0f)
		{
			yield return FiberHelper.Wait(squid.HitTime, (FiberHelper.WaitFlag)0);
		}
		squid.Hit();
		squid.DisableSpring();
		Vector3 from = squid.transform.localPosition;
		Vector3 dest = target.LocalPosition;
		yield return FiberAnimation.Animate(0.3f, delegate(float t)
		{
			squid.transform.localPosition = Vector3.Lerp(from, dest, t) + Vector3.up * Mathf.Sin(t * 3.14159274f) * 100f;
		});
		yield break;
	}

	private bool CanTileBeInked(Tile tile)
	{
		return !(tile == Tile.Invalid) && !(tile.Piece == null) && tile.Piece is NormalPiece && !(tile.Piece is GoalPiece) && !(tile.FindPiece<PropellerPiece>() != null) && tile.FindPiece<InkPiece>() == null && tile.FindPiece<SquidPiece>() == null;
	}

	private Tile FindTileToInk(Tile origin, int depth, HashSet<Tile> tilesTried)
	{
		if (depth > 3)
		{
			return Tile.Invalid;
		}
		foreach (Direction dir in SquidModule.sortedDirections)
		{
			Tile neighbour = origin.GetNeighbour(dir);
			if (!tilesTried.Contains(neighbour))
			{
				tilesTried.Add(neighbour);
				if (this.CanTileBeInked(neighbour))
				{
					return neighbour;
				}
			}
		}
		foreach (Direction dir2 in SquidModule.sortedDirections)
		{
			Tile neighbour2 = origin.GetNeighbour(dir2);
			if (!(neighbour2 == Tile.Invalid))
			{
				Tile tile = this.FindTileToInk(neighbour2, depth + 1, tilesTried);
				if (tile != Tile.Invalid)
				{
					return tile;
				}
			}
		}
		return Tile.Invalid;
	}

	private void SpreadInk(SquidPiece p, float delay)
	{
		Tile tile = p.GetTile();
		HashSet<Tile> tilesTried = new HashSet<Tile>();
		Tile tile2 = this.FindTileToInk(tile, 0, tilesTried);
		if (tile2 != Tile.Invalid)
		{
			Piece piece = this.session.TurnLogic.Board.SpawnPieceAt(tile2.Index, PieceId.Create<InkPiece>(string.Empty));
			p.TileIndex = tile2.Index;
			piece.transform.localScale = Vector3.zero;
			this.runner.Run(this.WaitAndJump(p, tile2, delay, piece), false);
		}
	}

	private IEnumerator WaitAndJump(SquidPiece squid, Tile target, float delay, Piece ink)
	{
		yield return new Fiber.OnExit(delegate()
		{
			if (squid == null)
			{
				return;
			}
			squid.IsJumping = false;
			squid.AlignToTile();
			ink.transform.localScale = Vector3.one;
			SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("SquidLandEffect", squid.transform.position, squid.gameObject.layer, new object[0]);
			spawnedEffect.transform.position = ink.transform.position;
			Tile tile = squid.GetTile();
			this.ActivateSpring(tile, 10f);
			foreach (Tile tile2 in tile.GetNeighbours())
			{
				this.ActivateSpring(tile2, 5f);
			}
		});
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		squid.Jump(1.4f);
		yield return FiberHelper.Wait(0.2857143f, (FiberHelper.WaitFlag)0);
		if (squid == null)
		{
			yield break;
		}
		squid.DisableSpring();
		squid.IsJumping = true;
		Vector3 start = squid.transform.localPosition;
		Vector3 end = target.LocalPosition;
		yield return FiberAnimation.Animate(0.7047619f, delegate(float t)
		{
			if (squid == null)
			{
				return;
			}
			Vector3 localPosition = Vector3.Lerp(start, end, t);
			localPosition.y += Mathf.Sin(t * 3.14159274f) * 100f;
			squid.transform.localPosition = localPosition;
		});
		yield break;
	}

	private static readonly List<Direction> sortedDirections = new List<Direction>
	{
		BubbleTopology.LeftDown,
		BubbleTopology.RightDown,
		BubbleTopology.Left,
		BubbleTopology.Right,
		BubbleTopology.LeftUp,
		BubbleTopology.RightUp
	};

	private readonly List<SquidPiece> squidPieces = new List<SquidPiece>();

	private readonly FiberRunner runner = new FiberRunner();

	private LevelSession session;

	private ResolveState resolveState;
}
