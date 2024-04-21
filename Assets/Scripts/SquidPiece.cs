using System;
using Spine;
using UnityEngine;

public class SquidPiece : CPPiece
{
	public SquidPiece.State CurrentState { get; set; }

	public Vector2 RetreatDir { get; private set; }

	public float HitTime { get; private set; }

	public bool IsJumping { get; set; }

	public override bool IsAttachment
	{
		get
		{
			return true;
		}
	}

	public override int TileLayer
	{
		get
		{
			return 3;
		}
	}

	public override bool PreventFurtherClustering
	{
		get
		{
			return true;
		}
	}

	public override bool BlockHitsOnPiecesUnderneath
	{
		get
		{
			return false;
		}
	}

	public override bool IsRotatable
	{
		get
		{
			return false;
		}
	}

	public override void DetachFromBoard()
	{
		base.DetachFromBoard();
	}

	public override void SpawnedByBoard(Board board)
	{
		base.SpawnedByBoard(board);
		TrackEntry trackEntry = this.spine.PlayAnimation(0, "Idle", true, false);
		trackEntry.time = UnityEngine.Random.value * trackEntry.endTime;
		this.CurrentState = SquidPiece.State.Idle;
		this.IsJumping = false;
	}

	public override void Hit(IHitResolver resolver)
	{
		if (this.CurrentState == SquidPiece.State.PowerHit || this.CurrentState == SquidPiece.State.Dead)
		{
			return;
		}
		if (resolver.Hit.cause == HitCause.DirectHit || resolver.Hit.cause == HitCause.Power || resolver.Hit.cause == HitCause.ClusterSplash)
		{
			HitCause cause = resolver.Hit.cause;
			if (cause != HitCause.ClusterSplash)
			{
				if (cause != HitCause.DirectHit)
				{
					if (cause == HitCause.Power)
					{
						this.CurrentState = SquidPiece.State.PowerHit;
						if (resolver.Hit.causedBy is LockPiece)
						{
							this.CurrentState = SquidPiece.State.ClusterHit;
						}
					}
				}
				else
				{
					this.CurrentState = SquidPiece.State.DirectHit;
				}
			}
			else
			{
				if (this.CurrentState == SquidPiece.State.DirectHit)
				{
					return;
				}
				this.CurrentState = SquidPiece.State.ClusterHit;
			}
			this.HitTime = resolver.Hit.time;
			if (resolver.Hit.causedBy != null)
			{
				Vector3 v = base.transform.position - resolver.Hit.causedBy.transform.position;
				this.RetreatDir = v;
			}
		}
	}

	private void BreakNeighboorChain(Tile origin, Direction dir, IHitResolver resolver, int step)
	{
	}

	private void StartAnimationAndIdle(string animation, float timeScale = 1f)
	{
		TrackEntry trackEntry = this.spine.state.SetAnimation(0, animation, false);
		trackEntry.timeScale = timeScale;
		TrackEntry idleState = this.spine.state.AddAnimation(0, "Idle", true, trackEntry.endTime - trackEntry.mixDuration);
		idleState.Start += delegate(Spine.AnimationState state, int trackIndex)
		{
			idleState.time = UnityEngine.Random.value * idleState.endTime;
		};
	}

	public void Jump(float timeScale)
	{
		if (SingletonAsset<SoundDatabase>.Instance.squidJump != null)
		{
			SingletonAsset<SoundDatabase>.Instance.squidJump.PlaySequential();
		}
		this.StartAnimationAndIdle("jump", timeScale);
	}

	public void Laugh()
	{
		if (SingletonAsset<SoundDatabase>.Instance.squidLaugh != null)
		{
			SingletonAsset<SoundDatabase>.Instance.squidLaugh.PlaySequential();
		}
		this.StartAnimationAndIdle("Laugh", 1f);
	}

	public void Dizzy()
	{
		this.StartAnimationAndIdle("Hit", 1f);
	}

	public void Hit()
	{
		this.spine.state.SetAnimation(0, "Hit", true);
		if (SingletonAsset<SoundDatabase>.Instance.squidHit != null)
		{
			SingletonAsset<SoundDatabase>.Instance.squidHit.PlaySequential();
		}
	}

	public void Death()
	{
		if (this.CurrentState != SquidPiece.State.Dead)
		{
			if (SingletonAsset<SoundDatabase>.Instance.squidDeath != null)
			{
				SingletonAsset<SoundDatabase>.Instance.squidDeath.Play();
			}
			this.spine.state.SetAnimation(0, "Death", false);
			this.CurrentState = SquidPiece.State.Dead;
		}
	}

	public override bool CanMoveBySpring
	{
		get
		{
			return !this.IsJumping;
		}
	}

	public SkeletonAnimation spine;

	private const string animationIdle = "Idle";

	private const string animationHit = "Hit";

	private const string animationLaugh = "Laugh";

	private const string animationJump = "jump";

	private const string animationDeath = "Death";

	public enum State
	{
		Idle,
		Stunned,
		DirectHit,
		ClusterHit,
		PowerHit,
		Dead
	}
}
