using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using Tactile;
using TactileModules.Validation;
using UnityEngine;

public class DailyQuestNode : MonoBehaviour
{
	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<DailyQuestNode> rewardClicked = delegate (DailyQuestNode A_0)
    {
    };



    public DailyQuestRewardBubble RewardBubble
	{
		get
		{
			return this.rewardBubble;
		}
	}

	public DailyQuestNode.NodeType nodeType { get; private set; }

	public int DayIndex
	{
		get
		{
			return this.dayIndex;
		}
	}

	public string Title
	{
		get
		{
			return this.dayItem.Title;
		}
	}

	public void Initialize(int dayIndex, ref int chestCounter)
	{
		this.dayIndex = dayIndex;
		this.dayItem = DailyQuestManager.Instance.GetQuestInfo(dayIndex);
		this.rewardBubble.Initialize(this.dayItem);
		this.boosterPivot.Initialize(this.dayItem);
		this.chestNumber = chestCounter;
		int num;
		this.nodeType = this.DetermineNodeTypeFromRewards(this.dayItem, out num);
		if (this.nodeType == DailyQuestNode.NodeType.Boosters)
		{
			chestCounter++;
		}
	}

	public Vector3 GetAvatarLocation()
	{
		Vector3 b = new Vector3(-35f, -40f, 0f);
		return base.transform.position + b;
	}

	public Vector3 GetCoinLocation()
	{
		return this.rewardBubble.transform.position;
	}

	private DailyQuestNode.NodeType DetermineNodeTypeFromRewards(DailyQuestInfo info, out int numBoosters)
	{
		int num = 0;
		numBoosters = 0;
		foreach (ItemAmount itemAmount in info.Rewards)
		{
			if (InventoryManager.Instance.GetMetaData<TicketMetaData>(itemAmount.ItemId) != null)
			{
				num++;
			}
			if (InventoryManager.Instance.GetMetaData<BoosterMetaData>(itemAmount.ItemId) != null)
			{
				numBoosters++;
			}
		}
		if (num > 0)
		{
			return DailyQuestNode.NodeType.Tickets;
		}
		if (numBoosters > 0)
		{
			return DailyQuestNode.NodeType.Boosters;
		}
		return DailyQuestNode.NodeType.Coins;
	}

	private TournamentRank GetRewardTicketType()
	{
		return TournamentRank.None;
	}

