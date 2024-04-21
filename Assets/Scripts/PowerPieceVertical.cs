using System;

public class PowerPieceVertical : PowerPieceBase
{
	protected override void TriggerPower(Tile origin, IHitResolver resolver)
	{
		PowerCombinationLogic.DoPower(new PowerCombination(new PowerColor[]
		{
			PowerColor.Blue
		}), origin, resolver);
	}
}
