using System;
using System.Collections;
using Cloud;
using TactileModules.TactileCloud;

namespace TactileModules.AbTesting.Cloud
{
	public interface ICloudRequest<T>
	{
		IEnumerator Execute();

		ICloudResponse GetResponse();

		T GetResult();

		bool IsValid(Response r);
	}
}
