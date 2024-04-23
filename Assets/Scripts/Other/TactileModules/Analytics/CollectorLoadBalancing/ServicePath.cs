using System;

namespace TactileModules.Analytics.CollectorLoadBalancing
{
	public class ServicePath
	{
		public ServicePath(string path, string appId)
		{
			this.path = path;
			this.appId = appId;
		}

		public string GetPath()
		{
			return this.path + this.appId;
		}

		private readonly string path;

		private readonly string appId;
	}
}
