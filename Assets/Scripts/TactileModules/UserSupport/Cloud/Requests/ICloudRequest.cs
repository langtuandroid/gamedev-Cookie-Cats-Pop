using System;
using System.Collections;
using Cloud;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public interface ICloudRequest
	{
		Response GetResponse();

		IEnumerator Execute();

		bool IsValid(Response r);
	}
}
