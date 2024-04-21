using System;
using System.Collections;
using System.Collections.Generic;
using Shared.OneLifeChallenge;
using Tactile;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;

public class OneLifeChallengeManagerProvider : IOneLifeChallengeProvider
{
	private UIViewManager ViewManager
	{
		get
		{
			return ManagerRepository.Get<UIViewManager>();
		}
	}

	public IEnumerator ClaimReward()
	{
		List<ItemAmount> items = this.Config().Rewards;
		foreach (ItemAmount itemAmount in items)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "OneLifeChallenge");
		}
		UIViewManager.UIViewStateGeneric<OneLifeChallengeRewardView> vs = UIViewManager.Instance.ShowView<OneLifeChallengeRewardView>(new object[]
		{
			items
		});
		yield return vs.WaitForClose();
		yield break;
	}

	public IEnumerator ShowEventStartView()
	{
		UIViewManager.UIViewStateGeneric<OneLifeChallengeStartInfoView> vs = UIViewManager.Instance.ShowView<OneLifeChallengeStartInfoView>(new object[]
		{
			false
		});
		yield return vs.WaitForClose();
		if ((int)vs.ClosingResult != 0)
		{
			OneLifeChallengeSystem oneLifeChallengeSystem = ManagerRepository.Get<OneLifeChallengeSystem>();
			OneLifeChallengeMapFlow c = oneLifeChallengeSystem.ControllerFactory.CreateMapFlow();
			FlowStack flowStack = ManagerRepository.Get<FlowStack>();
			flowStack.Push(c);
		}
		yield break;
	}

	public IEnumerator ShowEventStartSessionView()
	{
		UIViewManager.UIViewStateGeneric<OneLifeChallengeStartInfoView> vs = UIViewManager.Instance.ShowView<OneLifeChallengeStartInfoView>(new object[]
		{
			true
		});
		yield return vs.WaitForClose();
		yield break;
	}

	public IEnumerator ShowEventEndedView()
	{
		UIViewManager.UIViewStateGeneric<MessageBoxView> vs = this.ViewManager.ShowView<MessageBoxView>(new object[]
		{
			L.Get("Cookie Heist"),
			L.Get("The Cookie Heist is over! Better luck next time!"),
			L.Get("Ok")
		});
		yield return vs.WaitForClose();
		yield break;
	}

	public IEnumerator ShowLevelResultView()
	{
		UIViewManager.UIViewStateGeneric<OneLifeChallengeLevelResultView> vs = this.ViewManager.ShowView<OneLifeChallengeLevelResultView>(new object[0]);
		yield return vs.WaitForClose();
		yield break;
	}

	public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
	{
		return string.Format(L.Get("There are only {0} hours left of the Cookie Heist!"), timeSpan.TotalHours);
	}

	public OneLifeChallengeConfig Config()
	{
		return ConfigurationManager.Get<OneLifeChallengeConfig>();
	}
}
