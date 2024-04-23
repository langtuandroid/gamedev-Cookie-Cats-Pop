using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGame.ThemeHunt;
using TactileModules.PuzzleGame.ThemeHunt.Views;

public class ThemeHuntManager : ThemeHuntManagerBase
{
	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private MapStreamerCollection MapStreamerCollection
	{
		get
		{
			return ManagerRepository.Get<MapStreamerCollection>();
		}
	}

	protected override ThemeHuntManagerBase.PersistableState State
	{
		get
		{
			return UserSettingsManager.Get<ThemeHuntManagerBase.PersistableState>();
		}
	}

	protected override int GetFarthestUnlockedLevelHumanNumber()
	{
		return MainProgressionManager.Instance.GetFarthestUnlockedLevelHumanNumber();
	}

	protected override ThemeHuntConfig GetThemeHuntConfig()
	{
		return this.ConfigurationManager.GetConfig<ThemeHuntConfig>();
	}

	protected override void RegisterUserSettingsSync(Action handler)
	{
		UserSettingsManager.Instance.SettingsSynced += delegate(UserSettingsManager manager)
		{
			handler();
		};
	}

	protected override void RewardsClaimed(List<ThemeHuntRewardItem> themeHuntRewardItems)
	{
		List<ItemAmount> list = new List<ItemAmount>();
		foreach (ThemeHuntRewardItem themeHuntRewardItem in themeHuntRewardItems)
		{
			foreach (ItemAmount itemAmount in themeHuntRewardItem.Rewards)
			{
				InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "TreasureHunt");
				list.Add(itemAmount);
			}
		}
		this.fiber.Start(this.ShowThemeHuntReward(list));
	}

	protected override void LogThemeHuntItemCollected(string themeId, int collectedItems, int itemId)
	{
		Analytics.Instance.LogThemeHuntItemCollected(themeId, collectedItems, itemId);
	}

	protected override MapViewSetup GetMapViewSetup()
	{
		return this.MapStreamerCollection.GetMapViewSetupFromMapIdentifier("Main");
	}

	protected override void SaveLocalPrivateUserSettings()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	private IEnumerator ShowThemeHuntReward(List<ItemAmount> visualItems)
	{
		UIViewManager.UIViewStateGeneric<ThemeHuntRewardView> vs = UIViewManager.Instance.ShowView<ThemeHuntRewardView>(new object[]
		{
			visualItems
		});
		yield return vs.WaitForClose();
		if (!base.IsHuntActiveOnClient())
		{
			UIViewManager.UIViewStateGeneric<ThemeHuntEndView> vs2 = UIViewManager.Instance.ShowView<ThemeHuntEndView>(new object[0]);
			yield return vs2.WaitForClose();
		}
		yield break;
	}

	private Fiber fiber = new Fiber();
}
