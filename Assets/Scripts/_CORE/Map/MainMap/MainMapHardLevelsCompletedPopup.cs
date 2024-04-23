using System;
using System.Collections;
using TactileModules.PuzzleGame.MainLevels;

public class MainMapHardLevelsCompletedPopup : MapPopupManager.IMapPopup
{
	public MainMapHardLevelsCompletedPopup(IHardLevelsProvider hardLevelsProvider, MainProgressionManager mainProgressionManager, HardLevelsManager hardLevelsManager)
	{
		this.hardLevelsProvider = hardLevelsProvider;
		this.mainProgressionManager = mainProgressionManager;
		this.hardLevelsManager = hardLevelsManager;
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	private bool ShouldShowPopup(LevelProxy levelProxy)
	{
		return this.hardLevelsManager.ShouldShowCompletedPopup(levelProxy);
	}

	private IEnumerator ShowPopup(LevelProxy levelProxy)
	{
		yield return UIViewManager.Instance.ShowView<HardLevelsView>(new object[]
		{
			HardLevelsView.EventState.Completed
		}).WaitForClose();
		this.hardLevelsManager.DeactivateHardLevels(levelProxy, HardLevelsView.EventState.Completed);
		yield break;
	}

	public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		LevelProxy farthestCompletedLevelProxy = this.mainProgressionManager.GetFarthestCompletedLevelProxy();
		if (this.ShouldShowPopup(farthestCompletedLevelProxy))
		{
			popupFlow.AddPopup(this.ShowPopup(farthestCompletedLevelProxy));
		}
	}

	private readonly IHardLevelsProvider hardLevelsProvider;

	private readonly MainProgressionManager mainProgressionManager;

	private readonly HardLevelsManager hardLevelsManager;
}
