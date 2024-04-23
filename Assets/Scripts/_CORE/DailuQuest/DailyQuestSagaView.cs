using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Foundation;
using TactileModules.SagaCore;
using UnityEngine;

public class DailyQuestSagaView : ExtensibleView<DailyQuestSagaView.IExtension>, SagaAvatarController.IDotPositions, SagaAvatarController.IAvatarInfoProvider
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action PlayClicked = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action ClickedClose = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action ClickedClaim = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action QuestSkipped = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action CooldownSkipped = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action BeforeAnimatingFriends = delegate ()
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action AfterAnimatingFrends = delegate ()
    {
    };



    public void Initialize(List<DailyQuestInfo> infos, Func<string> timeLabelUpdateCallback, MapFacade mapFacade, DailyQuestManager dailyQuestManager, DailyQuestFactory dailyQuestFactory)
	{
		this.timeLabelUpdateCallback = timeLabelUpdateCallback;
		this.dailyQuestManager = dailyQuestManager;
		this.dailyQuestFactory = dailyQuestFactory;
		Instantiator[] componentsInChildren = this.path.gameObject.GetComponentsInChildren<Instantiator>();
		this.nodes = new DailyQuestNode[componentsInChildren.Length];
		int num = 0;
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.nodes[i] = componentsInChildren[i].GetInstance<DailyQuestNode>();
			if (i < infos.Count)
			{
				this.nodes[i].rewardClicked += this.NodeRewardClicked;
				this.nodes[i].Initialize(i, ref num);
			}
			else
			{
				componentsInChildren[i].gameObject.SetActive(false);
			}
		}
		this.path.UpdateMesh();
		this.avatarController = new SagaAvatarController(ManagerRepository.Get<CloudClient>(), ManagerRepository.Get<VipManager>(), ManagerRepository.Get<FacebookClient>(), mapFacade);
		this.avatarController.AttachToDotProvider(this, this, -20f, Rect.zero);
		this.avatarController.Refresh();
	}

	SagaAvatarInfo SagaAvatarController.IAvatarInfoProvider.CreateMeAvatarInfo()
	{
		return new SagaAvatarInfo
		{
			dotIndex = this.dailyQuestManager.CurrentQuestLevel.Index,
			visualPrefab = this.dailyQuestFactory.GetMeAvatarPrefab()
		};
	}

	Dictionary<CloudUser, SagaAvatarInfo> SagaAvatarController.IAvatarInfoProvider.CreateFriendsAvatarInfos()
	{
		Dictionary<CloudUser, SagaAvatarInfo> dictionary = new Dictionary<CloudUser, SagaAvatarInfo>();
		Dictionary<string, int> lastKnownFriendsProgress = this.dailyQuestManager.GetLastKnownFriendsProgress();
		foreach (KeyValuePair<string, int> keyValuePair in lastKnownFriendsProgress)
		{
			SagaAvatarInfo value = new SagaAvatarInfo
			{
				dotIndex = keyValuePair.Value,
				visualPrefab = this.dailyQuestFactory.GetFriendAvatarPrefab()
			};
			CloudUser userForFacebookId = ManagerRepository.Get<CloudClient>().GetUserForFacebookId(keyValuePair.Key);
			dictionary.Add(userForFacebookId, value);
		}
		return dictionary;
	}

	int SagaAvatarController.IAvatarInfoProvider.GetProgressMarkerDotIndex()
	{
		return -1;
	}

	public string CountDownText
	{
		get
		{
			return this.timeLeft.text;
		}
		set
		{
			this.timeLeft.text = value;
		}
	}

	private void NodeRewardClicked(DailyQuestNode node)
	{
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.nodes[i].ShowingReward = (node != null && i == node.DayIndex && !this.nodes[i].ShowingReward);
		}
	}

	private IEnumerator UpdateCooldownTimer()
	{
		while (DailyQuestManager.Instance.HasQuestCooldown)
		{
			this.timeLeft.text = this.timeLabelUpdateCallback();
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	protected override void ViewWillDisappear()
	{
		this.startupFiber.Terminate();
		this.cooldownFiber.Terminate();
	}

	public void UpdateFromState(DailyQuestChallengeState state)
	{
		if (base.Extension != null)
		{
			base.Extension.UpdateCloseButtonVisibility(!state.HasPendingReward);
		}
		this.playPivot.SetActive(!state.HasPendingReward && !state.HasQuestCooldown);
		this.claimPivot.SetActive(state.HasPendingReward);
		this.waitPivot.SetActive(state.HasQuestCooldown);
		if (state.HasQuestCooldown)
		{
			if (this.timeLabelUpdateCallback != null)
			{
				this.cooldownFiber.Start(this.UpdateCooldownTimer());
			}
		}
		else
		{
			this.cooldownFiber.Terminate();
		}
		if (this.playPivot.activeSelf)
		{
			this.skipPlayingPriceLabel.GetInstance().GetComponent<ButtonPurchase>().SetByShopItemIdentifier("ShopItemSkipPlayDailyQuest", delegate
			{
				this.QuestSkipped();
			});
		}
		if (this.waitPivot.activeSelf)
		{
			this.skipWaitingPriceLabel.GetInstance().GetComponent<ButtonPurchase>().SetByShopItemIdentifier("ShopItemSkipWaitDailyQuest", delegate
			{
				this.CooldownSkipped();
			});
		}
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.nodes[i].UpdateUI(state.QuestStates[i]);
		}
		this.claimRewardBackFill.Alpha = 0f;
	}

	[UsedImplicitly]
	private void PlayButtonClicked(UIEvent e)
	{
		this.PlayClicked();
	}

	[UsedImplicitly]
	private void CloseButtonClicked(UIEvent e)
	{
		this.ClickedClose();
	}

	[UsedImplicitly]
	private void ClaimButtonClicked(UIEvent e)
	{
		this.ClickedClaim();
	}

	public IEnumerator AnimateConsumingReward(int questIndex, DailyQuestInfo info)
	{
		UICamera.DisableInput();
		DailyQuestNode node = this.nodes[questIndex];
		this.claimPivot.gameObject.SetActive(false);
		yield return this.AnimateReward(node, info.Rewards, questIndex);
		yield return this.avatarController.MoveAvatar("me", questIndex, questIndex + 1, new SagaAvatarController.AvatarMoveAnimation(this.CustomAvatarMovement));
		if (base.Extension != null)
		{
			base.Extension.UpdateCloseButtonVisibility(true);
		}
		UICamera.EnableInput();
		yield break;
	}

	private IEnumerator CustomAvatarMovement(MapAvatar avatar, Vector3 fromPos, Vector3 toPos)
	{
		yield return FiberAnimation.MoveLocalTransform(avatar.transform, fromPos, toPos, null, 0.2f);
		yield break;
	}

	private IEnumerator AnimateReward(DailyQuestNode node, List<ItemAmount> reward, int dayIndex)
	{
		this.NodeRewardClicked(null);
		if (node.nodeType == DailyQuestNode.NodeType.Boosters)
		{
			yield return this.AnimateChestReward(node, reward, dayIndex);
		}
		else
		{
			yield return this.AnimateListedReward(node, reward, dayIndex);
		}
		yield break;
	}

	private IEnumerator AnimateListedReward(DailyQuestNode node, List<ItemAmount> reward, int dayIndex)
	{
		bool flag = true;
		node.ShowingReward = flag;
		yield return flag;
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		foreach (ItemAmount itemAmount in reward)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, node.Title);
		}
		yield return node.AnimateRewards();
		flag = false;
		node.ShowingReward = flag;
		yield return flag;
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	private IEnumerator AnimateChestReward(DailyQuestNode node, List<ItemAmount> reward, int dayIndex)
	{
		yield return node.AnimateChestIntoFocus(this.focusedChestPivot, 0.5f);
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		node.OpenBox();
		yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
		foreach (ItemAmount itemAmount in reward)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, node.Title);
		}
		yield return node.AnimateRewards();
		yield return node.AnimateChestBackInPlace(0.5f);
		yield break;
	}

	public IEnumerator AnimateCrossingOutMissedQuests(int missed, int furthestAvailableQuestIndex)
	{
		int animateEndIndex = furthestAvailableQuestIndex - missed;
		for (int i = furthestAvailableQuestIndex; i > animateEndIndex; i--)
		{
			DailyQuestNode node = this.nodes[i];
			node.CrossOut();
			yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	public IEnumerator AnimateFriendProgression(List<DailyQuestManager.FriendProgressionState> states)
	{
		if (states.Count == 0)
		{
			yield break;
		}
		UICamera.DisableInput();
		bool playState = this.playPivot.activeSelf;
		bool claimState = this.claimPivot.activeSelf;
		bool waitState = this.waitPivot.activeSelf;
		this.playPivot.SetActive(false);
		this.claimPivot.SetActive(false);
		this.waitPivot.SetActive(false);
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		this.BeforeAnimatingFriends();
		foreach (DailyQuestManager.FriendProgressionState state in states)
		{
			CloudUser cloudUser = ManagerRepository.Get<CloudClient>().GetUserForFacebookId(state.FacebookId);
			yield return this.avatarController.MoveAvatar(cloudUser.CloudId, state.OldQuestIndex, state.NewQuestIndex);
			if (base.Extension != null)
			{
				Vector3 dotWorldPos = this.nodes[state.NewQuestIndex].transform.parent.transform.position;
				yield return base.Extension.AnimateCoinsForFriend(dotWorldPos, 1);
			}
			InventoryManager.Instance.AddCoins(1, "FriendMovedOnQuest");
			yield return FiberHelper.Wait(0.8f, (FiberHelper.WaitFlag)0);
		}
		this.playPivot.SetActive(playState);
		this.claimPivot.SetActive(claimState);
		this.waitPivot.SetActive(waitState);
		this.AfterAnimatingFrends();
		UICamera.EnableInput();
		yield break;
	}

	Transform SagaAvatarController.IDotPositions.ContentRoot
	{
		get
		{
			return this.path.transform;
		}
	}

	bool SagaAvatarController.IDotPositions.TryGetDotPosition(int dotIndex, out Vector3 position)
	{
		dotIndex = Mathf.Max(0, dotIndex);
		if (dotIndex >= this.nodes.Length)
		{
			dotIndex = 0;
		}
		position = this.nodes[dotIndex].transform.parent.transform.localPosition;
		return true;
	}

	[SerializeField]
	private DailyQuestSpline path;

	[SerializeField]
	private GameObject playPivot;

	[SerializeField]
	private GameObject claimPivot;

	[SerializeField]
	private GameObject waitPivot;

	[SerializeField]
	private UILabel timeLeft;

	[SerializeField]
	private UIInstantiator skipWaitingPriceLabel;

	[SerializeField]
	private UIInstantiator skipPlayingPriceLabel;

	[SerializeField]
	private UISprite claimRewardBackFill;

	[SerializeField]
	private Transform focusedChestPivot;

	private DailyQuestNode[] nodes;

	private string priorContext;

	private Fiber cooldownFiber = new Fiber();

	private readonly Fiber startupFiber = new Fiber();

	private CurrencyOverlay currencyOverlay;

	private SagaAvatarController avatarController;

	private Func<string> timeLabelUpdateCallback;

	private DailyQuestManager dailyQuestManager;

	private DailyQuestFactory dailyQuestFactory;

	public interface IExtension
	{
		void UpdateCloseButtonVisibility(bool isVisible);

		IEnumerator AnimateCoinsForFriend(Vector3 fromWorldPos, int amount);
	}
}
