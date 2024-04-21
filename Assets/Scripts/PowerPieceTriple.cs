using System;

public class PowerPieceTriple : PowerPieceBase
{
	protected override void TriggerPower(Tile origin, IHitResolver resolver)
	{
		PowerCombinationLogic.DoPower(new PowerCombination(new PowerColor[1]), origin, resolver);
	}
}
