using System;

namespace TactileModules.AgeInfo
{
	public class AgeInfoAnalytics
	{
		[TactileAnalytics.EventAttribute("userConsent", true)]
		public class UserConsentEvent : BasicEvent
		{
			public UserConsentEvent(string viewName, int age)
			{
				this.ViewName = viewName;
				this.Age = age;
			}

			private TactileAnalytics.RequiredParam<string> ViewName { get; set; }

			private TactileAnalytics.RequiredParam<int> Age { get; set; }
		}
	}
}
