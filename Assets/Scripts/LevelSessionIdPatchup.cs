using System;
using System.Collections;
using TactileModules.PuzzleCore.LevelPlaying;

public class LevelSessionIdPatchup
{
	public LevelSessionIdPatchup(IPlayFlowEvents events)
	{
		events.PlayFlowCreated += this.HandlePlayFlowCreated;
	}

	private void HandlePlayFlowCreated(ICorePlayFlow obj)
	{
		obj.LevelStartedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelSessionStarted));
	}

	private IEnumerator HandleLevelSessionStarted(ILevelAttempt obj)
	{
		PlayLevel playLevel = (PlayLevel)obj.GameImplementation;
		if (playLevel != null && playLevel.Session != null)
		{
			playLevel.Session.SessionId.id = obj.LevelSession.SessionId;
		}
		yield break;
	}
}
