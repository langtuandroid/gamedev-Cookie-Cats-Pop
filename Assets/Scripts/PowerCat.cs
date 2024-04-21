using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Spine;
using Spine.Unity;
using UnityEngine;

public class PowerCat : MonoBehaviour
{
	public bool ChargedAnimIsLooping { get; protected set; }

	public PowerCat.State CurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	public void Initialize()
	{
		this.animationFiber = new Fiber(FiberBucket.Manual);
		this.collectBowlIsVisible = true;
		this.spine.UpdateComplete += this.HandleUpdateComplete;
		this.overrideBowlIndex = -1;
		this.currentState = PowerCat.State.Idle;
		this.UpdateState();
		this.OnInitialize();
	}

	public void SetState(PowerCat.State newState)
	{
		if (newState == PowerCat.State.Happy && this.currentState == newState)
		{
			return;
		}
		this.currentState = newState;
		this.UpdateState();
	}

	private void UpdateState()
	{
		PowerCat.State state = this.currentState;
		if (state != PowerCat.State.Idle)
		{
			if (state != PowerCat.State.Happy)
			{
				if (state == PowerCat.State.Charged)
				{
					this.overrideBowlIndex = -1;
					this.ChargedAnimIsLooping = false;
					this.animationFiber.Start(this.ChargedState());
				}
			}
			else
			{
				this.animationFiber.Start(this.HappyState());
			}
		}
		else
		{
			this.animationFiber.Start(this.IdleState());
		}
	}

	public void SetBowlFill(float fill)
	{
		this.overrideBowlIndex = Mathf.FloorToInt(fill * (float)(this.cookiesBowlImages.Count - 1));
	}

	private void HandleUpdateComplete(ISkeletonAnimation skeletonRenderer)
	{
		if (this.overrideBowlIndex > -1)
		{
			for (int i = 0; i < this.bowlSlotNames.Count; i++)
			{
				this.spine.skeleton.TryShowSlot(this.bowlSlotNames[i], this.collectBowlIsVisible);
			}
			Slot slot = this.spine.skeleton.FindSlot(this.cookiesBowlSlotName);
			if (slot != null)
			{
				int slotIndex = this.spine.skeleton.FindSlotIndex(this.cookiesBowlSlotName);
				slot.Attachment = this.spine.skeleton.GetAttachment(slotIndex, this.cookiesBowlImages[this.overrideBowlIndex]);
			}
		}
	}

	private void Update()
	{
		if (this.animationFiber != null)
		{
			this.animationFiber.Step();
		}
	}

	private IEnumerator IdleState()
	{
		for (;;)
		{
			this.spine.PlayAnimation(0, this.idleAnim, true, true);
			yield return FiberHelper.Wait((float)UnityEngine.Random.Range(5, 15), (FiberHelper.WaitFlag)0);
			if (this.boredAnims.Count > 0)
			{
				TrackEntry e = this.spine.PlayAnimation(0, this.boredAnims.GetRandom<string>(), false, false);
				yield return FiberHelper.Wait(e.Animation.Duration - this.spine.state.Data.DefaultMix, (FiberHelper.WaitFlag)0);
			}
		}
		yield break;
	}

	private IEnumerator HappyState()
	{
		TrackEntry anim = this.spine.PlayAnimation(0, this.happyAnim, false, false);
		yield return FiberHelper.Wait(anim.endTime - 0.2f, (FiberHelper.WaitFlag)0);
		this.SetState(PowerCat.State.Idle);
		yield return null;
		yield break;
	}

	protected virtual void OnInitialize()
	{
	}

	protected virtual IEnumerator ChargedState()
	{
		yield break;
	}

	public virtual IEnumerator AnimatePowerPiece(PieceId pieceId, Transform targetPivot, Action bubbleSpawned)
	{
		yield break;
	}

	protected void StartAnim(IEnumerator code)
	{
		this.animationFiber.Start(code);
	}

	protected void ScheduleIdle(float delay)
	{
		this.StartAnim(this.WaitAndGotoIdle(delay));
	}

	private IEnumerator WaitAndGotoIdle(float delay)
	{
		yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
		this.SetState(PowerCat.State.Idle);
		yield return null;
		yield break;
	}

	public SkeletonAnimation spine;

	public string idleAnim = "idle";

	public string happyAnim = "happy";

	public string cookiesBowlSlotName = "bowl";

	public List<string> cookiesBowlImages;

	public List<string> bowlSlotNames;

	public List<string> boredAnims = new List<string>();

	private int overrideBowlIndex;

	private bool collectBowlIsVisible;

	protected PowerCat.State currentState;

	private Fiber animationFiber;

	public enum State
	{
		Idle,
		Happy,
		Charged
	}
}
