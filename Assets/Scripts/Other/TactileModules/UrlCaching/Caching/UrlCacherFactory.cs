using System;
using System.Diagnostics;
using TactileModules.Analytics.Interfaces;
using TactileModules.UrlCaching.Analytics;
using TactileModules.UrlCaching.Support;

namespace TactileModules.UrlCaching.Caching
{
	public class UrlCacherFactory : IUrlCacherFactory
	{
		public UrlCacherFactory(IAnalytics analytics)
		{
			this.analytics = analytics;
			this.Initialize();
		}

		public UrlCacherFactory(IAnalytics analytics, string domain)
		{
			this.analytics = analytics;
			this.domain = domain;
			this.Initialize();
		}

		private void Initialize()
		{
			this.fileSystem = new FileSystem();
			this.wwwFactory = new WWWFactory();
		}

		public IUrlCacher Create(string domain)
		{
			AnalyticsReporter analyticsReporter = new AnalyticsReporter(this.analytics, domain);
			return new UrlCacher(analyticsReporter, this.fileSystem, this.wwwFactory, domain);
		}

		public IUrlCacher Create()
		{
			if (!string.IsNullOrEmpty(this.domain))
			{
				AnalyticsReporter analyticsReporter = new AnalyticsReporter(this.analytics, this.domain);
				return new UrlCacher(analyticsReporter, this.fileSystem, this.wwwFactory, this.domain);
			}
			ClientErrorEvent eventObject = new ClientErrorEvent("UrlCacherFactoryNoDomain", new StackTrace(false).ToString(), null, null, null, null, null, null, null);
			this.analytics.LogEvent(eventObject, -1.0, null);
			return null;
		}

		private readonly IAnalytics analytics;

		private readonly string domain;

		private IFileSystem fileSystem;

		private WWWFactory wwwFactory;
	}
}
