using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class TicketPanel : MonoBehaviour, IRewardTarget
{
	public void Init(TournamentRank rank)
	{
		this.rank = rank;
		this.UpdateAmount();
	}

	public void UpdateAmount()
	{
		if (this.silver.amount != null && this.gold.amount != null)
		{
			this.bronze.amount.text = TournamentManager.Instance.GetTicketsForRank(TournamentRank.Bronze).ToString();
			this.silver.amount.text = TournamentManager.Instance.GetTicketsForRank(TournamentRank.Silver).ToString();
			this.gold.amount.text = TournamentManager.Instance.GetTicketsForRank(TournamentRank.Gold).ToString();
		}
		else
		{
			this.bronze.amount.text = TournamentManager.Instance.GetTicketsForRank(this.rank).ToString();
		}
	}

	private TicketPanel.TicketGroup GetGroup(TournamentRank rank)
	{
		switch (rank)
		{
		case TournamentRank.Bronze:
			return this.bronze;
		case TournamentRank.Silver:
			return this.silver;
		case TournamentRank.Gold:
			return this.gold;
		default:
			return null;
		}
	}

	public void SetAmountForType(TournamentRank rank, int amount)
	{
		TicketPanel.TicketGroup group = this.GetGroup(rank);
		group.amount.text = amount.ToString();
		this.forcedAmounts = true;
	}

	public void IncreaseAmountForType(TournamentRank rank)
	{
		TicketPanel.TicketGroup group = this.GetGroup(rank);
		group.amount.text = (int.Parse(group.amount.text) + 1).ToString();
		this.forcedAmounts = true;
	}

	public void UpdateAllAmounts(Dictionary<object, int> nonCelebratedRewards)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (nonCelebratedRewards.ContainsKey(TournamentRank.Bronze))
		{
			num = nonCelebratedRewards[TournamentRank.Bronze];
		}
		if (nonCelebratedRewards.ContainsKey(TournamentRank.Silver))
		{
			num2 = nonCelebratedRewards[TournamentRank.Silver];
		}
		if (nonCelebratedRewards.ContainsKey(TournamentRank.Gold))
		{
			num3 = nonCelebratedRewards[TournamentRank.Gold];
		}
		this.bronze.amount.text = (TournamentManager.Instance.GetTicketsForRank(TournamentRank.Bronze) - num).ToString();
		this.silver.amount.text = (TournamentManager.Instance.GetTicketsForRank(TournamentRank.Silver) - num2).ToString();
		this.gold.amount.text = (TournamentManager.Instance.GetTicketsForRank(TournamentRank.Gold) - num3).ToString();
		this.forcedAmounts = true;
	}

	private void Update()
	{
		if (!this.forcedAmounts)
		{
			this.UpdateAmount();
		}
	}

	public Vector3 GetTicketPositionForRank(TournamentRank rank)
	{
		if (!(this.silver.icon != null) || !(this.gold.icon != null))
		{
			return this.bronze.icon.transform.position;
		}
		switch (rank)
		{
		case TournamentRank.Bronze:
			return this.bronze.icon.transform.position;
		case TournamentRank.Silver:
			return this.silver.icon.transform.position;
		case TournamentRank.Gold:
			return this.gold.icon.transform.position;
		default:
			return Vector3.zero;
		}
	}

	public IEnumerator AnimateTicket(TournamentRank rank, TicketPanel.AnimationDirection direction, Vector3 otherPos)
	{
		Vector3 sourcePos;
		Vector3 destPos;
		if (direction == TicketPanel.AnimationDirection.FromPanel)
		{
			sourcePos = this.GetTicketPositionForRank(rank);
			destPos = otherPos;
		}
		else
		{
			destPos = this.GetTicketPositionForRank(rank);
			sourcePos = otherPos;
		}
		sourcePos.z -= 35f;
		destPos.z -= 35f;
		Vector3 scaleSource = new Vector3(1f, 1f, 1f);
		Vector3 scaleDest = new Vector3(1f, 1f, 1f);
		if (this.flyingTicketPrefab != null)
		{
			GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.flyingTicketPrefab);
			go.gameObject.SetLayerRecursively(base.gameObject.layer);
			TournamentSetup.RankSetup setup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(rank);
			InventoryItemMetaData meta = InventoryManager.Instance.GetMetaData(setup.ticketItem);
			go.GetComponentInChildren<UISprite>().SpriteName = meta.IconSpriteName;
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.MoveTransform(go.transform, sourcePos, destPos, this.ticketAnimationCurve, 0.5f),
				FiberAnimation.ScaleTransform(go.transform, scaleSource, scaleDest, this.ticketAnimationCurve, 0.5f)
			});
			UnityEngine.Object.Destroy(go);
		}
		yield break;
	}

	Transform IRewardTarget.TryGetTransformTarget(InventoryItem item)
	{
		string text = item;
		if (text != null)
		{
			if (text == "TicketBronze")
			{
				return this.GetGroup(TournamentRank.Bronze).icon.transform;
			}
			if (text == "TicketSilver")
			{
				return this.GetGroup(TournamentRank.Silver).icon.transform;
			}
			if (text == "TicketGold")
			{
				return this.GetGroup(TournamentRank.Gold).icon.transform;
			}
		}
		return null;
	}

	UILabel IRewardTarget.TryGetAmountLabel(InventoryItem item)
	{
		string text = item;
		if (text != null)
		{
			if (text == "TicketBronze")
			{
				return this.GetGroup(TournamentRank.Bronze).amount;
			}
			if (text == "TicketSilver")
			{
				return this.GetGroup(TournamentRank.Silver).amount;
			}
			if (text == "TicketGold")
			{
				return this.GetGroup(TournamentRank.Gold).amount;
			}
		}
		return null;
	}

	void IRewardTarget.DisableInventoryListeners()
	{
		this.forcedAmounts = true;
	}

	void IRewardTarget.ReEnableInventoryListeners()
	{
		this.forcedAmounts = false;
	}

	void IRewardTarget.Initialize()
	{
		this.Init(TournamentRank.Bronze);
	}

	public AnimationCurve ticketAnimationCurve;

	public TicketPanel.TicketGroup bronze;

	public TicketPanel.TicketGroup silver;

	public TicketPanel.TicketGroup gold;

	public GameObject flyingTicketPrefab;

	private TournamentRank rank;

	private bool forcedAmounts;

	[Serializable]
	public class TicketGroup
	{
		public UILabel amount;

		public UISprite icon;
	}

	public enum AnimationDirection
	{
		IntoPanel,
		FromPanel
	}
}
