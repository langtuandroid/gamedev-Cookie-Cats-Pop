using System;
using Cloud;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class RequestLog
	{
		public ICloudRequest Request { get; set; }

		public Response Response { get; set; }
	}
}
