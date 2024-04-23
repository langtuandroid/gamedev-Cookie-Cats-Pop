using System;
using Fibers;

public class PowerPieceHorizontal : PowerPieceBase
{
	private void OnEnable()
	{
		this.animFiber = new Fiber(this.spine.PlayLoopBetweenEvents("FrogIntoBubble", "Frog_Idle_Start", "Frog_Idle_End", -1f), FiberBucket.Manual);
	}

	protected override void Update()
	{
		base.Update();
		if (this.animFiber != null)
		{
			this.animFiber.Step();
		}
	}

	protected override void TriggerPower(Tile origin, IHitResolver resolver)
	{
		PowerCombinationLogic.DoPower(new PowerCombination(new PowerColor[]
		{
			PowerColor.Green
		}), origin, resolver);
	}

	public SkeletonAnimation spine;

	private Fiber animFiber = new Fiber();
}
