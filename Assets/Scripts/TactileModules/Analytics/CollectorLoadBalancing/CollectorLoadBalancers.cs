using System;
using System.Collections.Generic;

namespace TactileModules.Analytics.CollectorLoadBalancing
{
	public class CollectorLoadBalancers
	{
		public void Add(CollectorLoadBalancer loadBalancer)
		{
			this.collectors.Add(loadBalancer);
		}

		public void Reset()
		{
			foreach (CollectorLoadBalancer collectorLoadBalancer in this.collectors)
			{
				collectorLoadBalancer.ResetActiveCollector();
			}
		}

		private readonly List<CollectorLoadBalancer> collectors = new List<CollectorLoadBalancer>();
	}
}
