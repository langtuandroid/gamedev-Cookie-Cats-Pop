using System;
using System.Collections;
using TactileModules.Placements;

public class SendLivesToFriendsPlacement : IPlacementRunnableNoBreak, IPlacementRunnable
{
	public SendLivesToFriendsPlacement(SendLivesAtStartManager sendLivesAtStartManager)
	{
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
			
		}
		yield break;
	}

	private bool ShouldShowPopup()
	{
		return false && this.sendLivesAtStartManager.CanShowSendLivesAtStart();
	}



	private readonly SendLivesAtStartManager sendLivesAtStartManager;
}
