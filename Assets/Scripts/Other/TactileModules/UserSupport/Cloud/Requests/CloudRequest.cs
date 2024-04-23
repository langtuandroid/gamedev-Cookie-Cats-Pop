using System;
using System.Collections;
using Cloud;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public abstract class CloudRequest : ICloudRequest
	{
		public CloudRequest()
		{
		}

		public CloudRequest(IConversations conversations, IUserSupportCloud cloud)
		{
			this.conversations = conversations;
			this.cloud = cloud;
		}

		public abstract IEnumerator Execute();

		public Response GetResponse()
		{
			return this.response;
		}

		public virtual bool IsValid(Response r)
		{
			return r.Success;
		}

		protected IUserSupportCloud cloud;

		protected IConversations conversations;

		protected Response response;
	}
}
