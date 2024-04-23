using System;
using System.Collections;
using System.Collections.Generic;

using Tactile;
using Tactile.GardenGame.Story;
using TactileModules.Ads;
using TactileModules.Foundation;
using UnityEngine;

public class GameConsoleCommands : BaseCommandHandler
{

	private static void AdTestShowInterstitial()
	{
		FiberCtrl.Pool.Run(ManagerRepository.Get<InterstitialPresenter>().ShowInterstitial(), false);
	}


	private static void AdTestRequestInterstitial()
	{
		ManagerRepository.Get<InterstitialPresenter>().RequestInterstitial();
	}


	private static void AdTestFetchAndShowInterstitial()
	{
		FiberCtrl.Pool.Run(ManagerRepository.Get<InterstitialPresenter>().FetchAndShowInterstitial(10), false);
	}


	private static void AdTestShowRewardedVideo()
	{
		RewardedVideoParameters data = new RewardedVideoParameters("Debug", string.Empty, 0);
		FiberCtrl.Pool.Run(ManagerRepository.Get<RewardedVideoPresenter>().ShowRewardedVideo(data, delegate(bool videoCompleted)
		{
			UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
			{
				"Rewarded Video",
				"Video completed: " + videoCompleted,
				"OK"
			});
		}), false);
	}


	private static void AdTestFetchAndShowRewardedVideo()
	{
		RewardedVideoParameters data = new RewardedVideoParameters("Debug", string.Empty, 0);
		FiberCtrl.Pool.Run(ManagerRepository.Get<RewardedVideoPresenter>().FetchAndShowRewardedVideo(data, delegate(bool videoCompleted)
		{
			UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
			{
				"Video completed",
				"Completed: " + videoCompleted,
				"OK"
			});
		}, 10), false);
	}


	private static void AdTestShowMediationDebugger()
	{
		MaxSdkAndroid.ShowMediationDebugger();
	}


	private static void TestCrash()
	{
		GameObject gameObject = new GameObject("CrashObject");
		FiberCtrl.Pool.Run(FiberAnimation.MoveTransform(gameObject.transform, Vector3.zero, Vector3.one, null, 2f), false);
		UnityEngine.Object.Destroy(gameObject);
	}


	private static void Give(string itemType,  int amount)
	{
		InventoryManager.Instance.Add(itemType, amount, "Console");
		if (itemType == "Coin")
		{
			ManagerRepository.Get<StoryManager>().TotalPagesCollected += amount;
		}
	}


	private static void Deplete( string itemType)
	{
		int amount = InventoryManager.Instance.GetAmount(itemType);
		InventoryManager.Instance.Consume(itemType, amount, "Console");
	}


	private static void SyncUserSettings()
	{
		UserSettingsManager.Instance.SyncUserSettings();
	}

	private static void DeleteCloudUser()
	{
		FiberCtrl.Pool.Run(GameConsoleCommands.DeleteCloudUserCr(), false);
	}

	private static IEnumerator DeleteCloudUserCr()
	{
		CloudUser s = ManagerRepository.Get<CloudClient>().CachedMe;
		UIViewManager.UIViewStateGeneric<ProgressView> vs = UIViewManager.Instance.ShowView<ProgressView>(new object[]
		{
			"Deleting CloudUser",
			"Deleting " + s.DisplayName
		});
		object error = null;
		yield return ManagerRepository.Get<CloudClient>().DeleteCloudUser(delegate(object err)
		{
			error = err;
		});
		vs.View.Close(0);
		if (error != null)
		{
			UIViewManager instance = UIViewManager.Instance;
			object[] array = new object[4];
			array[0] = "Error";
			array[1] = error.ToString();
			array[2] = "Ok";
			instance.ShowView<MessageBoxView>(array);
		}
		yield break;
	}

	public static IEnumerable<string> Give_Amount_Autocomplete()
	{
		yield return "1";
		yield return "10";
		yield return "100";
		yield return "1000";
		yield break;
	}

	public static IEnumerable<string> Give_Item_Autocomplete()
	{
		List<string> names;
		List<string> values;
		CollectionExtensions.GetConstNamesAndValues<InventoryItem, string>(out names, out values);
		foreach (string v in values)
		{
			if (!string.IsNullOrEmpty(v))
			{
				yield return v;
			}
		}
		yield break;
	}
}
