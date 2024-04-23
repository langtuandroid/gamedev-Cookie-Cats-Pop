using System;
using System.Collections;

public class MainMapHardLevelsExpiredPopup : MapPopupManager.IMapPopup
{
	public MainMapHardLevelsExpiredPopup(IHardLevelsProvider hardLevelsProvider, HardLevelsManager hardLevelsManager)
	{
		this.hardLevelsProvider = hardLevelsProvider;
		this.hardLevelsManager = hardLevelsManager;
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	private IEnumerator ShowPopup(LevelProxy levelProxy)
	{
		yield return UIViewManager.Instance.ShowView<HardLevelsView>(new object[]
		{
			HardLevelsView.EventState.Expired
		}).WaitForClose();
		this.hardLevelsManager.DeactivateHardLevels(levelProxy, HardLevelsView.EventState.Expired);
		yield break;
	}

	private void EndSilently(LevelProxy levelProxy)
	{
		this.hardLevelsManager.DeactivateHardLevels(levelProxy, HardLevelsView.EventState.Expired);
	}

	public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		LevelProxy levelProxy = new LevelProxy(this.hardLevelsProvider.GetMainLevelDatabase(), new int[]
		{
			PuzzleGame.PlayerState.FarthestUnlockedLevelIndex
		});
		if (this.hardLevelsManager.ShouldShowExpiredPopup(levelProxy))
		{
			popupFlow.AddPopup(this.ShowPopup(levelProxy));
		}
		else if (this.hardLevelsManager.ShouldEndFeatureSilently(levelProxy))
		{
			Action a = delegate()
			{
				this.EndSilently(levelProxy);
			};
			popupFlow.AddSilentAction(a);
		}
	}

	private readonly IHardLevelsProvider hardLevelsProvider;

	private readonly HardLevelsManager hardLevelsManager;
}
