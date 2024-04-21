using System;
using System.Collections;
using Fibers;
using TactileModules.FacebookExtras;
using TactileModules.Foundation;
using UnityEngine;

public class IntroView : UIView
{
	private FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	private FacebookLoginManager FacebookLoginManager
	{
		get
		{
			return ManagerRepository.Get<FacebookLoginManager>();
		}
	}

	private void StartGame(UIEvent e)
	{
		this.ViewClosed(BootFlow.IntroResult.Play);
	}

	private void StartGameDev(UIEvent e)
	{
		this.ViewClosed(BootFlow.IntroResult.PlayDeveloper);
	}

	protected override void ViewLoad(object[] parameters)
	{
	}

	protected override void ViewWillAppear()
	{
		AudioManager.Instance.SetMusic(null, true);
		this.PrepareSequence();
	}

	protected override void ViewDidAppear()
	{
		this.fiber.Start(this.Sequence());
	}

	protected override void ViewDidDisappear()
	{
		this.fiber.Terminate();
	}

	private void PrepareSequence()
	{
		this.logo.gameObject.SetActive(false);
		this.button.gameObject.SetActive(false);
		this.logInButtonPivot.gameObject.SetActive(false);
		SingingBand instance = this.singingBand.GetInstance<SingingBand>();
		instance.ConfigureSingers(false, null);
		instance.SetMultiTrack(SingletonAsset<MultiTrackDatabase>.Instance.title);
		instance.PrepareForDrop(base.GetElementSize().y);
		this.BackgroundAnimation.Skeleton.SetSkin(SingletonAsset<SingerDatabase>.Instance.GetMissingBandMember());
	}

	private IEnumerator Sequence()
	{
		yield return new Fiber.OnExit(delegate()
		{
			this.logo.gameObject.SetActive(true);
			this.button.gameObject.SetActive(true);
			this.logInButtonPivot.SetActive(this.FacebookClient.IsInitialized && !this.FacebookLoginManager.IsLoggedInAndUserRegistered);
		});
		SingingBand band = this.singingBand.GetInstance<SingingBand>();
		band.StartSinging(false);
		yield return band.AnimateDropFromAbove();
		yield break;
	}

	private void Update()
	{
		if (UIViewManager.Instance.IsEscapeKeyDownAndAvailable(base.gameObject.layer))
		{
			FiberCtrl.Pool.Run(this.AskToQuit(), false);
		}
		if (Input.GetMouseButtonDown(0))
		{
			while (this.fiber.Step())
			{
			}
		}
	}

	private IEnumerator AskToQuit()
	{
		UIViewManager.UIViewStateGeneric<MessageBoxView> vs = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
		{
			L.Get("Quit Game"),
			L.Get("Do you want to quit playing Cookie Cats POP?"),
			L.Get("Quit"),
			L.Get("Cancel")
		});
		yield return vs.WaitForClose();
		if ((int)vs.ClosingResult == 0)
		{
			Application.Quit();
		}
		yield break;
	}

	private IEnumerator DoFacebookInfo()
	{
		Boot.IsRequestsBlocked += false;
		UIViewManager.UIViewStateGeneric<FacebookLoginInfoView> vs = UIViewManager.Instance.ShowView<FacebookLoginInfoView>(new object[0]);
		yield return vs.WaitForClose();
		Boot.IsRequestsBlocked += true;
		if ((int)vs.ClosingResult == 1)
		{
			this.logInButtonPivot.SetActive(false);
			this.StartGame(default(UIEvent));
		}
		yield break;
	}

	private void ButtonFacebookLoginInfoClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.DoFacebookInfo(), false);
	}

	public UIWidget logo;

	public UIInstantiator button;

	public UIInstantiator singingBand;

	private Fiber fiber = new Fiber();

	public Action<BootFlow.IntroResult> ViewClosed = delegate(BootFlow.IntroResult A_0)
	{
	};

	public GameObject logInButtonPivot;

	public SkeletonAnimation BackgroundAnimation;
}
