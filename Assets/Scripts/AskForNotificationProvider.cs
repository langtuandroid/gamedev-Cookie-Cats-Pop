using System;
using Tactile;
using TactileModules.Foundation;

public class AskForNotificationProvider : AskForNotificationManager.IAskForNotificationManagerProvider
{
	AskForNotificationManager.AskForNotificationConfig AskForNotificationManager.IAskForNotificationManagerProvider.Config
	{
		get
		{
			return ConfigurationManager.Get<AskForNotificationManager.AskForNotificationConfig>();
		}
	}

	public OneSignalManager OneSignalManager
	{
		get
		{
			return ManagerRepository.Get<OneSignalManager>();
		}
	}
}
