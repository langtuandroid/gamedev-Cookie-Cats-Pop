using System;
using System.Collections;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;

public class VipReminderPopup : MapPopupManager.IMapPopup
{
	public VipReminderPopup()
	{
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	private MainLevelDatabase MainLevels
	{
		get
		{
			return this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		}
	}

	private bool ShouldShowPopup(int levelUnlocked)
	{
		LevelProxy levelProxy = new LevelProxy(this.MainLevels, new int[]
		{
			levelUnlocked
		});
		return levelProxy.HumanNumber == 70 || (levelProxy.HumanNumber >= 70 && (levelProxy.HumanNumber - 70) % 50 == 0);
	}

	private IEnumerator ShowPopup()
	{
		yield return UIViewManager.Instance.ShowView<VipProgramNotSubscriberView>(new object[0]).WaitForClose();
		yield break;
	}

	public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		if (this.ShouldShowPopup(unlockedLevelIndex))
		{
			popupFlow.AddPopup(this.ShowPopup());
		}
	}
}
