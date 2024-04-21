using System;

namespace TactileModules.Analytics.CollectorLoadBalancing
{
	public interface ICollectorLoadBalancer
	{
		string ActiveCollector();

		void ResetActiveCollector();

		void FailoverToNextCollector();
	}
}
