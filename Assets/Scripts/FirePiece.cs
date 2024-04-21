using System;

public class FirePiece : PowerPieceBase
{
	protected override void TriggerPower(Tile origin, IHitResolver resolver)
	{
		PowerCombinationLogic.DoPower(new PowerCombination(new PowerColor[]
		{
			PowerColor.Red
		}), origin, resolver);
	}

	private const float t1 = 0.3f;

	private const float t2 = 0.6f;

	private const float t3 = 0.1f;

	private const float t4 = 0.5f;

	private const float t5 = 0.2f;
}
