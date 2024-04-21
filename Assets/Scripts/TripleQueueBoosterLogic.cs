using System;
using System.Collections;

public class TripleQueueBoosterLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		session.BallQueue.ActivateTripleQueue();
		yield break;
	}
}
