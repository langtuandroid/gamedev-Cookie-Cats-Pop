using System;
using TactileModules.Foundation.CloudSynchronization;
using TactileModules.UserSupport.Cloud.Requests;

namespace TactileModules.UserSupport
{
	public class UserSupportSystem
	{
		public UserSupportSystem(IUserSupportViewControllerFactory controllerFactory, IConversations conversations, ICloudRequestFactory requestFactory, IUser user, ICloudSynchronizable cloudSynchronizable)
		{
			this.ControllerFactory = controllerFactory;
			this.Conversations = conversations;
			this.CloudRequestFactory = requestFactory;
			this.User = user;
			this.Synchronizer = cloudSynchronizable;
		}

		public IUserSupportViewControllerFactory ControllerFactory { get; private set; }

		public IConversations Conversations { get; private set; }

		public ICloudRequestFactory CloudRequestFactory { get; private set; }

		public IUser User { get; private set; }

		public ICloudSynchronizable Synchronizer { get; private set; }

		private readonly IUserSupportViewControllerFactory controllerFactory;

		private readonly IConversations conversations;

		private readonly ICloudRequestFactory requestFactory;

		private readonly IUser user;

		private readonly ICloudSynchronizable cloudSynchronizable;
	}
}
