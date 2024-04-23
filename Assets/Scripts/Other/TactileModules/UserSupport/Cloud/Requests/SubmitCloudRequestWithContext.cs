using System;
using System.Collections;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class SubmitCloudRequestWithContext : SubmitCloudRequest
	{
		public SubmitCloudRequestWithContext(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		protected override IEnumerator SubmitMessage()
		{
			yield return this.cloud.Submit(base.MessageData.Message, base.MessageData.Email, base.MessageData.Name, this.response, base.MessageData.Context);
			yield break;
		}
	}
}
