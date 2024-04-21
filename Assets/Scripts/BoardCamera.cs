using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class BoardCamera : MonoBehaviour
{
	public void Initialize(TurnLogic turnLogic)
	{
		this.turnLogic = turnLogic;
		this.turnLogic.ShotFired += this.TurnLogic_ShotFired;
		this.focusFiber = new Fiber(FiberBucket.Manual);
		this.moveBackFiber = new Fiber(FiberBucket.Manual);
		this.boosterBarTargetLocations = new Vector3[2];
		this.boosterBarTargetLocations[0] = this.boosterBarPivot.localPosition;
		this.boosterBarTargetLocations[1] = this.boosterBarPivot.localPosition + Vector3.down * 300f;
	}

	public bool IsAnimating
	{
		get
		{
			return !this.focusFiber.IsTerminated || !this.moveBackFiber.IsTerminated;
		}
	}

	private void OnDestroy()
	{
		if (this.focusFiber != null)
		{
			this.focusFiber.Terminate();
		}
		if (this.moveBackFiber != null)
		{
			this.moveBackFiber.Terminate();
		}
	}

	private void Update()
	{
		if (this.focusFiber != null)
		{
			this.focusFiber.Step();
		}
		if (this.moveBackFiber != null)
		{
			this.moveBackFiber.Step();
		}
	}

	private void TurnLogic_ShotFired(TurnLogic.Shot[] shots, ResolveState resolveState)
	{
		this.resolveState = resolveState;
		this.resolveState.AllHitsApplied += this.AllHitsApplied;
		this.didCameraMoveLastTurn = false;
		this.focusFiber.Start(this.MoveCameraToShots(shots));
		this.turnLogic.ResolveCompleted -= this.TurnLogic_ResolveCompleted;
		this.turnLogic.ResolveCompleted += this.TurnLogic_ResolveCompleted;
	}

	private void TurnLogic_ResolveCompleted(TurnLogic turnLogic)
	{
		this.turnLogic.ResolveCompleted -= this.TurnLogic_ResolveCompleted;
		if (this.didCameraMoveLastTurn)
		{
			this.moveBackFiber.Start(this.MoveToBottom());
		}
		this.resolveState.AllHitsApplied -= this.AllHitsApplied;
		this.resolveState = null;
	}

	private void AllHitsApplied(List<HitMark> allHits)
	{
		List<BoardCamera.ClusterTile> list = new List<BoardCamera.ClusterTile>();
		foreach (HitMark hitMark in allHits)
		{
			list.Add(new BoardCamera.ClusterTile(hitMark.piece.GetTile(), hitMark.time));
		}
		if (list.Count > 0)
		{
			this.focusFiber.Push(this.MoveCameraToCluster(list));
		}
	}

	private IEnumerator MoveCameraToShots(TurnLogic.Shot[] shots)
	{
		TurnLogic.Shot shot = this.FindHighestShot(shots);
		if (shot == null)
		{
			yield break;
		}
		yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		Vector3 panTarget;
		if (this.TryGetFocusPosition(shot.hitTile, out panTarget))
		{
			this.didCameraMoveLastTurn = true;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				this.PanCamera(panTarget, float.MaxValue),
				FiberAnimation.MoveLocalTransform(this.boosterBarPivot, this.boosterBarPivot.localPosition, this.boosterBarTargetLocations[1], AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.6f)
			});
		}
		yield break;
	}

	private IEnumerator MoveCameraToCluster(List<BoardCamera.ClusterTile> cluster)
	{
		Tile topMostTile = this.GetTopTileInCluster(cluster);
		if (topMostTile == Tile.Invalid)
		{
			yield break;
		}
		Vector3 topMostPosition;
		if (!this.TryGetFocusPosition(topMostTile, out topMostPosition))
		{
			yield break;
		}
		this.didCameraMoveLastTurn = true;
		cluster.Sort((BoardCamera.ClusterTile a, BoardCamera.ClusterTile b) => a.time.CompareTo(b.time));
		float duration = cluster[cluster.Count - 1].time;
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.FocusCameraOnCluster(cluster, duration),
			FiberAnimation.MoveLocalTransform(this.boosterBarPivot, this.boosterBarPivot.localPosition, this.boosterBarTargetLocations[1], AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), Mathf.Min(0.6f, duration))
		});
		yield break;
	}

	private IEnumerator FocusCameraOnCluster(List<BoardCamera.ClusterTile> cluster, float duration)
	{
		Vector3 panTarget = this.panningRoot.localPosition;
		float time = 0f;
		do
		{
			Tile currentFocusTile;
			Vector3 vector;
			if (this.TryFindClusterTile(cluster, time * 2f, out currentFocusTile) && this.TryGetFocusPosition(currentFocusTile, out vector) && vector.y < panTarget.y)
			{
				panTarget = vector;
			}
			this.panningRoot.localPosition = Vector3.Lerp(this.panningRoot.localPosition, panTarget, 4f * Time.deltaTime);
			yield return null;
			time += Time.deltaTime;
		}
		while (time <= duration);
		yield break;
	}

	private Tile GetTopTileInCluster(List<BoardCamera.ClusterTile> cluster)
	{
		if (cluster.Count <= 0)
		{
			return Tile.Invalid;
		}
		BoardCamera.ClusterTile clusterTile = cluster[0];
		for (int i = 1; i < cluster.Count; i++)
		{
			if (cluster[i].tile.Index < clusterTile.tile.Index)
			{
				clusterTile = cluster[i];
			}
		}
		return clusterTile.tile;
	}

	private bool TryFindClusterTile(List<BoardCamera.ClusterTile> cluster, float time, out Tile tile)
	{
		for (int i = cluster.Count - 1; i >= 0; i--)
		{
			if (cluster[i].time <= time)
			{
				tile = cluster[i].tile;
				return true;
			}
		}
		tile = Tile.Invalid;
		return false;
	}

	private IEnumerator MoveToBottom()
	{
		float timer = 0.7f;
		do
		{
			yield return null;
			timer -= Time.deltaTime;
		}
		while (!this.focusFiber.IsTerminated || timer >= 0f);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.PanCamera(Vector3.zero, 0.5f),
			FiberAnimation.MoveLocalTransform(this.boosterBarPivot, this.boosterBarPivot.localPosition, this.boosterBarTargetLocations[0], AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.6f)
		});
		yield break;
	}

	private IEnumerator PanCamera(Vector3 target, float maxDuration = 3.40282347E+38f)
	{
		Vector3 diff = target - this.panningRoot.localPosition;
		yield return FiberAnimation.MoveLocalTransform(this.panningRoot, this.panningRoot.localPosition, target, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), Mathf.Min(Mathf.Abs(diff.y) / 500f, maxDuration));
		yield break;
	}

	private bool TryGetFocusPosition(Tile tile, out Vector3 focusPosition)
	{
		if (tile == Tile.Invalid)
		{
			focusPosition = Vector3.zero;
			return false;
		}
		Vector3 boardRowNrWorldPosition = this.turnLogic.Board.GetBoardRowNrWorldPosition(tile.Coord.y);
		if (base.transform.InverseTransformPoint(boardRowNrWorldPosition).y > this.GetElementSize().y * 0.5f - 50f)
		{
			Vector3 boardRowNrWorldPosition2 = this.turnLogic.Board.GetBoardRowNrWorldPosition(0);
			Vector3 vector = this.panningRoot.InverseTransformPoint(boardRowNrWorldPosition2);
			Vector3 vector2 = this.panningRoot.InverseTransformPoint(boardRowNrWorldPosition);
			vector2.y -= this.boardBounds.GetElementSize().y - this.turnLogic.Board.TileSizeInParentSpace();
			float b = vector.y - (this.boardBounds.GetElementSize().y - this.turnLogic.Board.TileSizeInParentSpace());
			vector2.y = Mathf.Min(vector2.y, b);
			focusPosition = new Vector3(0f, -vector2.y, 0f);
			return true;
		}
		focusPosition = Vector3.zero;
		return false;
	}

	private TurnLogic.Shot FindHighestShot(TurnLogic.Shot[] shots)
	{
		int num = int.MaxValue;
		TurnLogic.Shot result = null;
		foreach (TurnLogic.Shot shot in shots)
		{
			if (!(shot.hitTile == Tile.Invalid))
			{
				int y = shot.hitTile.Coord.y;
				if (y < num)
				{
					result = shot;
					num = y;
				}
			}
		}
		return result;
	}

	public Transform panningRoot;

	public UIElement boardBounds;

	public Transform boosterBarPivot;

	private TurnLogic turnLogic;

	private Fiber focusFiber;

	private Fiber moveBackFiber;

	private Vector3[] boosterBarTargetLocations;

	private const float topBarHeight = 50f;

	private ResolveState resolveState;

	private bool didCameraMoveLastTurn;

	private struct ClusterTile
	{
		public ClusterTile(Tile tile, float time)
		{
			this.tile = tile;
			this.time = time;
		}

		public Tile tile;

		public float time;
	}
}
