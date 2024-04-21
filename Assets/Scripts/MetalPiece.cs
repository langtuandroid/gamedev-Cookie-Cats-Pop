using System;

public class MetalPiece : CPPiece
{
	public override void Hit(IHitResolver resolver)
	{
	}

	public override bool CanMoveBySpring
	{
		get
		{
			return false;
		}
	}
}
