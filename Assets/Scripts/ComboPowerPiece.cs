using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class ComboPowerPiece : PowerPieceBase
{
	private new void Update()
	{
		if (this.animFiber != null)
		{
			this.animFiber.Step();
		}
	}

	public override void Initialize(LevelSession session)
	{
		this.SetPowerCombination(session.Powers.CurrentCombination);
	}

	public void SetPowerCombination(PowerCombination powerCombination)
	{
		this.currentCombination = powerCombination;
		if (this.currentCombination == PowerCombination.All)
		{
			this.finalPowerPivot.SetActive(true);
			this.yellowPivot.SetActive(false);
			this.redPivot.SetActive(false);
			this.bluePivot.SetActive(false);
			this.greenPivot.SetActive(false);
		}
		else
		{
			this.finalPowerPivot.SetActive(false);
			this.yellowPivot.SetActive(this.currentCombination.IsColorEnabled(PowerColor.Yellow));
			this.redPivot.SetActive(this.currentCombination.IsColorEnabled(PowerColor.Red));
			this.bluePivot.SetActive(this.currentCombination.IsColorEnabled(PowerColor.Blue));
			this.greenPivot.SetActive(this.currentCombination.IsColorEnabled(PowerColor.Green));
			this.fireNote.SetActive(this.currentCombination.IsColorEnabled(PowerColor.Red) && !this.currentCombination.IsColorEnabled(PowerColor.Green));
		}
		List<IEnumerator> list = new List<IEnumerator>();
		if (this.greenPivot.activeSelf)
		{
			if (!this.redPivot.activeSelf)
			{
				this.normalFrog.gameObject.SetActive(true);
				this.fireFrog.gameObject.SetActive(false);
				list.Add(this.normalFrog.PlayLoopBetweenEvents("FrogIntoBubble", "Frog_Idle_Start", "Frog_Idle_End", -1f));
			}
			else
			{
				this.normalFrog.gameObject.SetActive(false);
				this.fireFrog.gameObject.SetActive(true);
				list.Add(this.fireFrog.PlayLoopBetweenEvents("FrogIntoBubble", "Frog_Idle_Start", "Frog_Idle_End", -1f));
			}
		}
		if (list.Count > 0)
		{
			this.animFiber = new Fiber(FiberHelper.RunParallel(list.ToArray()), FiberBucket.Manual);
		}
	}

	protected override void TriggerPower(Tile origin, IHitResolver resolver)
	{
		PowerCombinationLogic.DoPower(this.currentCombination, origin, resolver);
	}

	public GameObject yellowPivot;

	public GameObject redPivot;

	public GameObject bluePivot;

	public GameObject greenPivot;

	public GameObject finalPowerPivot;

	public SkeletonAnimation normalFrog;

	public SkeletonAnimation fireFrog;

	public GameObject fireNote;

	private PowerCombination currentCombination;

	private Fiber animFiber;

	private static Dictionary<PowerCombination, Action<Tile, IHitResolver>> combinationLogic;
}
