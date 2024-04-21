using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class ClaimCloudRequest : CloudRequest
	{
		public ClaimCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public override IEnumerator Execute()
		{
			yield return this.Claim();
			yield break;
		}

		private IEnumerator Claim()
		{
			this.response = new Response();
			yield return this.cloud.ClaimAllAttachments(this.response);
			if (this.IsValid(this.response))
			{
				List<ItemAmount> parsedResponse = this.GetParsedResponse();
				this.conversations.NotifyClaimed(parsedResponse);
			}
			yield break;
		}

		private List<ItemAmount> GetParsedResponse()
		{
			ArrayList array = (ArrayList)this.response.data["claimedAttachments"];
			List<Attachment> list = JsonSerializer.ArrayListToGenericList<Attachment>(array);
			List<ItemAmount> list2 = new List<ItemAmount>();
			foreach (Attachment attachment in list)
			{
				if (this.conversations.IsAttachmentSupported(attachment) && this.conversations.IsAttachmentRenderable(attachment))
				{
					list2.Add(Attachment.ToItemAmount(attachment));
				}
			}
			return list2;
		}
	}
}
