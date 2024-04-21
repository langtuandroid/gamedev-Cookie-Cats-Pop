using System;
using System.Collections;
using Fibers;
using Spine;
using UnityEngine;

public class WalkingKitten : MonoBehaviour
{
	public void Step()
	{
		this.animFiber.Step();
	}

	public void Initialize(Func<Vector3> targetFunction)
	{
		this.targetFunction = targetFunction;
		this.kittenIndex = UnityEngine.Random.Range(1, 4);
		this.animFiber.Start(this.Animate());
	}

	private void SetAnim(string postFix)
	{
		TrackEntry trackEntry = this.spine.PlayAnimation(0, this.prefix + this.kittenIndex + postFix, postFix == this.idle, false);
		if (postFix == this.happy)
		{
			this.spine.state.AddAnimation(0, this.prefix + this.kittenIndex + this.idle, true, trackEntry.endTime - trackEntry.mixDuration);
		}
	}

	private IEnumerator Animate()
	{
		base.transform.localPosition = this.targetFunction();
		this.SetAnim(this.idle);
		yield return FiberHelper.Wait(UnityEngine.Random.value * 0.3f, (FiberHelper.WaitFlag)0);
		for (;;)
		{
			this.SetAnim(this.happy);
			yield return FiberHelper.Wait((float)UnityEngine.Random.Range(2, 5), (FiberHelper.WaitFlag)0);
			yield return this.AnimateWalk(base.transform.localPosition, this.targetFunction());
		}
		yield break;
	}

	public IEnumerator AnimateWalk(Vector3 localFrom, Vector3 localTo)
	{
		float distanceChunks = (localFrom - localTo).magnitude / SingletonAsset<LevelVisuals>.Instance.kittenWalkStepLength;
		int numBounces = Mathf.CeilToInt(distanceChunks);
		float duration = distanceChunks * 0.3f;
		Vector3 normalScale = new Vector3(1f, 1f, 1f);
		Vector3 squashScale = new Vector3(0.8f, 1.2f, 1f);
		yield return FiberAnimation.Animate(duration, delegate(float v)
		{
			float d = Mathf.Abs(Mathf.Sin(v * 3.14159274f * (float)numBounces));
			float t = Mathf.Abs(Mathf.Sin(v * 3.14159274f * (float)numBounces - 0.4f));
			this.transform.localPosition = FiberAnimation.LerpNoClamp(localFrom, localTo, v);
			this.feetPivot.localPosition = Vector3.up * d * SingletonAsset<LevelVisuals>.Instance.kittenWalkHeight;
			this.feetPivot.localScale = Vector3.Lerp(normalScale, squashScale, t);
		});
		this.feetPivot.localScale = normalScale;
		yield break;
	}

	public Transform feetPivot;

	public SkeletonAnimation spine;

	public string prefix = "Kitten";

	public string happy = "_Happy";

	public string idle = "_Idle";

	public string surprised = "_Surprised";

	private Fiber animFiber = new Fiber(FiberBucket.Manual);

	private int kittenIndex;

	private Func<Vector3> targetFunction;
}
