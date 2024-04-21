using System;
using System.Collections;

public class RainbowBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		new RainbowEffectOnPieces(session);
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		session.LoadSpecialShot(PieceId.Create<RainbowPiece>(string.Empty), true);
		yield break;
	}
}
