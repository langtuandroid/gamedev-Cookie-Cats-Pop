using System;

namespace TactileModules.UrlCaching.Analytics.Events
{
	public class EventBase
	{
		public EventBase(string url, string domain)
		{
			this.Url = url;
			this.Domain = domain;
		}

		private TactileAnalytics.RequiredParam<string> Url { get; set; }

		private TactileAnalytics.RequiredParam<string> Domain { get; set; }
	}
}
