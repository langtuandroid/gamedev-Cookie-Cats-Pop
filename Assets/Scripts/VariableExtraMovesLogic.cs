using System;
using System.Collections;

public class VariableExtraMovesLogic : BoosterLogic
{
	protected override IEnumerator Logic(LevelSession session)
	{
		this.movesAmount.text = this.variableExtraMoves.ToString();
		yield return FiberHelper.Wait(1.5f, (FiberHelper.WaitFlag)0);
		session.BallQueue.ModifyBallsLeft(this.variableExtraMoves, false);
		if (session.SessionState != LevelSessionState.Playing)
		{
			session.SetState(LevelSessionState.Playing);
		}
		yield break;
	}

	public int variableExtraMoves = 1;

	public UILabel movesAmount;
}
