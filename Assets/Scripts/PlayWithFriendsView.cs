using System;
using System.Collections;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;

public class PlayWithFriendsView : UIView
{
	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	private void Dismiss(UIEvent e)
	{
		base.Close(0);
	}

	public void ButtonFacebookLoginClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.DoFacebookLogin(), false);
	}

	public IEnumerator DoFacebookLogin()
	{
		yield return this.FacebookLoginManager.EnsureLoggedInAndUserRegistered();
		base.Close(0);
		yield break;
	}
}
