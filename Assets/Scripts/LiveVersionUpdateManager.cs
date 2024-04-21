using System;
using System.Collections;
using Tactile;
using UnityEngine;

public class LiveVersionUpdateManager : MapPopupManager.IMapPopup
{
	public LiveVersionUpdateManager()
	{
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		if (this.ShouldShowPopup())
		{
			popupFlow.AddPopup(this.ShowPopup());
		}
	}

	private LiveVersionConfig Config
	{
		get
		{
			return ConfigurationManager.Get<LiveVersionConfig>();
		}
	}

	private bool ShouldShowPopup()
	{
		return this.Config != null && this.Config.VersionNumber > int.Parse(SystemInfoHelper.BundleVersion);
	}

	private IEnumerator ShowPopup()
	{
		UIViewManager.UIViewStateGeneric<MessageBoxView> vs = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
		{
			L.Get("New Version"),
			L.Get("New version available. Update now?"),
			L.Get("Update"),
			L.Get("Not Now")
		});
		yield return vs.WaitForClose();
		if ((int)vs.ClosingResult == 0)
		{
			string url = string.Empty;
			string bundleIdentifier = SystemInfoHelper.BundleIdentifier;
			url = "market://details?id=" + bundleIdentifier;
			Application.OpenURL(url);
		}
		yield break;
	}
}
