using System;
using System.Collections.Generic;

namespace TactileModules.Analytics.CollectorLoadBalancing
{
	public class CollectorLoadBalancer : ICollectorLoadBalancer
	{
		public CollectorLoadBalancer(string userId, List<string> collectUrls)
		{
			this.collectUrls = collectUrls;
			this.userId = userId;
			this.CalculateCollectUrl();
		}

		public string ActiveCollector()
		{
			return this.collectUrls[this.collectUrlIdx];
		}

		public void ResetActiveCollector()
		{
			this.CalculateCollectUrl();
		}

		public void FailoverToNextCollector()
		{
			this.collectUrlIdx = (this.collectUrlIdx + 1) % this.collectUrls.Count;
		}

		private void CalculateCollectUrl()
		{
			this.collectUrlIdx = Math.Abs(this.userId.GetHashCode()) % this.collectUrls.Count;
		}

		private string userId;

		private List<string> collectUrls;

		private int collectUrlIdx;
	}
}
