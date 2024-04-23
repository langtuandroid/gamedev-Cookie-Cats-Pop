using System;
using System.Collections;
using Tactile;

public class AskForNotificationManager : MapPopupManager.IMapPopup
{
	private AskForNotificationManager(AskForNotificationManager.IAskForNotificationManagerProvider pProvider)
	{
		this.provider = pProvider;
		MapPopupManager.Instance.RegisterPopupObject(this);
	}

	private bool PersistedUserAcceptNotification
	{
		get
		{
			return TactilePlayerPrefs.GetBool("AskForNotificationManager|AcceptedNotification", false);
		}
		set
		{
			TactilePlayerPrefs.SetBool("AskForNotificationManager|AcceptedNotification", value);
		}
	}

	public static AskForNotificationManager Instance { get; private set; }

	public static AskForNotificationManager CreateInstance(AskForNotificationManager.IAskForNotificationManagerProvider pProvider)
	{
		if (AskForNotificationManager.Instance != null)
		{
			throw new Exception("AskForNotificationManager Instance already exists!");
		}
		AskForNotificationManager.Instance = new AskForNotificationManager(pProvider);
		return AskForNotificationManager.Instance;
	}

	private bool IsUserAccept()
	{
		return this.PersistedUserAcceptNotification;
	}

	private IEnumerator ShowAskForNotificationView()
	{
		UIViewManager.UIViewState vs = UIViewManager.Instance.ShowView<AskForNotificationView>(new object[0]);
		yield return vs.WaitForClose();
		if ((int)vs.ClosingResult > 0)
		{
			this.PersistedUserAcceptNotification = true;
			this.provider.OneSignalManager.RegisterForPush();
			yield return null;
		}
		yield break;
	}

	private bool ShouldShowPopup(int unlockedLevelIndex)
	{
		if (this.IsUserAccept() || this.provider.OneSignalManager.IsRegisteredForNotifications)
		{
			return false;
		}
		int levelRequired = this.provider.Config.LevelRequired;
		return unlockedLevelIndex >= levelRequired && this.provider.Config.RepeatStep > 0 && (unlockedLevelIndex - levelRequired) % this.provider.Config.RepeatStep == 0;
	}

	public void TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		if (this.ShouldShowPopup(unlockedLevelIndex))
		{
			popupFlow.AddPopup(this.ShowAskForNotificationView());
		}
	}

	private const string acceptedNotificationKey = "AskForNotificationManager|AcceptedNotification";

	private AskForNotificationManager.IAskForNotificationManagerProvider provider;

	public interface IAskForNotificationManagerProvider
	{
		AskForNotificationManager.AskForNotificationConfig Config { get; }

		OneSignalManager OneSignalManager { get; }
	}

	[ConfigProvider("AskForNotifications")]
	public class AskForNotificationConfig
	{
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[JsonSerializable("RepeatStep", null)]
		public int RepeatStep { get; set; }
	}
}
