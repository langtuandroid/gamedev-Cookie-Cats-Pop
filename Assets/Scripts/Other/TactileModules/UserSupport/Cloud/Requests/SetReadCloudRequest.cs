using System;
using System.Collections;
using Cloud;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class SetReadCloudRequest : CloudRequest
	{
		public SetReadCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public override IEnumerator Execute()
		{
			yield return this.Mark();
			yield break;
		}

		private IEnumerator Mark()
		{
			this.response = new Response();
			yield return this.cloud.SetRead(this.response);
			yield break;
		}
	}
}
