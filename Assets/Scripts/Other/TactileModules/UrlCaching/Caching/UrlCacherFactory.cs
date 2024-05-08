using System;
using System.Diagnostics;
using TactileModules.Analytics.Interfaces;
using TactileModules.UrlCaching.Analytics;
using TactileModules.UrlCaching.Support;

namespace TactileModules.UrlCaching.Caching
{
	public class UrlCacherFactory : IUrlCacherFactory
	{
		public UrlCacherFactory()
		{
			
			this.Initialize();
		}

		public UrlCacherFactory(string domain)
		{
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
			
			return new UrlCacher(this.fileSystem, this.wwwFactory, domain);
		}

		public IUrlCacher Create()
		{
			if (!string.IsNullOrEmpty(this.domain))
			{
				return new UrlCacher(this.fileSystem, this.wwwFactory, this.domain);
			}
			ClientErrorEvent eventObject = new ClientErrorEvent("UrlCacherFactoryNoDomain", new StackTrace(false).ToString(), null, null, null, null, null, null, null);
			return null;
		}

		private readonly string domain;

		private IFileSystem fileSystem;

		private WWWFactory wwwFactory;
	}
}
