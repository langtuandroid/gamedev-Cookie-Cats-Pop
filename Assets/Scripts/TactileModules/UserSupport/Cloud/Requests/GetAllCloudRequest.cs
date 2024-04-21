using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud.Requests
{
	public class GetAllCloudRequest : CloudRequest
	{
		public GetAllCloudRequest(IConversations conversations, IUserSupportCloud cloud) : base(conversations, cloud)
		{
		}

		public override IEnumerator Execute()
		{
			yield return this.Load();
			yield break;
		}

		private IEnumerator Load()
		{
			this.response = new Response();
			yield return this.cloud.GetAll(this.response);
			if (this.IsValid(this.response))
			{
				this.conversations.Set(GetAllCloudRequest.GetParsedResponse(this.response.data));
			}
			yield break;
		}

		public static List<Conversation> GetParsedResponse(Hashtable jsonData)
		{
			ArrayList array = (ArrayList)jsonData["messages"];
			List<Message> messages = JsonSerializer.ArrayListToGenericList<Message>(array);
			Conversation conversation = new Conversation();
			conversation.Messages = messages;
			return new List<Conversation>
			{
				conversation
			};
		}
	}
}
