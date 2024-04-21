using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class ClaimMessageAttachmentsSilentCloudRequest : CloudRequest
	{
		public ClaimMessageAttachmentsSilentCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public Message Message { private get; set; }

		public List<string> AttachmentNames { private get; set; }

		public override IEnumerator Execute()
		{
			yield return this.Claim();
			yield break;
		}

		private IEnumerator Claim()
		{
			this.response = new Response();
			yield return this.cloud.ClaimAttachments(this.Message.Id, this.AttachmentNames, this.response);
			yield break;
		}
	}
}
