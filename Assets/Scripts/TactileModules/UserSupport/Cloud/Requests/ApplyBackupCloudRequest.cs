using System;
using System.Collections;
using Cloud;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class ApplyBackupCloudRequest : CloudRequest
	{
		public ApplyBackupCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public Message Message { get; set; }

		public override IEnumerator Execute()
		{
			this.response = new Response();
			yield return this.cloud.AppliedBackup(this.Message.Id, this.response);
			yield break;
		}
	}
}