	private int RewardBoosterIndex()
	{
		int num = 0;
		for (int i = 0; i < this.dayIndex; i++)
		{
			foreach (ItemAmount itemAmount in this.dayItem.Rewards)
			{
				if (InventoryManager.Instance.GetMetaData<BoosterMetaData>(itemAmount.ItemId) != null)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void UpdateUI(DailyQuestManager.QuestState state)
	{
		if (this.dayIndex >= ConfigurationManager.Get<DailyQuestConfig>().Quests.Count)
		{
			base.gameObject.SetActive(false);
			return;
		}
		DailyQuestNode.NodeType nodeType = this.nodeType;
		if (nodeType != DailyQuestNode.NodeType.Tickets)
		{
			if (nodeType != DailyQuestNode.NodeType.Boosters)
			{
				this.currentPivot = this.coinPivot;
			}
			else
			{
				this.currentPivot = this.boosterPivot;
				this.currentPivot.SetChestSkinFromNumberOfBoosters(this.chestNumber);
			}
		}
		else
		{
			this.currentPivot = this.ticketPivot;
			this.currentPivot.SetTicketSpriteFromDayItem(this.dayItem);
		}
		this.boosterPivot.pivot.SetActive(this.currentPivot == this.boosterPivot);
		this.ticketPivot.pivot.SetActive(this.currentPivot == this.ticketPivot);
		this.coinPivot.pivot.SetActive(this.currentPivot == this.coinPivot);
		this.rewardBubble.rewardGrid.ShowRewards();
		this.isCompleted = (state == DailyQuestManager.QuestState.Completed);
		this.currentPivot.SetCompleted(this.isCompleted);
		bool available = state != DailyQuestManager.QuestState.Missed;
		this.currentPivot.SetAvailable(available);
	}

	private void OnDestroy()
	{
		this.crossFiber.Terminate();
	}

	public bool ShowingReward
	{
		get
		{
			return this.rewardBubble.Visible;
		}
		set
		{
			this.rewardBubble.Visible = value;
		}
	}

	private void OnClick()
	{
		if (this.isCompleted)
		{
			return;
		}
		this.rewardClicked(this);
	}

	public IEnumerator AnimateChestIntoFocus(Transform targetTransform, float duration)
	{
		this.ShowingReward = false;
		this.orgPosition = this.currentPivot.pivot.transform.position;
		this.orgScale = this.currentPivot.pivot.transform.localScale;
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.MoveTransform(this.currentPivot.pivot.transform, this.orgPosition, targetTransform.position, SingletonAsset<CommonCurves>.Instance.easeInOut, duration),
			FiberAnimation.ScaleTransform(this.currentPivot.pivot.transform, this.orgScale, targetTransform.localScale, SingletonAsset<CommonCurves>.Instance.easeInOut, duration)
		});
		yield break;
	}

	public void OpenBox()
	{
		this.currentPivot.SetCompleted(true);
		this.currentPivot.ShowRewards(true);
	}

	public IEnumerator AnimateChestBackInPlace(float fadeOutDuration)
	{
		this.ShowingReward = false;
		this.currentPivot.ShowRewards(false);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.MoveTransform(this.currentPivot.pivot.transform, this.currentPivot.pivot.transform.position, this.orgPosition, SingletonAsset<CommonCurves>.Instance.easeInOut, fadeOutDuration),
			FiberAnimation.ScaleTransform(this.currentPivot.pivot.transform, this.currentPivot.pivot.transform.localScale, this.orgScale, SingletonAsset<CommonCurves>.Instance.easeInOut, fadeOutDuration)
		});
		yield break;
	}

	public void CrossOut()
	{
		this.crossFiber.Start(this.AnimCrossOut());
	}

	private IEnumerator AnimCrossOut()
	{
		this.currentPivot.crossPivot.SetActive(true);
		yield return FiberAnimation.ScaleTransform(this.currentPivot.crossPivot.transform, Vector3.one * 0.7f, Vector3.one, this.curveDailyQuestCrossAppear, 0f);
		this.currentPivot.SetAvailable(false);
		yield break;
	}

	public IEnumerator AnimateRewards()
	{
		if (this.currentPivot.rewardGrid == null)
		{
			yield return this.rewardBubble.AnimateRewards();
		}
		else
		{
			yield return this.currentPivot.rewardGrid.Animate(true, true);
		}
		yield break;
	}

	[SerializeField]
	private DailyQuestNode.ActivePivot coinPivot;

	[SerializeField]
	private DailyQuestNode.ActivePivot boosterPivot;

	[SerializeField]
	private DailyQuestNode.ActivePivot ticketPivot;

	[SerializeField]
	private AnimationCurve curveDailyQuestCrossAppear;

	[SerializeField]
	[OptionalSerializedField]
	private DailyQuestRewardBubble rewardBubble;

	private DailyQuestNode.ActivePivot currentPivot;

	private Fiber crossFiber = new Fiber();

	private int dayIndex;

	private DailyQuestInfo dayItem;

	private bool isCompleted;

	private Vector3 orgPosition;

	private Vector3 orgScale;

	private int chestNumber;

	public enum NodeType
	{
		Coins,
		Boosters,
		Tickets
	}

	[Serializable]
	public class ActivePivot
	{
		public void Initialize(DailyQuestInfo info)
		{
			if (this.rewardGrid != null)
			{
				this.rewardGrid.Initialize(info.Rewards, true);
				this.ShowRewards(false);
			}
		}

		public void ShowRewards(bool show)
		{
			if (this.rewardGrid != null)
			{
				this.rewardGrid.gameObject.SetActive(show);
			}
		}

		public void SetCompleted(bool completed)
		{
			if (this.completed != null)
			{
				this.completed.SetActive(completed);
			}
			if (this.locked != null)
			{
				this.locked.SetActive(!completed);
			}
			if (this.chest != null)
			{
				this.chest.PlayAnimation(0, (!completed) ? "closed cycle" : "open empty", true, true);
			}
		}

		public void SetAvailable(bool isAvailable)
		{
			this.crossPivot.SetActive(!isAvailable);
			if (this.lockedSprite != null)
			{
				if (!isAvailable)
				{
					this.lockedSprite.Color = this.greyOutUnavailable;
				}
				else
				{
					this.lockedSprite.Color = Color.white;
				}
			}
		}

		public void SetChestSkinFromNumberOfBoosters(int numBoosters)
		{
			if (this.chest != null)
			{
				int num = Mathf.Clamp(numBoosters - 1, 0, DailyQuestNode.ActivePivot.chestSkins.Length - 1);
				this.chest.skeleton.SetSkin(DailyQuestNode.ActivePivot.chestSkins[num]);
			}
		}

		public void SetTicketSpriteFromDayItem(DailyQuestInfo dayItem)
		{
			if (dayItem.Rewards.GetAmount("TicketGold") > 0)
			{
				UISprite uisprite = this.completedSprite;
				string iconSpriteName = InventoryManager.Instance.GetMetaData<TicketMetaData>("TicketGold").IconSpriteName;
				this.lockedSprite.SpriteName = iconSpriteName;
				uisprite.SpriteName = iconSpriteName;
			}
			else if (dayItem.Rewards.GetAmount("TicketSilver") > 0)
			{
				UISprite uisprite2 = this.completedSprite;
				string iconSpriteName = InventoryManager.Instance.GetMetaData<TicketMetaData>("TicketSilver").IconSpriteName;
				this.lockedSprite.SpriteName = iconSpriteName;
				uisprite2.SpriteName = iconSpriteName;
			}
			else
			{
				UISprite uisprite3 = this.completedSprite;
				string iconSpriteName = InventoryManager.Instance.GetMetaData<TicketMetaData>("TicketBronze").IconSpriteName;
				this.lockedSprite.SpriteName = iconSpriteName;
				uisprite3.SpriteName = iconSpriteName;
			}
		}

		public GameObject pivot;

		public GameObject completed;

		public GameObject locked;

		public GameObject crossPivot;

		public UISprite lockedSprite;

		public UISprite completedSprite;

		public Color greyOutUnavailable;

		public Color greenCompleted;

		public SkeletonAnimation chest;

		public RewardGrid rewardGrid;

		private static string[] chestSkins = new string[]
		{
			"chest_1",
			"chest_2",
			"chest_3",
			"chest_4",
			"chest_05"
		};
	}
}
