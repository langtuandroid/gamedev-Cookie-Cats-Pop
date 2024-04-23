using System;
using System.Collections;
using Cloud;
using TactileModules.TactileCloud;

namespace TactileModules.AbTesting.Cloud
{
	public abstract class CloudRequest<T> : ICloudRequest<T>
	{
		public abstract IEnumerator Execute();

		public abstract T GetResult();

		public ICloudResponse GetResponse()
		{
			return this.response;
		}

		public virtual bool IsValid(Response r)
		{
			return r.Success;
		}

		protected ICloudResponse response;
	}
}
