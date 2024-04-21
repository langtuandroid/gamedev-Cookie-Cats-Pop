using System;
using System.Collections;

public class FrozenPiece : CPPiece
{
	public override MatchFlag MatchFlag
	{
		get
		{
			return string.Empty;
		}
	}

	public override void Hit(IHitResolver resolver)
	{
		HitMark hit = resolver.Hit;
		if (hit.cause == HitCause.ClusterSplash && hit.causedBy.MatchFlag != string.Empty)
		{
			PieceId newId = PieceId.Create<NormalPiece>(hit.causedBy.MatchFlag);
			resolver.MarkForReplacement(newId, 0f);
			resolver.QueueAnimation(this.AnimateShatter(), 0f);
		}
		if (hit.cause == HitCause.Power)
		{
			base.Hit(resolver);
		}
	}

	private IEnumerator AnimateShatter()
	{
		SpawnedEffect spawnedEffect = EffectPool.Instance.SpawnEffect("IceExplode", base.transform.position, base.gameObject.layer, new object[0]);
		ZLayer.EffectOverlay.AdjustTransform(spawnedEffect.transform);
		yield break;
	}
}
