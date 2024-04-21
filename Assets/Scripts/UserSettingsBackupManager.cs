using System;
using System.Collections;
using Cloud;
using Fibers;
using Tactile;
using TactileModules.Foundation.CloudSynchronization;

public class UserSettingsBackupManager : ICloudSynchronizable
{
	public UserSettingsBackupManager(UserSettingsBackupManager.IUserSettingsBackupManagerProvider provider, ICloudInterfaceBase cloudInterfaceBase, TimeStampManager timeStampManager)
	{
		this.cloudInterfaceBase = cloudInterfaceBase;
		this.provider = provider;
		this.timeStampManager = timeStampManager;
		this.checkFiber.Start(this.Checker());
		this.ResetTimer();
	}

	public static UserSettingsBackupManager Instance
	{
		get
		{
			return UserSettingsBackupManager.instance;
		}
	}

	public bool BackupEnabled
	{
		get
		{
			return this.Config.Enabled && PuzzleGame.PlayerState.FarthestUnlockedLevelHumanNumber >= this.Config.LevelRequired;
		}
	}

	private UserSettingsBackupManager.Configuration Config
	{
		get
		{
			return this.provider.GetConfiguration();
		}
	}

	public static UserSettingsBackupManager CreateInstance(UserSettingsBackupManager.IUserSettingsBackupManagerProvider provider, ICloudInterfaceBase cloudInterfaceBase, TimeStampManager timeStampManager)
	{
		UserSettingsBackupManager.instance = new UserSettingsBackupManager(provider, cloudInterfaceBase, timeStampManager);
		return UserSettingsBackupManager.instance;
	}

	private void ResetTimer()
	{
		this.timerFiber.Start(this.UploadFlagLooper());
	}

	private IEnumerator UploadFlagLooper()
	{
		if (this.Config.BackupInterval <= 0)
		{
			yield break;
		}
		for (;;)
		{
			yield return FiberHelper.Wait((float)this.Config.BackupInterval, (FiberHelper.WaitFlag)0);
			this.needToUpload = true;
		}
		yield break;
	}

	private IEnumerator Checker()
	{
		while (this.Config.Enabled)
		{
			if (this.needToUpload && this.BackupEnabled)
			{
				if (this.requestFiber.IsTerminated)
				{
					this.requestFiber.Start(this.UploadUserSettings(this.provider.GetUserSettingsAsJson()));
				}
			}
			else if (this.needToDownload && this.requestFiber.IsTerminated)
			{
				this.requestFiber.Start(this.DownloadUserSettings());
			}
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		}
		yield break;
		yield break;
	}

	private IEnumerator UploadUserSettings(string userSettingsJson)
	{
		if (!this.BackupEnabled)
		{
			yield break;
		}
		if (this.Config.RetryInterval != -1)
		{
			yield return FiberHelper.Wait((float)this.timeStampManager.GetTimeLeftInSeconds("backupRetry"), (FiberHelper.WaitFlag)0);
		}
		this.ResetTimer();
		this.needToUpload = false;
		Response response = new Response();
		yield return this.cloudInterfaceBase.UploadBackupUserSettings(userSettingsJson, SystemInfoHelper.DeviceID, response);
		if (!response.Success)
		{
			if (this.Config.RetryInterval != -1)
			{
				this.timeStampManager.CreateTimeStamp("backupRetry", this.Config.RetryInterval);
				this.needToUpload = true;
			}
		}
		yield break;
	}

	private IEnumerator DownloadUserSettings()
	{
		if (!this.Config.Enabled)
		{
			yield break;
		}
		if (this.Config.RetryInterval != -1)
		{
			yield return FiberHelper.Wait((float)this.timeStampManager.GetTimeLeftInSeconds("downloadRetry"), (FiberHelper.WaitFlag)0);
		}
		this.needToDownload = false;
		Response response = new Response();
		yield return this.cloudInterfaceBase.DownloadBackupUserSettings(SystemInfoHelper.DeviceID, response);
		if (response.Success)
		{
			if (response.data != null && response.data.ContainsKey("userSettings"))
			{
				this.provider.RestoreFromUserSettings(response.data["userSettings"] as string);
			}
			else
			{
				this.provider.FailedToRestore(L.Get("Something went wrong, please try again later"));
			}
		}
		else
		{
			if (this.Config.RetryInterval != -1)
			{
				this.timeStampManager.CreateTimeStamp("downloadRetry", this.Config.RetryInterval);
				this.needToDownload = true;
			}
			this.provider.FailedToRestore(response.ErrorInfo);
		}
		yield break;
	}

	public void CreateNewBackup()
	{
		if (!this.BackupEnabled)
		{
			return;
		}
		this.requestFiber.Start(this.UploadUserSettings(this.provider.GetUserSettingsAsJson()));
	}

	public void RestoreBackup()
	{
		if (!this.Config.Enabled)
		{
			return;
		}
		this.requestFiber.Start(this.DownloadUserSettings());
	}

	public IEnumerator Synchronize()
	{
		if (!this.BackupEnabled)
		{
			yield break;
		}
		if (this.provider.GetConfiguration().BackupOnFullSync)
		{
			yield return this.UploadUserSettings(this.provider.GetUserSettingsAsJson());
		}
		yield break;
	}

	private static UserSettingsBackupManager instance;

	private readonly ICloudInterfaceBase cloudInterfaceBase;

	public readonly UserSettingsBackupManager.IUserSettingsBackupManagerProvider provider;

	private readonly TimeStampManager timeStampManager;

	private bool needToUpload;

	private bool needToDownload;

	private readonly Fiber requestFiber = new Fiber();

	private readonly Fiber checkFiber = new Fiber();

	private readonly Fiber timerFiber = new Fiber();

	[ConfigProvider("BackupConfiguration")]
	public class Configuration
	{
		[JsonSerializable("Enabled", null)]
		public bool Enabled { get; set; }

		[JsonSerializable("BackupOnFullSync", null)]
		public bool BackupOnFullSync { get; set; }

		[JsonSerializable("BackupInterval", null)]
		public int BackupInterval { get; set; }

		[JsonSerializable("RetryInterval", null)]
		public int RetryInterval { get; set; }

		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }
	}

	public interface IUserSettingsBackupManagerProvider
	{
		UserSettingsBackupManager.Configuration GetConfiguration();

		string GetUserSettingsAsJson();

		void RestoreFromUserSettings(string userSettingsJson);

		void FailedToRestore(string message);
	}
}
