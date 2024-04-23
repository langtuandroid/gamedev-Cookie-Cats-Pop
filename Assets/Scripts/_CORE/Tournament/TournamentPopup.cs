using System;
using System.Collections;
using System.Collections.Generic;

[MapPopupIdentifier("TournamentManager")]
public class TournamentPopup : MapPopupManager.IMapPopup
{
	public TournamentPopup(TournamentManager tournamentManager, TournamentControllerFactory controllerFactory)
	{
		this.tournamentManager = tournamentManager;
		this.controllerFactory = controllerFactory;
	}

	void MapPopupManager.IMapPopup.TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		if (this.ShouldShowPopup())
		{
			popupFlow.AddPopup(this.ShowPopupMessage());
		}
	}

	private bool ShouldShowPopup()
	{
		bool flag = this.tournamentManager.IsTournamentRankUnlocked(TournamentRank.Bronze);
		return !this.tournamentManager.GetPersistedTournamentUnlocked() && flag;
	}

	private IEnumerator ShowPopupMessage()
	{
		this.tournamentManager.AddTicketsToUser(TournamentRank.Bronze, 3, "tournamentsUnlocked");
		this.tournamentManager.SetPersistedTournamentUnlocked();
		yield return this.ShowUnlockedMessage();
		yield break;
	}

	private IEnumerator ShowUnlockedMessage()
	{
		bool isComplete = false;
		List<ItemAmount> rewards = new List<ItemAmount>
		{
			new ItemAmount
			{
				ItemId = "TicketBronze",
				Amount = 3
			}
		};
		UIViewManager.UIViewStateGeneric<TournamentUnlockedView> vs = UIViewManager.Instance.ShowView<TournamentUnlockedView>(new object[0]);
		vs.View.Initialize(rewards, delegate
		{
			isComplete = true;
		});
		while (!isComplete)
		{
			yield return null;
		}
		this.controllerFactory.CreateAndPushMapFlow();
		yield break;
	}

	private readonly TournamentManager tournamentManager;

	private readonly TournamentControllerFactory controllerFactory;
}
