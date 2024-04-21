using System;
using System.Collections;

public class PlatformAlertView
{
	public static IEnumerator Show(string title, string message, string button0Text)
	{
		PlatformAlertView platformAlertView = new PlatformAlertView();
		return platformAlertView.ShowInternal(title, message, button0Text);
	}

	private IEnumerator ShowInternal(string title, string message, string button0Text)
	{
		this.callbackReceived = false;
		this.buttonPressed = null;
		EtceteraAndroidManager.alertButtonClickedEvent += this.AlertButtonClicked;
		EtceteraAndroidManager.alertCancelledEvent += this.AlertButtonCancelled;
		EtceteraAndroid.showAlert(title, message, button0Text);
		while (!this.callbackReceived)
		{
			yield return null;
		}
		EtceteraAndroidManager.alertButtonClickedEvent -= this.AlertButtonClicked;
		EtceteraAndroidManager.alertCancelledEvent -= this.AlertButtonCancelled;
		yield break;
	}

	public static IEnumerator Show(string title, string message, string button0Text, string button1Text, Action<int> callback)
	{
		PlatformAlertView platformAlertView = new PlatformAlertView();
		return platformAlertView.ShowInternal(title, message, button0Text, button1Text, callback);
	}

	private IEnumerator ShowInternal(string title, string message, string button0Text, string button1Text, Action<int> callback)
	{
		this.callbackReceived = false;
		this.buttonPressed = null;
		EtceteraAndroidManager.alertButtonClickedEvent += this.AlertButtonClicked;
		EtceteraAndroidManager.alertCancelledEvent += this.AlertButtonCancelled;
		this.callbackReceived = false;
		this.buttonPressed = null;
		EtceteraAndroid.showAlert(title, message, button1Text, button0Text);
		while (!this.callbackReceived)
		{
			yield return null;
		}
		if (callback != null)
		{
			callback((!(this.buttonPressed == button1Text)) ? 0 : 1);
		}
		EtceteraAndroidManager.alertButtonClickedEvent -= this.AlertButtonClicked;
		EtceteraAndroidManager.alertCancelledEvent -= this.AlertButtonCancelled;
		yield break;
	}

	private void AlertButtonClicked(string buttonText)
	{
		this.callbackReceived = true;
		this.buttonPressed = buttonText;
	}

	private void AlertButtonCancelled()
	{
		this.callbackReceived = true;
		this.buttonPressed = "__CANCELLED__";
	}

	private bool callbackReceived;

	private string buttonPressed;
}
