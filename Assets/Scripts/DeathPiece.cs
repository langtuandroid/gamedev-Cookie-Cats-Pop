using System;
using System.Collections;

public class DeathPiece : CPPiece
{
	public override void Hit(IHitResolver resolver)
	{
		if (resolver.Hit.cause == HitCause.DirectHit)
		{
			resolver.MarkForRemoval(0f, -1);
			resolver.QueueAnimation(this.AnimateSplatter(), 0f);
		}
		else
		{
			base.Hit(resolver);
		}
	}

	private IEnumerator AnimateSplatter()
	{
		EffectPool.Instance.SpawnEffect("InkSplatter", base.transform.position, base.gameObject.layer, new object[0]);
		yield break;
	}
}
