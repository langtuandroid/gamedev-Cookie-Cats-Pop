using System;

namespace TactileModules.AgeInfo.UI
{
	public class UserConsentView : UIView
	{
		protected override void ViewLoad(object[] parameters)
		{
			if (parameters.Length >= 1)
			{
				this.provider = (parameters[0] as AgeInfoManager.IDataProvider);
			}
		}

		public virtual void OnConfirm(UIEvent e)
		{
			base.Close(0);
		}

		public void OnInformation(UIEvent e)
		{
			AgeInfoConfig config = this.provider.Config;
			if (string.IsNullOrEmpty(config.TermsOfServiceURL))
			{
				URLHelper.OpenURL("https://tactile.dk/legal/tos.html");
			}
			else
			{
				URLHelper.OpenURL(config.TermsOfServiceURL);
			}
		}

		public void OnPrivacy(UIEvent e)
		{
			AgeInfoConfig config = this.provider.Config;
			if (string.IsNullOrEmpty(config.PrivacyPolicyURL))
			{
				URLHelper.OpenURL("https://tactile.dk/legal/privacy.html");
			}
			else
			{
				URLHelper.OpenURL(config.PrivacyPolicyURL);
			}
		}

		public virtual string GetViewName()
		{
			return "UserConsentView";
		}

		protected AgeInfoManager.IDataProvider provider;
	}
}
