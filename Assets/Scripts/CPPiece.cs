using System;
using System.Collections;
using UnityEngine;

public class CPPiece : Piece
{
	public virtual bool PreventFurtherClustering
	{
		get
		{
			return false;
		}
	}

	public virtual bool PreventClustering
	{
		get
		{
			return false;
		}
	}

	public virtual bool BlockHitsOnPiecesUnderneath
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanMoveBySpring
	{
		get
		{
			return true;
		}
	}

	public virtual int PointsForRemoving
	{
		get
		{
			return 10;
		}
	}

	public Tile GetTile()
	{
		return Piece.StaticBoard.GetTile(base.TileIndex);
	}

	protected override void OnSpawned()
	{
		base.OnSpawned();
		UISprite componentInChildren = base.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.Color = Color.white;
		}
		this.doSpring = false;
	}

	public virtual void Hit(IHitResolver resolver)
	{
		if (resolver.Hit.cause == HitCause.Cluster)
		{
			foreach (Tile at in this.GetTile().GetNeighbours())
			{
				resolver.MarkHit(at, HitCause.ClusterSplash, 0.2f);
			}
			resolver.QueueAnimation(this.AnimatePop(), 0f);
			resolver.MarkForRemoval(0f, -1);
		}
		if (resolver.Hit.cause == HitCause.Power)
		{
			resolver.QueueAnimation(this.AnimateHide(), 0f);
			resolver.MarkForRemoval(0f, -1);
		}
	}

	public void RemoveAfterClustersIfAble(ResolveState resolveState)
	{
		if (this.RemovableAfterClusters())
		{
			IHitResolver hitResolver = resolveState.CreatePieceResolver(this);
			hitResolver.MarkForRemoval(0f, 0);
			hitResolver.QueueAnimation(this.AnimatePop(), 0f);
		}
	}

	public void RemoveIfAtBottom(ResolveState resolveState)
	{
		bool flag = this.GetTile().Coord.y >= BossLevelDatabase.Database.maxRowsCount;
		if (flag)
		{
			IHitResolver hitResolver = resolveState.CreatePieceResolver(this);
			hitResolver.MarkForRemoval(0f, 0);
			hitResolver.QueueAnimation(this.AnimatePop(), 0f);
		}
	}

	protected virtual bool RemovableAfterClusters()
	{
		foreach (Tile tile in this.GetTile().GetNeighbours())
		{
			if (tile.Piece is SpikePiece)
			{
				return true;
			}
		}
		return false;
	}

	protected IEnumerator AnimateHide()
	{
		base.gameObject.SetActive(false);
		yield break;
	}

	public virtual IEnumerator AnimatePop()
	{
		yield return FiberAnimation.ScaleTransform(base.transform, Vector3.one, Vector3.one * 1.3f, null, 0.1f);
		base.transform.localScale = Vector3.zero;
		EffectPool.Instance.SpawnEffect("BubblePopSmoke", base.transform.position, base.gameObject.layer, new object[0]);
		yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	public override void AlignToTile()
	{
		base.AlignToTile();
		this.restingPos = base.transform.localPosition;
		this.velocity = Vector3.zero;
		this.DisableSpring();
	}

	public void ActivateSpring(Vector3 otherWP, float force)
	{
		if (!this.CanMoveBySpring)
		{
			return;
		}
		Vector3 vector = otherWP - base.transform.position;
		base.transform.localPosition = this.restingPos - vector.normalized * force * SingletonAsset<LevelVisuals>.Instance.piecesSpringForce;
		this.doSpring = true;
	}

	public void DisableSpring()
	{
		this.doSpring = false;
	}

	protected virtual void Update()
	{
		if (this.CanMoveBySpring && this.doSpring && base.TileIndex >= 0)
		{
			Vector3 vector = base.transform.localPosition;
			Vector3 a = this.restingPos - vector - this.velocity * SingletonAsset<LevelVisuals>.Instance.piecesSpringFriction;
			this.velocity += a * Time.deltaTime * SingletonAsset<LevelVisuals>.Instance.piecesSpringCoefficient;
			vector += this.velocity * Time.deltaTime;
			base.transform.localPosition = vector;
			if (a.magnitude < 1f && this.velocity.magnitude < 1f)
			{
				this.doSpring = false;
			}
		}
	}

	public virtual void Initialize(LevelSession session)
	{
	}

	protected const int FRONT_LAYER = 1;

	private Vector3 restingPos;

	private Vector3 velocity;

	private bool doSpring;
}
