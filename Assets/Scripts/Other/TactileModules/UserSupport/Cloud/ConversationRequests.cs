using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.UserSupport.Cloud.Requests;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.Cloud
{
	public class ConversationRequests : IConversationRequests
	{
		public ConversationRequests(ICloudRequestFactory requestFactory, IConversations conversations, IUser user)
		{
			this.requestFactory = requestFactory;
			this.conversations = conversations;
			this.user = user;
			this.SetEventHandlers();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<CloudRequestsSequence> RequestsFailed;



		private void SetEventHandlers()
		{
			this.conversations.AdsOffClaimed += this.ConversationsOnAdsOffClaimed;
		}

		private void ConversationsOnAdsOffClaimed(Message message)
		{
			FiberCtrl.Pool.Run(this.ClaimAdsOff(message), false);
		}

		public IEnumerator LoadMessages()
		{
			GetAllCloudRequest getMessagesRequest = this.requestFactory.Create<GetAllCloudRequest>();
			CloudRequestsSequence loadRequests = new CloudRequestsSequence(new ICloudRequest[]
			{
				getMessagesRequest
			});
			yield return this.ProcessRequests(loadRequests);
			yield break;
		}

		public IEnumerator SubmitMessage(string message)
		{
			SubmitCloudRequest submitRequest = this.requestFactory.Create<SubmitCloudRequest>();
			submitRequest.MessageData = new SubmitMessageData
			{
				Message = message,
				Name = this.user.Name,
				Email = this.user.Email
			};
			GetAllCloudRequest getMessagesRequest = this.requestFactory.Create<GetAllCloudRequest>();
			CloudRequestsSequence submitRequests = new CloudRequestsSequence(new ICloudRequest[]
			{
				submitRequest,
				getMessagesRequest
			});
			yield return this.ProcessRequests(submitRequests);
			yield break;
		}

		public IEnumerator SetRead()
		{
			SetReadCloudRequest setReadRequest = this.requestFactory.Create<SetReadCloudRequest>();
			CheckUnreadCloudRequest checkUnreadRequest = this.requestFactory.Create<CheckUnreadCloudRequest>();
			CloudRequestsSequence markReadRequests = new CloudRequestsSequence(new ICloudRequest[]
			{
				setReadRequest,
				checkUnreadRequest
			});
			yield return this.ProcessRequests(markReadRequests);
			yield break;
		}

		public IEnumerator Claim()
		{
			ClaimCloudRequest claimRequest = this.requestFactory.Create<ClaimCloudRequest>();
			GetAllCloudRequest refreshMessagesRequest = this.requestFactory.Create<GetAllCloudRequest>();
			CloudRequestsSequence claimRequests = new CloudRequestsSequence(new ICloudRequest[]
			{
				claimRequest,
				refreshMessagesRequest
			});
			yield return this.ProcessRequests(claimRequests);
			yield break;
		}

		public IEnumerator BackupDismissed(Message message)
		{
			DismissBackupCloudRequest dismissRequest = this.requestFactory.Create<DismissBackupCloudRequest>();
			dismissRequest.Message = message;
			GetAllCloudRequest refreshMessagesRequest = this.requestFactory.Create<GetAllCloudRequest>();
			CloudRequestsSequence requests = new CloudRequestsSequence(new ICloudRequest[]
			{
				dismissRequest,
				refreshMessagesRequest
			});
			yield return this.ProcessRequests(requests);
			yield break;
		}

		public IEnumerator BackupApplied(Message message)
		{
			ApplyBackupCloudRequest applyRequest = this.requestFactory.Create<ApplyBackupCloudRequest>();
			applyRequest.Message = message;
			GetAllCloudRequest refreshMessagesRequest = this.requestFactory.Create<GetAllCloudRequest>();
			CloudRequestsSequence requests = new CloudRequestsSequence(new ICloudRequest[]
			{
				applyRequest,
				refreshMessagesRequest
			});
			yield return this.ProcessRequests(requests);
			yield break;
		}

		public IEnumerator ClaimAdsOff(Message message)
		{
			ClaimMessageAttachmentsSilentCloudRequest claimAdsOffRequest = this.requestFactory.Create<ClaimMessageAttachmentsSilentCloudRequest>();
			claimAdsOffRequest.Message = message;
			claimAdsOffRequest.AttachmentNames = new List<string>
			{
				"AdsOff"
			};
			CloudRequestsSequence requests = new CloudRequestsSequence(new ICloudRequest[]
			{
				claimAdsOffRequest
			});
			yield return this.ProcessRequests(requests);
			yield break;
		}

		private IEnumerator ProcessRequests(CloudRequestsSequence requests)
		{
			yield return requests.Execute();
			if (!requests.Success)
			{
				this.RequestsFailed(requests);
			}
			yield break;
		}

		private readonly ICloudRequestFactory requestFactory;

		private readonly IConversations conversations;

		private readonly IUser user;
	}
}
