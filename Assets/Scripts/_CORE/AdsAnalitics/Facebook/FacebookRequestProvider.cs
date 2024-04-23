using System;

public class FacebookRequestProvider : FacebookRequestManager.IDataProvider
{
	public string GetLocalizedInviteFriendsTitleText()
	{
		return L.Get("Invite friends to play Cookie Cats POP");
	}

	public string GetLocalizedInviteFriendsMessageText()
	{
		return L.Get("The neighborhood cats are hungry for cookies, and only YOU can help them!");
	}
}
