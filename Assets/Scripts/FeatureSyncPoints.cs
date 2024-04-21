using System;
using System.Collections;
using System.Diagnostics;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.PuzzleGames.GameCore;

public class FeatureSyncPoints : IFeatureSyncPoints
{
	public FeatureSyncPoints(IFullScreenManager fullScreenManager)
	{
		fullScreenManager.MidChange.Register(new Func<ChangeInfo, IEnumerator>(this.HandleFullscreenMidChange));
		UIViewManager.Instance.OnFadedToBlack += this.InvokeFadeEvent;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action SafeForFeaturesToSync;

	private IEnumerator HandleFullscreenMidChange(ChangeInfo changeInfo)
	{
		this.InvokeFadeEvent();
		yield break;
	}

	private void InvokeFadeEvent()
	{
		if (this.SafeForFeaturesToSync != null)
		{
			this.SafeForFeaturesToSync();
		}
	}

	private readonly IFullScreenManager fullScreenManager;
}
