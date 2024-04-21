using System;

namespace TactileModules.UrlCaching.Analytics.Events
{
	[TactileAnalytics.EventAttribute("urlCachingError", true)]
	public class UrlCachingErrorEvent : EventBase
	{
		public UrlCachingErrorEvent(string url, string domain, string errorName, string errorMessage, Exception exception) : base(url, domain)
		{
			this.errorName = errorName;
			this.errorMessage = errorMessage;
			if (exception != null)
			{
				this.exception = exception.ToString();
			}
		}

		private TactileAnalytics.RequiredParam<string> errorName { get; set; }

		private TactileAnalytics.RequiredParam<string> errorMessage { get; set; }

		private TactileAnalytics.OptionalParam<string> exception { get; set; }
	}
}
