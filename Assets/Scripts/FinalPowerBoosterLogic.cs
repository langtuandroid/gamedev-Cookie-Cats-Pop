using System;
using System.Collections;

public class FinalPowerBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		PowerCombination powerCombination = new PowerCombination(new PowerColor[]
		{
			PowerColor.Blue,
			PowerColor.Green,
			PowerColor.Red,
			PowerColor.Yellow
		});
		session.Powers.SetOverrideCombination(powerCombination);
		session.LoadSpecialShot(PieceId.Create<ComboPowerPiece>(string.Empty), true);
		yield break;
	}
}
