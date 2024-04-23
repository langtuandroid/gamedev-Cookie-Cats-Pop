using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.UserSupport.DataProviders;

namespace TactileModules.UserSupport.Cloud
{
	public class UserSupportCloud : IUserSupportCloud
	{
		public UserSupportCloud(CloudClientBase cloudClient, IMessageMetaData messageMetaDataProvider)
		{
			this.messageMetaDataProvider = messageMetaDataProvider;
			this.cloudClient = cloudClient;
		}

		public IEnumerator Submit(string message, string email, string name, Response response, string messageContext = "support_message")
		{
			string userId = this.GetUserId();
			Hashtable metaData = this.messageMetaDataProvider.CreateMetaData();
			ICloudInterfaceBase cloudInterface = this.cloudClient.cloudInterface;
			string userId2 = userId;
			yield return cloudInterface.UserSupportSubmitMessage(userId2, message, email, name, metaData, response, messageContext, null);
			yield break;
		}

		public virtual IEnumerator GetAll(Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportGetMessages(userId, response);
			yield break;
		}

		public virtual IEnumerator GetArticles(Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportGetArticles(userId, response);
			yield break;
		}

		public virtual IEnumerator SetRead(Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportSetRead(userId, response);
			yield break;
		}

		public virtual IEnumerator ClaimAllAttachments(Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportClaimAttachments(userId, response);
			yield break;
		}

		public IEnumerator ClaimAttachments(string messageId, List<string> attachmentNames, Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportClaimMessageAttachments(userId, messageId, attachmentNames.ToArray(), response);
			yield break;
		}

		public virtual IEnumerator CheckUnread(Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportCheckUnread(userId, response);
			yield break;
		}

		public IEnumerator DismissedBackup(string messageId, Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportDismissedBackup(messageId, userId, response);
			yield break;
		}

		public IEnumerator AppliedBackup(string messageId, Response response)
		{
			string userId = this.GetUserId();
			yield return this.cloudClient.cloudInterface.UserSupportAppliedBackup(messageId, userId, response);
			yield break;
		}

		private string GetUserId()
		{
			if (this.cloudClient.CachedMe == null)
			{
				return string.Empty;
			}
			return this.cloudClient.CachedMe.CloudId;
		}

		private readonly IMessageMetaData messageMetaDataProvider;

		private readonly CloudClientBase cloudClient;
	}
}
