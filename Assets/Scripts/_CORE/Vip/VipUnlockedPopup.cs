using System;
using System.Collections;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;

public class VipUnlockedPopup : MapPopupManager.IMapPopup
{
	public VipUnlockedPopup()
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

	private VipProgramConfig Config
	{
		get
		{
			return ConfigurationManager.Get<VipProgramConfig>();
		}
	}

	private bool ShouldShowPopup(int levelUnlocked)
	{
		LevelProxy levelProxy = new LevelProxy(this.MainLevels, new int[]
		{
			levelUnlocked
		});
		return levelProxy.HumanNumber == this.Config.LevelRequiredForVip;
	}

	private IEnumerator ShowPopup()
	{
		UIViewManager.UIViewStateGeneric<VipProgramUnlockedView> vs = UIViewManager.Instance.ShowView<VipProgramUnlockedView>(new object[0]);
		yield return vs.WaitForClose();
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
