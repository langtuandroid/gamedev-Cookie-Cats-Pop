using System;
using System.Collections;
using TactileModules.Placements;

public class SendLivesToFriendsPlacement : IPlacementRunnableNoBreak, IPlacementRunnable
{
	public SendLivesToFriendsPlacement(FacebookClient facebookClient, SendLivesAtStartManager sendLivesAtStartManager)
	{
		this.facebookClient = facebookClient;
		this.sendLivesAtStartManager = sendLivesAtStartManager;
	}

	public string ID
	{
		get
		{
			return "SendLivesToFriends";
		}
	}

	public IEnumerator Run(IPlacementViewMediator placementViewMediator)
	{
		if (this.ShouldShowPopup())
		{
			yield return this.ShowPopup();
		}
		yield break;
	}

	private bool ShouldShowPopup()
	{
		return this.facebookClient.IsSessionValid && this.sendLivesAtStartManager.CanShowSendLivesAtStart();
	}

	private IEnumerator ShowPopup()
	{
		UIViewManager.UIViewStateGeneric<FacebookSelectFriendsAndRequestView> vs = UIViewManager.Instance.ShowView<FacebookSelectFriendsAndRequestView>(new object[]
		{
			FacebookSelectFriendsAndRequestView.RequestType.GiftLives,
			true
		});
		yield return vs.WaitForClose();
		yield break;
	}

	private readonly FacebookClient facebookClient;

	private readonly SendLivesAtStartManager sendLivesAtStartManager;
}
