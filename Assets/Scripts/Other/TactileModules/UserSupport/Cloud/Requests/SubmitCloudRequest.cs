using System;
using System.Collections;
using Cloud;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class SubmitCloudRequest : CloudRequest
	{
		public SubmitCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public SubmitMessageData MessageData { get; set; }

		public override IEnumerator Execute()
		{
			this.response = new Response();
			yield return this.SubmitMessage();
			yield break;
		}

		protected virtual IEnumerator SubmitMessage()
		{
			yield return this.cloud.Submit(this.MessageData.Message, this.MessageData.Email, this.MessageData.Name, this.response, "support_message");
			yield break;
		}
	}
}
