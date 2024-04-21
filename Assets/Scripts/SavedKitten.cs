using System;
using System.Collections;
using Fibers;
using Spine;
using UnityEngine;

public class SavedKitten : MonoBehaviour
{
	public bool IsLanded { get; private set; }

	public void Initialize(SavedKittens owner, Vector3 position)
	{
		this.owner = owner;
		base.gameObject.SetLayerRecursively(owner.gameObject.layer);
		base.transform.parent = owner.transform;
		base.transform.localPosition = Vector3.zero;
		this.kittenIndex = UnityEngine.Random.Range(1, 4);
		this.logicFiber.Start(this.Logic(position));
		SavedKittens savedKittens = this.owner;
		savedKittens.onUpdate = (Action)Delegate.Combine(savedKittens.onUpdate, new Action(delegate()
		{
			if (this.shouldPlayIdle)
			{
				this.shouldPlayIdle = false;
				this.SetAnim(this.idle);
			}
			this.logicFiber.Step();
		}));
		this.spine.gameObject.SetActive(false);
		this.parachuteKitten.gameObject.SetActive(true);
		this.shadow.gameObject.SetActive(false);
	}

	private void SetAnim(string postFix)
	{
		TrackEntry trackEntry = this.spine.PlayAnimation(0, this.prefix + this.kittenIndex + postFix, postFix == this.idle, true);
		if (postFix == this.happy)
		{
			trackEntry.End += delegate(Spine.AnimationState state, int trackIndex)
			{
				this.shouldPlayIdle = true;
			};
		}
		this.spine.Update(0.001f);
	}

	private IEnumerator PlayParachuteLandAnim(float duration)
	{
		yield return this.parachuteKitten.PlayTimeline("Dropdown", this.parachuteKitten.state.GetCurrent(0).time, 2.66666675f, duration, 1f, null);
		yield break;
	}

	private IEnumerator PositionShadowToWorldPosition(Vector3 worldPosition, float duration)
	{
		while (duration > 0f)
		{
			duration -= Time.deltaTime;
			this.shadowPivot.position = worldPosition;
			yield return null;
		}
		yield break;
	}

	private IEnumerator Logic(Vector3 fromWP)
	{
		Vector3 local = this.owner.transform.InverseTransformPoint(fromWP);
		local.z = 0f;
		base.transform.position = local;
		Vector3 dest = this.owner.GetRandomWalkingPosition();
		Vector3 shadowWorldPosition = this.owner.transform.TransformPoint(dest);
		shadowWorldPosition.z = base.transform.position.z;
		this.SetAnim(this.idle);
		float foldInDuration = 0.783333361f;
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberHelper.RunSerial(new IEnumerator[]
			{
				this.parachuteKitten.PlayTimeline("Dropdown", 0f, 1.56666672f, 1.56666672f, 1f, null),
				this.parachuteKitten.PlayLoopBetweenEvents("Dropdown", "Parachute_IdleStart", "Parachute_IdleEnd", this.fallingDuration - 1.56666672f - foldInDuration),
				FiberHelper.RunParallel(new IEnumerator[]
				{
					this.PlayParachuteLandAnim(foldInDuration),
					FiberAnimation.ScaleTransform(this.parachuteScaler, this.parachuteScaler.localScale, Vector3.one, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), foldInDuration),
					UIFiberAnimations.FadeAlpha(this.shadow, 0f, this.shadow.Alpha, foldInDuration, null),
					FiberHelper.RunDelayed(0f, delegate
					{
						this.shadow.gameObject.SetActive(true);
					})
				})
			}),
			FiberAnimation.MoveLocalTransform(base.transform, local, dest, this.fallingCurve, this.fallingDuration),
			this.PositionShadowToWorldPosition(shadowWorldPosition, this.fallingDuration),
			FiberAnimation.MoveLocalTransform(this.parachuteKitten.transform, this.parachuteKitten.transform.localPosition, this.landTarget.transform.localPosition, null, this.fallingDuration)
		});
		this.shadowPivot.localPosition = Vector3.zero;
		this.spine.gameObject.SetActive(true);
		this.parachuteKitten.gameObject.SetActive(false);
		this.IsLanded = true;
		for (;;)
		{
			this.SetAnim(this.happy);
			yield return FiberHelper.Wait((float)UnityEngine.Random.Range(2, 5), (FiberHelper.WaitFlag)0);
			Vector3 f = base.transform.localPosition;
			Vector3 t = this.owner.GetRandomWalkingPosition();
			yield return this.AnimateWalk(f, t);
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
			this.transform.localPosition = FiberAnimation.LerpNoClamp(localFrom, localTo, v) + Vector3.up * d * SingletonAsset<LevelVisuals>.Instance.kittenWalkHeight;
			this.spine.transform.localScale = Vector3.Lerp(normalScale, squashScale, t);
		});
		this.spine.transform.localScale = normalScale;
		yield break;
	}

	private SavedKittens owner;

	private Fiber logicFiber = new Fiber(FiberBucket.Manual);

	private int kittenIndex = 1;

	public AnimationCurve fallingCurve;

	public string prefix = "Kitten";

	public string happy = "_Happy";

	public string idle = "_Idle";

	public string surprised = "_Surprised";

	public float fallingDuration = 2.5f;

	[SerializeField]
	private SkeletonAnimation spine;

	[SerializeField]
	private SkeletonAnimation parachuteKitten;

	[SerializeField]
	private Transform parachuteScaler;

	[SerializeField]
	private Transform shadowPivot;

	[SerializeField]
	private UISprite shadow;

	[SerializeField]
	private Transform landTarget;

	private const float foldOutDuration = 1.56666672f;

	private const string parachuteAnim = "Dropdown";

	private bool shouldPlayIdle;
}
