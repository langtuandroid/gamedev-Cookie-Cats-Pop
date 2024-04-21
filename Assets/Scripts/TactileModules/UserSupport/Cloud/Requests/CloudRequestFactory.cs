using System;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class CloudRequestFactory : ICloudRequestFactory
	{
		public CloudRequestFactory(IConversations conversations, IUserSupportCloud cloud)
		{
			this.conversations = conversations;
			this.cloud = cloud;
		}

		public T Create<T>() where T : ICloudRequest
		{
			return (T)((object)Activator.CreateInstance(typeof(T), new object[]
			{
				this.conversations,
				this.cloud
			}));
		}

		private IConversations conversations;

		private IUserSupportCloud cloud;
	}
}
