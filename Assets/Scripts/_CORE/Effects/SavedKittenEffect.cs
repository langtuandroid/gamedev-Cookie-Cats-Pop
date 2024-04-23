using System;
using System.Collections;
using UnityEngine;

public class SavedKittenEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		SavedKittenEffect.Parameters p = parameters[0] as SavedKittenEffect.Parameters;
		float sineWidth = 100f;
		if (p.worldEnd.x < p.worldStart.x)
		{
			sineWidth = -sineWidth;
		}
		float duration = SingletonAsset<ExperienceSettings>.Instance.GetActive().kittenFlyToHudDuration;
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.Animate(duration, this.movementCurve, delegate(float t)
			{
				Vector3 position = FiberAnimation.LerpNoClamp(p.worldStart, p.worldEnd, t);
				position.y -= Mathf.Sin(t * 3.14159274f) * this.sineHeight;
				position.x += Mathf.Sin(t * 3.14159274f) * sineWidth;
				position.z = this.WorldZ;
				this.movePivot.position = position;
			}, false),
			FiberAnimation.ScaleTransform(this.movePivot, Vector3.zero, Vector3.one, this.scaleCurve, duration)
		});
		p.onFinished();
		yield break;
	}

	public Transform movePivot;

	public AnimationCurve movementCurve;

	public float sineHeight = 100f;

	public AnimationCurve scaleCurve;

	public float WorldZ = -225f;

	public class Parameters
	{
		public Vector3 worldStart;

		public Vector3 worldEnd;

		public Action onFinished;
	}
}
