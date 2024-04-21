using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Prime31;

public class EtceteraAndroidManager : AbstractManager
{
	static EtceteraAndroidManager()
	{
		AbstractManager.initialize(typeof(EtceteraAndroidManager));
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> alertButtonClickedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action alertCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> promptFinishedWithTextEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action promptCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string, string> twoFieldPromptFinishedWithTextEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action twoFieldPromptCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action webViewCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action albumChooserCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> albumChooserSucceededEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action photoChooserCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> photoChooserSucceededEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> videoRecordingSucceededEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action videoRecordingCancelledEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action ttsInitializedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action ttsFailedToInitializeEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action askForReviewWillOpenMarketEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action askForReviewRemindMeLaterEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action askForReviewDontAskAgainEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> inlineWebViewJSCallbackEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<string> notificationReceivedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<List<EtceteraAndroid.Contact>> contactsLoadedEvent;

	public void alertButtonClicked(string positiveButton)
	{
		if (EtceteraAndroidManager.alertButtonClickedEvent != null)
		{
			EtceteraAndroidManager.alertButtonClickedEvent(positiveButton);
		}
	}

	public void alertCancelled(string empty)
	{
		if (EtceteraAndroidManager.alertCancelledEvent != null)
		{
			EtceteraAndroidManager.alertCancelledEvent();
		}
	}

	public void promptFinishedWithText(string text)
	{
		string[] array = text.Split(new string[]
		{
			"|||"
		}, StringSplitOptions.None);
		if (array.Length == 1 && EtceteraAndroidManager.promptFinishedWithTextEvent != null)
		{
			EtceteraAndroidManager.promptFinishedWithTextEvent(array[0]);
		}
		if (array.Length == 2 && EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent != null)
		{
			EtceteraAndroidManager.twoFieldPromptFinishedWithTextEvent(array[0], array[1]);
		}
	}

	public void promptCancelled(string empty)
	{
		if (EtceteraAndroidManager.promptCancelledEvent != null)
		{
			EtceteraAndroidManager.promptCancelledEvent();
		}
	}

	public void twoFieldPromptCancelled(string empty)
	{
		if (EtceteraAndroidManager.twoFieldPromptCancelledEvent != null)
		{
			EtceteraAndroidManager.twoFieldPromptCancelledEvent();
		}
	}

	public void webViewCancelled(string empty)
	{
		if (EtceteraAndroidManager.webViewCancelledEvent != null)
		{
			EtceteraAndroidManager.webViewCancelledEvent();
		}
	}

	public void albumChooserCancelled(string empty)
	{
		if (EtceteraAndroidManager.albumChooserCancelledEvent != null)
		{
			EtceteraAndroidManager.albumChooserCancelledEvent();
		}
	}

	public void albumChooserSucceeded(string path)
	{
		if (EtceteraAndroidManager.albumChooserSucceededEvent != null)
		{
			if (File.Exists(path))
			{
				EtceteraAndroidManager.albumChooserSucceededEvent(path);
			}
			else if (EtceteraAndroidManager.albumChooserCancelledEvent != null)
			{
				EtceteraAndroidManager.albumChooserCancelledEvent();
			}
		}
	}

	public void photoChooserCancelled(string empty)
	{
		if (EtceteraAndroidManager.photoChooserCancelledEvent != null)
		{
			EtceteraAndroidManager.photoChooserCancelledEvent();
		}
	}

	public void photoChooserSucceeded(string path)
	{
		if (EtceteraAndroidManager.photoChooserSucceededEvent != null)
		{
			if (File.Exists(path))
			{
				EtceteraAndroidManager.photoChooserSucceededEvent(path);
			}
			else if (EtceteraAndroidManager.photoChooserCancelledEvent != null)
			{
				EtceteraAndroidManager.photoChooserCancelledEvent();
			}
		}
	}

	public void videoRecordingSucceeded(string path)
	{
		if (EtceteraAndroidManager.videoRecordingSucceededEvent != null)
		{
			EtceteraAndroidManager.videoRecordingSucceededEvent(path);
		}
	}

	public void videoRecordingCancelled(string empty)
	{
		if (EtceteraAndroidManager.videoRecordingCancelledEvent != null)
		{
			EtceteraAndroidManager.videoRecordingCancelledEvent();
		}
	}

	public void ttsInitialized(string result)
	{
		bool flag = result == "1";
		if (flag && EtceteraAndroidManager.ttsInitializedEvent != null)
		{
			EtceteraAndroidManager.ttsInitializedEvent();
		}
		if (!flag && EtceteraAndroidManager.ttsFailedToInitializeEvent != null)
		{
			EtceteraAndroidManager.ttsFailedToInitializeEvent();
		}
	}

	public void ttsUtteranceCompleted(string utteranceId)
	{
	}

	public void askForReviewWillOpenMarket(string empty)
	{
		if (EtceteraAndroidManager.askForReviewWillOpenMarketEvent != null)
		{
			EtceteraAndroidManager.askForReviewWillOpenMarketEvent();
		}
	}

	public void askForReviewRemindMeLater(string empty)
	{
		if (EtceteraAndroidManager.askForReviewRemindMeLaterEvent != null)
		{
			EtceteraAndroidManager.askForReviewRemindMeLaterEvent();
		}
	}

	public void askForReviewDontAskAgain(string empty)
	{
		if (EtceteraAndroidManager.askForReviewDontAskAgainEvent != null)
		{
			EtceteraAndroidManager.askForReviewDontAskAgainEvent();
		}
	}

	public void inlineWebViewJSCallback(string message)
	{
		EtceteraAndroidManager.inlineWebViewJSCallbackEvent.fire(message);
	}

	public void notificationReceived(string extraData)
	{
		EtceteraAndroidManager.notificationReceivedEvent.fire(extraData);
	}

	private void contactsLoaded(string json)
	{
		if (EtceteraAndroidManager.contactsLoadedEvent != null)
		{
			List<EtceteraAndroid.Contact> obj = Json.decode<List<EtceteraAndroid.Contact>>(json, null);
			EtceteraAndroidManager.contactsLoadedEvent(obj);
		}
	}
}
