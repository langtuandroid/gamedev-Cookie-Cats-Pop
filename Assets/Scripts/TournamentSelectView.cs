using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using UnityEngine;

public class TournamentSelectView : UIView
{
	protected override void ViewWillAppear()
	{
		this.tournamentManager = TournamentManager.Instance;
		this.tournamentCloudManager = TournamentManager.Instance.Cloud;
		this.tournamentSetup = SingletonAsset<TournamentSetup>.Instance;
		for (int i = 0; i < this.rankButtons.Count; i++)
		{
			this.rankButtons[i].GetInstance().GetComponent<ITournamentRankSelectButton>().Init(i + TournamentRank.Bronze);
		}
		this.ShowHighestJoinableTournament();
		this.UpdateUI();
	}

	protected override void ViewDidAppear()
	{
		this.updateFiber.Start(this.UpdateUIContinuously());
	}

	private IEnumerator UpdateUIContinuously()
	{
		for (;;)
		{
			if (!this.tournamentCloudManager.TournamentOpen)
			{
				this.timerCountdown.text = this.tournamentManager.GetTimeRemainingForNextTournamentAsString();
				if (this.tournamentCloudManager.TournamentEnded)
				{
					bool requestSucceeded = false;
					yield return this.tournamentManager.UpdateStatusOrShowDialog(delegate(bool success)
					{
						requestSucceeded = success;
					});
					if (!requestSucceeded)
					{
						base.Close(1);
					}
				}
			}
			this.UpdateUI();
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	private void UpdateUI()
	{
		this.rankGrid.SetActive(true);
		if (this.tournamentCloudManager.TournamentOpen)
		{
			this.EnablePivot(this.joinablePivot);
		}
		else if (!this.tournamentCloudManager.TournamentEnded)
		{
			this.rankGrid.SetActive(false);
			this.EnablePivot(this.unjoinablePivot);
			if (TournamentManager.Instance.UserWillBeNotifiedWhenTournamentIsJoinable)
			{
				this.EnablePivot(this.notifyPivot);
			}
			else
			{
				this.EnablePivot(this.dontNotifyPivot);
			}
		}
	}

	private void EnablePivot(GameObject pivot)
	{
		this.unjoinablePivot.SetActive(pivot != this.joinablePivot);
		this.notifyPivot.SetActive(pivot == this.notifyPivot);
		this.dontNotifyPivot.SetActive(pivot == this.dontNotifyPivot);
		this.joinablePivot.SetActive(pivot == this.joinablePivot);
	}

	protected override void ViewWillDisappear()
	{
		this.updateFiber.Terminate();
		this.joinFiber.Terminate();
	}

	private void TournamentButtonClicked(UIEvent e)
	{
		TournamentRank tournamentRank = e.sender.GetComponent<ITournamentRankSelectButton>().Rank;
		if (!this.tournamentManager.IsTournamentRankUnlocked(tournamentRank))
		{
			UIViewManager.Instance.ShowView<TournamentLockedView>(new object[0]).View.SetLockedRank(tournamentRank);
			return;
		}
		this.SwitchRank(tournamentRank);
	}

	private void TournamentInfoButtonClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<TournamentInfoView>(new object[0]).View.Initialize(this.rank);
	}

	private void ReflectRankToUI()
	{
		foreach (UIInstantiator uiinstantiator in this.rankButtons)
		{
			ITournamentRankSelectButton component = uiinstantiator.GetInstance().GetComponent<ITournamentRankSelectButton>();
			component.Selected = (component.Rank == this.rank);
		}
		TournamentSetup.RankSetup rankSetup = this.tournamentSetup.GetRankSetup(this.rank);
		this.rankName.text = L.Get(rankSetup.displayName);
		this.rankName.fontStyle = rankSetup.fontStyle;
		this.rankIcon.TextureResource = rankSetup.bigIconResourcePath;
		TicketMetaData metaData = InventoryManager.Instance.GetMetaData<TicketMetaData>(rankSetup.ticketItem);
		this.rankCostIcon.SpriteName = metaData.IconSpriteName;
	}

	private void SwitchRank(TournamentRank newRank)
	{
		this.rank = newRank;
		this.ReflectRankToUI();
		if (this.RankChanged != null)
		{
			this.RankChanged(newRank);
		}
	}

	private void DismissClicked(UIEvent e)
	{
		base.Close(TournamentSelectView.CloseResult.Dimissed);
	}

	private void JoinButtonClicked(UIEvent e)
	{
		this.joinFiber.Start(this.HandleUserWantsToJoinTournament());
	}

	private IEnumerator ShowTicketShop(TournamentRank r)
	{
		UIViewManager.UIViewStateGeneric<TicketShopView> vs = UIViewManager.Instance.ShowView<TicketShopView>(new object[]
		{
			this.rank
		});
		yield return vs.WaitForClose();
		yield break;
	}

	private IEnumerator HandleUserWantsToJoinTournament()
	{
		bool didSucceed = false;
		yield return this.tournamentManager.TryJoin(this.rank, new Func<TournamentRank, IEnumerator>(this.ShowTicketShop), delegate(bool success)
		{
			didSucceed = success;
		});
		if (didSucceed)
		{
			yield return this.ticketPanel.GetInstance<TicketPanel>().AnimateTicket(this.rank, TicketPanel.AnimationDirection.FromPanel, this.rankCostIcon.transform.position);
			base.Close(TournamentSelectView.CloseResult.Joined);
		}
		yield break;
	}

	private void TicketPanelClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<TicketShopView>(new object[]
		{
			this.rank
		});
	}

	private void NotifyMeWhenTournamentIsJoinable(UIEvent e)
	{
		this.tournamentManager.NotifyUserWhenTournamentIsJoinable();
	}

	private void ShowHighestJoinableTournament()
	{
		TournamentRank highestAvailableTournament = this.tournamentManager.GetHighestAvailableTournament();
		this.SwitchRank(highestAvailableTournament);
	}

	public Action<TournamentRank> RankChanged;

	[SerializeField]
	private UIResourceQuad rankIcon;

	[SerializeField]
	private UILabel rankName;

	[SerializeField]
	private UISprite rankCostIcon;

	[SerializeField]
	private GameObject rankGrid;

	[SerializeField]
	private List<UIInstantiator> rankButtons;

	[SerializeField]
	private UILabel timerCountdown;

	[SerializeField]
	private UIInstantiator ticketPanel;

	[SerializeField]
	private GameObject unjoinablePivot;

	[SerializeField]
	private GameObject notifyPivot;

	[SerializeField]
	private GameObject dontNotifyPivot;

	[SerializeField]
	private GameObject joinablePivot;

	private TournamentRank rank = TournamentRank.Bronze;

	private readonly Fiber joinFiber = new Fiber();

	private readonly Fiber updateFiber = new Fiber();

	private TournamentManager tournamentManager;

	private TournamentCloudManager tournamentCloudManager;

	private TournamentSetup tournamentSetup;

	public enum CloseResult
	{
		Joined,
		Dimissed
	}
}
