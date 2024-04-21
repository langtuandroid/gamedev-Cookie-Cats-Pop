using System;

public class CloudPiece : CPPiece
{
	public override void Hit(IHitResolver resolver)
	{
		if (resolver.Hit.cause == HitCause.DirectHit)
		{
			resolver.MarkForRemoval(0f, -1);
		}
		else
		{
			base.Hit(resolver);
		}
	}
}
