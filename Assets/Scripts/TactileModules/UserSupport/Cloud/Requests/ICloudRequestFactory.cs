using System;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public interface ICloudRequestFactory
	{
		T Create<T>() where T : ICloudRequest;
	}
}
