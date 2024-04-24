using System;
using System.Collections;
using Fibers;
using JetBrains.Annotations;
using Tactile;

using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.UserSupport;
using TactileModules.UserSupport.ViewControllers;
using UnityEngine;

public class SettingsView : UIView
{
	
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
		this.UpdateUI();
	}

	protected override void ViewWillDisappear()
	{
		
	}

	private void UpdateUI()
	{
		this.musicToggleButton.GetInstance<ButtonToggle>().IsOn = AudioManager.Instance.MusicActive;
		this.soundToggleButton.GetInstance<ButtonToggle>().IsOn = AudioManager.Instance.SoundEffectsActive;
		this.notificationToggleButton.GetInstance<ButtonToggle>().IsOn = !NotificationManager.Instance.LocalNotificationsBlocked;
		ButtonWithTitle instance = this.facebookButton.GetInstance<ButtonWithTitle>();
		if (false)
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
