using System;
using System.Collections;

public class ShieldBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		session.ActivateShield();
		yield break;
	}
}
