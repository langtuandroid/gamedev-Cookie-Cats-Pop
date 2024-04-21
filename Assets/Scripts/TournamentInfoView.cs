using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentInfoView : UIView
{
	public void Initialize(TournamentRank rank)
	{
		this.rank = rank;
		this.PopulateRewardList();
		this.title.text = string.Format(L.Get("{0} Prizes"), SingletonAsset<TournamentSetup>.Instance.GetRankAsString(rank));
		this.avatarPivot.gameObject.SetActive(false);
		for (int i = 0; i < this.rankButtons.Count; i++)
		{
			this.rankButtons[i].GetInstance().GetComponent<ITournamentRankSelectButton>().Init(i + TournamentRank.Bronze);
			this.rankButtons[i].GetInstance<UIElement>().GetButton().Payload = i + TournamentRank.Bronze;
			if (i + TournamentRank.Bronze == rank)
			{
				this.HandleRankSelected(rank, this.rankButtons[i].GetInstance<UISprite>());
			}
		}
	}

	private void TournamentButtonClicked(UIEvent e)
	{
		this.HandleRankSelected((TournamentRank)e.payload, e.sender.GetElement().GetComponent<UISprite>());
	}

	private void HandleRankSelected(TournamentRank rank, UISprite sprite)
	{
		foreach (UIInstantiator uiinstantiator in this.rankButtons)
		{
			if (uiinstantiator.GetInstance<UISprite>() != sprite)
			{
				uiinstantiator.GetInstance().GetComponent<ITournamentRankSelectButton>().Selected = false;
			}
			else
			{
				uiinstantiator.GetInstance().GetComponent<ITournamentRankSelectButton>().Selected = true;
			}
		}
		TournamentSetup.RankSetup rankSetup = SingletonAsset<TournamentSetup>.Instance.GetRankSetup(rank);
		this.title.text = string.Format(L.Get("{0} Prizes"), L.Get(rankSetup.displayName));
		this.selectionSprite.parent = sprite.transform;
		this.selectionSprite.localScale = Vector3.one;
		this.selectionSprite.localPosition = Vector3.forward * 0.05f;
		UIFontStyle fontStyle = rankSetup.fontStyle;
		if (fontStyle)
		{
			this.title.fontStyle = fontStyle;
		}
		this.UpdateRewardList(rank);
	}

	private void PopulateRewardList()
	{
		for (int i = 0; i < TournamentManager.Instance.GetRewardsForTournamentRank(this.rank).Prizes.Count; i++)
		{
			TournamentRewardItem tournamentRewardItem = UnityEngine.Object.Instantiate<TournamentRewardItem>(this.rewardItemPrefab);
			tournamentRewardItem.transform.parent = this.grid.transform;
			tournamentRewardItem.transform.localScale = Vector3.one;
			tournamentRewardItem.gameObject.SetLayerRecursively(this.grid.gameObject.layer);
			tournamentRewardItem.gameObject.name = "RewardRow" + i;
		}
	}

	private void UpdateRewardList(TournamentRank rank)
	{
		TournamentRankConfig rewardsForTournamentRank = TournamentManager.Instance.GetRewardsForTournamentRank(rank);
		for (int i = 0; i < rewardsForTournamentRank.Prizes.Count; i++)
		{
			TournamentPrizeConfig tournamentPrizeConfig = rewardsForTournamentRank.Prizes[i];
			TournamentRewardItem[] componentsInChildren = this.grid.GetComponentsInChildren<TournamentRewardItem>();
			componentsInChildren[i].Init(tournamentPrizeConfig);
			this.numberOfParticipants.text = string.Format(L.Get("{0} Contestants"), tournamentPrizeConfig.RankTo.ToString());
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(0);
	}

	public IEnumerator AnimateAvatar(int targetPrizeTier)
	{
		TournamentRewardItem[] cells = this.grid.GetComponentsInChildren<TournamentRewardItem>();
		Vector3 startPosition = this.GetCellAvatarPosition(cells[cells.Length - 1]);
		this.avatarPivot.transform.position = new Vector3(startPosition.x, startPosition.y, this.avatarPivot.transform.position.z);
		this.avatarPivot.gameObject.SetActive(true);
		yield return FiberHelper.Wait(0.75f, (FiberHelper.WaitFlag)0);
		yield return FiberAnimation.ScaleTransform(this.avatarPivot.transform, Vector3.zero, Vector3.one, this.avatarAttentionCurve, 0f);
		for (int i = cells.Length - 1; i >= targetPrizeTier + 1; i--)
		{
			yield return this.JumpAvatarFromCellToCell(cells[i], cells[i - 1], 1f, 50f);
		}
		yield return FiberHelper.Wait(0.35f, (FiberHelper.WaitFlag)0);
		yield return cells[targetPrizeTier].AnimateGivingReward();
		yield break;
	}

	private Vector3 GetCellAvatarPosition(TournamentRewardItem cell)
	{
		return cell.transform.TransformPoint(new Vector3(-cell.GetElementSize().x * 0.5f, 0f, 0f));
	}

	private IEnumerator JumpAvatarFromCellToCell(TournamentRewardItem cellA, TournamentRewardItem cellB, float duration, float jumpWidth)
	{
		float avatarZ = this.avatarPivot.transform.position.z;
		Vector3 source = this.GetCellAvatarPosition(cellA);
		Vector3 dest = this.GetCellAvatarPosition(cellB);
		yield return FiberAnimation.Animate(duration, delegate(float t)
		{
			Vector3 position = Vector3.Lerp(source, dest, t);
			position.z = avatarZ;
			position.x -= Mathf.Sin(t * 3.14159274f) * jumpWidth;
			this.avatarPivot.transform.position = position;
		});
		yield break;
	}

	[SerializeField]
	private TournamentRewardItem rewardItemPrefab;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel numberOfParticipants;

	[SerializeField]
	private UIGridLayout grid;

	[SerializeField]
	private Transform selectionSprite;

	[SerializeField]
	private List<UIInstantiator> rankButtons;

	[SerializeField]
	private GameObject avatarPivot;

	[SerializeField]
	private AnimationCurve avatarAttentionCurve;

	private TournamentRank rank = TournamentRank.Bronze;
}
