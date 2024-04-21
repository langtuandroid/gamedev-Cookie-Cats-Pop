using System;
using System.Collections;

public class SuperAimBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		session.Cannon.ActivateSuperAim();
		yield break;
	}
}
