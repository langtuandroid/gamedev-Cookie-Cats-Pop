using System;
using Tactile;

public class InAppPurchaseManagerCommandHandler : BaseCommandHandler
{

	private static void TogglePayingUser()
	{
		UserSettingsManager.Get<InAppPurchaseManager.PersistableState>().IsPayingUser = !UserSettingsManager.Get<InAppPurchaseManager.PersistableState>().IsPayingUser;
	}
}
