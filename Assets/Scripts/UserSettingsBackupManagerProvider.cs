using System;
using System.Collections;
using Tactile;
using UnityEngine;

public class UserSettingsBackupManagerProvider : UserSettingsBackupManager.IUserSettingsBackupManagerProvider
{
	public UserSettingsBackupManager.Configuration GetConfiguration()
	{
		return ConfigurationManager.Get<UserSettingsBackupManager.Configuration>();
	}

	public string GetUserSettingsAsJson()
	{
		Hashtable value;
		Hashtable value2;
		UserSettingsManager.Instance.UserSettingsToHashTable(out value, out value2);
		Hashtable obj = new Hashtable
		{
			{
				"privateSettings",
				value
			},
			{
				"publicSettings",
				value2
			}
		};
		return obj.toJson();
	}

	public void RestoreFromUserSettings(string userSettingsJson)
	{
		Hashtable hashtable = userSettingsJson.hashtableFromJson();
		bool flag = UserSettingsManager.Instance.Restore(hashtable["privateSettings"] as Hashtable, hashtable["publicSettings"] as Hashtable);
		if (flag)
		{
			UIViewManager.UIViewStateGeneric<MessageBoxView> vs = UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
			{
				L.Get("Backup successfully restored!"),
				L.Get("The application will now close. Please relaunch the application"),
				L.Get("Ok")
			});
			FiberCtrl.Pool.Run(this.WaitForClose(vs), false);
		}
		else
		{
			this.FailedToRestore(L.Get("Something went wrong, please try again later"));
		}
	}

	public void FailedToRestore(string message)
	{
		UIViewManager.Instance.ShowView<MessageBoxView>(new object[]
		{
			L.Get("Restoring from backup failed!"),
			message,
			L.Get("Ok")
		});
	}

	private IEnumerator WaitForClose(UIViewManager.UIViewState vs)
	{
		yield return vs.WaitForClose();
		Application.Quit();
		yield break;
	}
}
