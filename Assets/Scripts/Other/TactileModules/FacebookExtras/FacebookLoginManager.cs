using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Prime31;
using UnityEngine;

namespace TactileModules.FacebookExtras
{
	public class FacebookLoginManager
	{
		public FacebookLoginManager([NotNull] IFacebookLoginManagerProvider provider, IDialogViewProvider dialogViewProvider)
		{
			this.provider = provider;
			this.DialogViews = dialogViewProvider;
		}

		private FacebookClient FacebookClient
		{
			get
			{
				return this.provider.FacebookClient;
			}
		}

		public bool IsLoggedInAndUserRegistered
		{
			get
			{
				return this.FacebookClient.IsSessionValid && this.provider.CloudClient.HasValidUser;
			}
		}

		public IEnumerator EnsureLoggedInAndUserRegistered()
		{
			if (this.IsLoggedInAndUserRegistered)
			{
				yield break;
			}
			object vs = this.DialogViews.ShowProgressView(L.Get("Logging In"));
			while (!this.FacebookClient.IsInitialized)
			{
				yield return null;
			}
			if (this.FacebookClient.IsSessionValid)
			{
				yield return this.FacebookClient.Update();
			}
			object loginError = null;
			if (!this.FacebookClient.IsSessionValid)
			{
				yield return this.FacebookClient.Login(FacebookLoginManager.loginPermissions, delegate(object err)
				{
					loginError = err;
				});
				if (this.FacebookClient.IsSessionValid)
				{
					yield return this.FacebookClient.Update();
				}
			}
			if (this.FacebookClient.IsSessionValid && loginError == null)
			{
				yield return this.provider.CloudClient.UpdateRegistrationCr();
				yield return this.provider.PostSuccessfulLogin();
			}
			this.DialogViews.CloseView(vs);
			yield return this.DialogViews.WaitForClosingView(vs);
			if (this.FacebookClient.IsSessionValid && this.provider.CloudClient.HasValidUser)
			{
				FacebookAnalytics.LogUserRegistered(true, this.FacebookClient.CachedMe);
			}
			if (loginError != null)
			{
				yield return this.DisplayLoginError(loginError);
			}
			yield break;
		}

		public IEnumerator EnsureFriendListPermissions()
		{
			if (this.FacebookClient.HasFriendListPermissions)
			{
				yield break;
			}
			object vs = this.DialogViews.ShowProgressView(L.Get("Requesting FriendList Permissions"));
			object loginError = null;
			yield return this.FacebookClient.ReauthorizeWithReadPermissions(new List<string>
			{
				"user_friends"
			}, delegate(object err)
			{
				loginError = err;
			});
			if (loginError == null)
			{
				yield return this.FacebookClient.Update();
			}
			if (loginError == null)
			{
				yield return this.provider.CloudClient.UpdateRegistrationCr();
			}
			this.DialogViews.CloseView(vs);
			yield return this.DialogViews.WaitForClosingView(vs);
			if (loginError != null)
			{
				yield return this.DisplayLoginError(loginError);
			}
			yield break;
		}

		public IEnumerator EnsurePublishPermissions()
		{
			if (this.FacebookClient.HasPublishPermissions)
			{
				yield break;
			}
			object vs = this.DialogViews.ShowProgressView(L.Get("Requesting Publish Permissions"));
			object loginError = null;
			yield return this.FacebookClient.ReauthorizeWithPublishPermissions(new List<string>
			{
				"publish_actions"
			}, delegate(object err)
			{
				loginError = err;
			});
			this.DialogViews.CloseView(vs);
			yield return this.DialogViews.WaitForClosingView(vs);
			if (loginError != null)
			{
				yield return this.DisplayLoginError(loginError);
			}
			yield break;
		}

		public static bool LoginErrorMessage(object loginError, string message)
		{
			P31Error p31Error = loginError as P31Error;
			if (p31Error == null || p31Error.message != null)
			{
			}
			return p31Error != null && p31Error.message != null && p31Error.message.IndexOf(message) != -1;
		}

		public IEnumerator DisplayLoginError(object loginError)
		{
			if (!FacebookLoginManager.LoginErrorMessage(loginError, "The user denied the app") && !FacebookLoginManager.LoginErrorMessage(loginError, "User canceled log in") && !FacebookLoginManager.LoginErrorMessage(loginError, "User canceled login"))
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					object vs = this.DialogViews.ShowMessageBox(L._("Facebook login failed"), L._("You need to be connected to the internet to use this feature"), L._("Ok"), null);
					yield return this.DialogViews.WaitForClosingView(vs);
				}
				else
				{
					object vs2 = this.DialogViews.ShowMessageBox(L._("Facebook login failed"), string.Format(L._("Please make sure you have authorized {0} in your Facebook settings."), this.provider.ProductName), L._("Ok"), null);
					yield return this.DialogViews.WaitForClosingView(vs2);
				}
			}
			yield break;
		}

		private IFacebookLoginManagerProvider provider;

		private static readonly List<string> loginPermissions = new List<string>
		{
			"public_profile",
			"email",
			"user_friends"
		};

		private readonly IDialogViewProvider DialogViews;
	}
}
