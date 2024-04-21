using System;
using System.Collections;
using Cloud;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class CheckUnreadCloudRequest : CloudRequest
	{
		public CheckUnreadCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public override IEnumerator Execute()
		{
			yield return this.GetUnreadCount();
			yield break;
		}

		private IEnumerator GetUnreadCount()
		{
			this.response = new Response();
			yield return this.cloud.CheckUnread(this.response);
			if (this.IsValid(this.response))
			{
				object obj = this.response.data["unread"];
				this.conversations.SetNewMessagesAvailable((int)((double)obj));
			}
			yield break;
		}
	}
}
