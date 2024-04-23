using System;
using System.Collections;
using Tactile;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;

public class FacebookLoginManagerProvider : IFacebookLoginManagerProvider
{
	CloudClientBase IFacebookLoginManagerProvider.CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	public FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	string IFacebookLoginManagerProvider.ProductName
	{
		get
		{
			return Constants.FACEBOOK_APP_DISPLAY_NAME;
		}
	}

	IEnumerator IFacebookLoginManagerProvider.PostSuccessfulLogin()
	{
		yield return ManagerRepository.Get<UserSettingsManager>().SyncUserSettingsCr();
		yield break;
	}
}
