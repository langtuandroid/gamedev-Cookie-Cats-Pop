using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

public class GateVictory : IGateVictory, IGameInterface
{
	public IEnumerator ShowVictory(GateManager gateManager, ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> action)
	{
		UIViewManager.Instance.ShowView<GateVictoryBackgroundView>(new object[0]);
		if (gateManager.CurrentGateComplete)
		{
			UIViewManager instance = UIViewManager.Instance;
			object[] array = new object[4];
			array[0] = "Gate Unlocked!";
			array[1] = "You have unlocked the gate!";
			array[2] = "Continue";
			UIViewManager.UIViewStateGeneric<MessageBoxView> vs = instance.ShowView<MessageBoxView>(array);
			yield return vs.WaitForClose();
		}
		else
		{
			UIViewManager instance2 = UIViewManager.Instance;
			object[] array2 = new object[4];
			array2[0] = "Victory";
			array2[1] = "You earned a gate key!";
			array2[2] = "Continue";
			UIViewManager.UIViewStateGeneric<MessageBoxView> vs2 = instance2.ShowView<MessageBoxView>(array2);
			yield return vs2.WaitForClose();
		}
		yield break;
	}
}
