using System;
using System.Collections;
using TactileModules.Foundation;
using TactileModules.PuzzleCore.LevelPlaying;

public class SynchronizerAtLevelEnd
{
	public SynchronizerAtLevelEnd(IPlayFlowEvents playFlowEvents, CloudSynchronizer cloudSynchronizer)
	{
		this.cloudSynchronizer = cloudSynchronizer;
		playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
	}

	private void HandlePlayFlowCreated(ICorePlayFlow obj)
	{
		obj.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
	}

	private IEnumerator HandleLevelEnded(ILevelAttempt levelAttempt)
	{
		if (levelAttempt.Completed)
		{
			this.cloudSynchronizer.SyncCloud();
		}
		yield break;
	}

	private readonly CloudSynchronizer cloudSynchronizer;
}
