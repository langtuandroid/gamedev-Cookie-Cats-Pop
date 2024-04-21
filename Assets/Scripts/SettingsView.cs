using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;
using Tactile;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.UserSupport;
using TactileModules.UserSupport.ViewControllers;
using UnityEngine;

public class SettingsView : UIView
{
	private FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.versionLabel.text = "Version " + SystemInfoHelper.BundleShortVersion + "-" + SystemInfoHelper.BundleVersion;
		this.configVersionLabel.text = "Config v" + this.ConfigurationManager.GetVersion();
		MainLevelDatabase levelDatabase = this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		if (levelDatabase.AssetBundleInfo != null)
		{
			this.levelDatabaseLabel.text = levelDatabase.AssetBundleInfo.Filename;
			int startIndex = levelDatabase.AssetBundleInfo.URL.LastIndexOf('/');
			UILabel uilabel = this.levelDatabaseLabel;
			uilabel.text = uilabel.text + " (" + levelDatabase.AssetBundleInfo.URL.Substring(startIndex) + ")";
		}
		else
		{
			this.levelDatabaseLabel.text = "Default Database";
		}
	}

	protected override void ViewWillAppear()
	{
		this.FacebookClient.FacebookLogin += this.UpdateUI;
		this.FacebookClient.FacebookLogout += this.UpdateUI;
		this.UpdateUI();
	}

	protected override void ViewWillDisappear()
	{
		this.FacebookClient.FacebookLogin -= this.UpdateUI;
		this.FacebookClient.FacebookLogout -= this.UpdateUI;
	}

	private void UpdateUI()
	{
		this.musicToggleButton.GetInstance<ButtonToggle>().IsOn = AudioManager.Instance.MusicActive;
		this.soundToggleButton.GetInstance<ButtonToggle>().IsOn = AudioManager.Instance.SoundEffectsActive;
		this.notificationToggleButton.GetInstance<ButtonToggle>().IsOn = !NotificationManager.Instance.LocalNotificationsBlocked;
		ButtonWithTitle instance = this.facebookButton.GetInstance<ButtonWithTitle>();
		if (this.FacebookClient.IsSessionValid)
		{
			instance.Title = L.Get("Log Out");
		}
		else
		{
			instance.Title = L.Get("Log In");
		}
		this.notificationToggleButton.GetInstance<ButtonToggle>().Title = this.GetNotificationsButtonText(NotificationManager.Instance.LocalNotificationsBlocked);
	}

	[UsedImplicitly]
	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	[UsedImplicitly]
	private void CheatClicked(UIEvent e)
	{
	}

	[UsedImplicitly]
	private void MusicClicked(UIEvent e)
	{
		ButtonToggle component = e.sender.GetComponent<ButtonToggle>();
		AudioManager.Instance.MusicActive = component.IsOn;
	}

	[UsedImplicitly]
	private void SoundClicked(UIEvent e)
	{
		ButtonToggle component = e.sender.GetComponent<ButtonToggle>();
		AudioManager.Instance.SoundEffectsActive = component.IsOn;
	}

	[UsedImplicitly]
	private void FacebookLogInOrOut(UIEvent e)
	{
		if (this.FacebookClient.IsSessionValid)
		{
			UIViewManager.UIViewStateGeneric<ProgressView> vs = UIViewManager.Instance.ShowView<ProgressView>(new object[]
			{
				L.Get("Logging Out")
			});
			FiberCtrl.Pool.Run(this.FacebookClient.DeletePermissions(delegate(object err)
			{
				vs.View.Close(0);
				if (err == null)
				{
					this.CloudClient.ClearCachedAndPersistedUserData();
					this.Close(1);
				}
				else
				{
					UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
					{
						L.Get("Sorry"),
						L.Get("There was a problem when trying to log out. Please try again later.")
					});
				}
			}), false);
		}
		else
		{
			FiberCtrl.Pool.Run(this.DoFacebookLogin(), false);
		}
	}

	[UsedImplicitly]
	private void ToStartScreen(UIEvent e)
	{
		FlowStack flowStack = ManagerRepository.Get<FlowStack>();
		flowStack.Push(new SettingsView.StartScreenFlow());
		base.Close(0);
	}

	[UsedImplicitly]
	private void HelpClicked(UIEvent e)
	{
		IUserSupportViewControllerFactory controllerFactory = ManagerRepository.Get<UserSupportSystem>().ControllerFactory;
		IUIViewController iuiviewController = controllerFactory.CreateConversationViewController();
		iuiviewController.ShowView();
	}

	[UsedImplicitly]
	private void CancelNotificationsClicked(UIEvent e)
	{
		NotificationManager.Instance.LocalNotificationsBlocked = !NotificationManager.Instance.LocalNotificationsBlocked;
		this.notificationToggleButton.GetInstance<ButtonToggle>().Title = this.GetNotificationsButtonText(NotificationManager.Instance.LocalNotificationsBlocked);
		this.UpdateUI();
	}

	private string GetNotificationsButtonText(bool disabled)
	{
		if (!disabled)
		{
			return L.Get("Notifications Enabled");
		}
		return L.Get("Notifications Disabled");
	}

	private IEnumerator DoFacebookLogin()
	{
		IEnumerator e = this.FacebookLoginManager.EnsureLoggedInAndUserRegistered();
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	[SerializeField]
	private UIInstantiator musicToggleButton;

	[SerializeField]
	private UIInstantiator soundToggleButton;

	[SerializeField]
	private UIInstantiator notificationToggleButton;

	[SerializeField]
	private UILabel versionLabel;

	[SerializeField]
	private UILabel levelDatabaseLabel;

	[SerializeField]
	private UILabel configVersionLabel;

	[SerializeField]
	private UIInstantiator facebookButton;

	[SerializeField]
	private GameObject helpButton;

	public class StartScreenFlow : IFlow, IFullScreenOwner, IFiberRunnable
	{
		public IEnumerator Run()
		{
			FullScreenManager screenManager = ManagerRepository.Get<FullScreenManager>();
			yield return screenManager.Push(this);
			this.isClosed = false;
			while (!this.isClosed)
			{
				yield return null;
			}
			yield return screenManager.Pop();
			yield break;
		}

		public void OnExit()
		{
		}

		public IEnumerator ScreenAcquired()
		{
			UIViewManager.UIViewStateGeneric<IntroView> uiviewStateGeneric = UIViewManager.Instance.ShowView<IntroView>(new object[0]);
			IntroView view = uiviewStateGeneric.View;
			IntroView introView = view;
			introView.ViewClosed = (Action<BootFlow.IntroResult>)Delegate.Combine(introView.ViewClosed, new Action<BootFlow.IntroResult>(this.HandleViewClosed));
			yield break;
		}

		private void HandleViewClosed(BootFlow.IntroResult obj)
		{
			this.isClosed = true;
		}

		public void ScreenLost()
		{
		}

		public void ScreenReady()
		{
		}

		private bool isClosed;
	}
}
