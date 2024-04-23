using System;
using System.Collections;
using Fibers;
using Shared.PiggyBank.Module.Interfaces;
using Spine;
using UnityEngine;

namespace TactileModules.PuzzleGame.PiggyBank.UI
{
	public class PiggyBankLevelDotIndicator : ExtensibleVisual<IPiggyBankLevelDotIndicatorExtension>
	{
		private void Start()
		{
			PiggyBankLevelDotIndicator.current = this;
			if (base.Extension != null)
			{
				TrackEntry trackEntry = base.Extension.PlayAnimation(this.animationName);
				trackEntry.End += this.EndHandler;
			}
		}

		private void EndHandler(Spine.AnimationState state, int trackIndex)
		{
			this.fiber.Terminate();
			this.fiber.Start(this.WaitAndRunAnimation(this.timeBetweenAnimations));
		}

		private IEnumerator WaitAndRunAnimation(float waitTime)
		{
			yield return FiberHelper.Wait(waitTime, (FiberHelper.WaitFlag)0);
			if (base.Extension != null)
			{
				TrackEntry trackEntry = base.Extension.PlayAnimation(this.animationName);
				trackEntry.End += this.EndHandler;
			}
			yield break;
		}

		private void OnDestroy()
		{
			this.fiber.Terminate();
			PiggyBankLevelDotIndicator.current = null;
		}

		public static PiggyBankLevelDotIndicator current;

		[SerializeField]
		private float timeBetweenAnimations = 5f;

		[SerializeField]
		private string animationName = "PiggieBankIdleWacking";

		private readonly Fiber fiber = new Fiber();
	}
}
