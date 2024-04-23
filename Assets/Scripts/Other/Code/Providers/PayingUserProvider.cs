using System;
using Tactile;
using TactileModules.Ads;
using TactileModules.UserSettings;

namespace Code.Providers
{
	public class PayingUserProvider : IPayingUserProvider
	{
		public PayingUserProvider(IUserSettingsGetter<InAppPurchaseManager.PersistableState> userSettingsGetter)
		{
			this.userSettingsGetter = userSettingsGetter;
		}

		public bool IsPayingUser()
		{
			return this.userSettingsGetter.Get().IsPayingUser;
		}

		private readonly IUserSettingsGetter<InAppPurchaseManager.PersistableState> userSettingsGetter;
	}
}
