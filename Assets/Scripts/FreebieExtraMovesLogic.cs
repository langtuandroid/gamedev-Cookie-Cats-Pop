using System;
using System.Collections;

public class FreebieExtraMovesLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		session.BallQueue.ModifyBallsLeft(2, false);
		if (session.SessionState != LevelSessionState.Playing)
		{
			session.SetState(LevelSessionState.Playing);
		}
		yield break;
	}

	public const int NUMBER_OF_EXTRA_MOVES_ADDED = 2;
}
