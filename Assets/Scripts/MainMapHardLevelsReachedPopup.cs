using System;
using System.Collections;
using TactileModules.FeatureManager.DataClasses;

public class MainMapHardLevelsReachedPopup : MapPopupManager.IMapPopup
{
	public MainMapHardLevelsReachedPopup(IHardLevelsProvider hardLevelsProvider, HardLevelsManager hardLevelsManager)
	{
		this.hardLevelsProvider = hardLevelsProvider;
		this.hardLevelsManager = hardLevelsManager;
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	private bool ShouldShowPopup(LevelProxy levelProxy)
	{
		return this.hardLevelsManager.ShouldShowStartPopup(levelProxy);
	}

	private IEnumerator ShowPopup(LevelProxy levelProxy)
	{
		ActivatedFeatureInstanceData state = this.hardLevelsManager.GetActivationDataForLevelInRange(levelProxy, HardLevelsManager.RangeMatchFilter.Any);
		if (state == null)
		{
			yield break;
		}
		this.hardLevelsManager.MarkHardLevelsAsStarted(state);
		yield return UIViewManager.Instance.ShowView<HardLevelsView>(new object[]
		{
			HardLevelsView.EventState.Started,
			this.hardLevelsManager.GetExpirationDateForHardLevels(state)
		}).WaitForClose();
		yield break;
	}

	public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		LevelProxy levelProxy = new LevelProxy(this.hardLevelsProvider.GetMainLevelDatabase(), new int[]
		{
			PuzzleGame.PlayerState.FarthestUnlockedLevelIndex
		});
		if (this.ShouldShowPopup(levelProxy))
		{
			popupFlow.AddPopup(this.ShowPopup(levelProxy));
		}
	}

	private readonly IHardLevelsProvider hardLevelsProvider;

	private readonly HardLevelsManager hardLevelsManager;
}
