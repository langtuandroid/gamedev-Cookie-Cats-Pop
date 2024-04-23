using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport
{
	public class Conversations : IConversations
	{
		public Conversations(IAttachmentsListener attachmentsListener, ISupportedAttachments supportedAttachments)
		{
			this.attachmentsListener = attachmentsListener;
			this.supportedAttachments = supportedAttachments;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Loaded;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> NewMessagesAvailable;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action AttachmentsClaimed;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Message> AdsOffClaimed;



		public IConversation Conversation
		{
			get
			{
				return this.GetConversation();
			}
		}

		public void Set(List<Conversation> conversations)
		{
			this.conversations = conversations;
			this.numberOfNewMessagesAvailableOnServer = this.GetUnreadMessages().Count;
			this.ClaimAdsOffAttachment();
			this.FilterAttachments();
			this.Loaded();
		}

		private void FilterAttachments()
		{
			Conversation conversation = this.GetConversation();
			foreach (Message message in conversation.Messages)
			{
				List<Attachment> list = new List<Attachment>();
				foreach (Attachment attachment in message.Attachments)
				{
					if (this.IsAttachmentSupported(attachment) && this.IsAttachmentRenderable(attachment))
					{
						list.Add(attachment);
					}
				}
				message.Attachments = list;
			}
		}

		public bool IsAttachmentSupported(Attachment attachment)
		{
			return this.supportedAttachments.GetSupportedAttachments().Contains(attachment.Name);
		}

		public bool IsAttachmentRenderable(Attachment attachment)
		{
			return !Attachment.GetNonRenderableNames().Contains(attachment.Name);
		}

		public int GetNumberOfUnreadMessagesAvailableOnServer()
		{
			return this.numberOfNewMessagesAvailableOnServer;
		}

		public void SetNewMessagesAvailable(int numMessages)
		{
			this.numberOfNewMessagesAvailableOnServer = numMessages;
			this.NewMessagesAvailable(this.numberOfNewMessagesAvailableOnServer);
		}

		public void NotifyClaimed(List<ItemAmount> attachments)
		{
			this.attachmentsListener.AttachmentClaimed(attachments, delegate
			{
				this.AttachmentsClaimed();
			});
		}

		public List<Conversation> GetEntries()
		{
			return this.conversations;
		}

		public List<Message> GetUnreadMessages()
		{
			List<Message> list = new List<Message>();
			Conversation conversation = this.GetConversation();
			foreach (Message message in conversation.Messages)
			{
				if (!message.IsRead)
				{
					list.Add(message);
				}
			}
			return list;
		}

		public bool HasPendingBackup()
		{
			return this.GetPendingBackup() != null;
		}

		public Message GetPendingBackup()
		{
			Conversation conversation = this.GetConversation();
			List<Message> messages = conversation.Messages;
			for (int i = messages.Count - 1; i >= 0; i--)
			{
				Message message = messages[i];
				if (message.HasPendingBackup())
				{
					return message;
				}
			}
			return null;
		}

		private Conversation GetConversation()
		{
			return (this.conversations.Count <= 0) ? new Conversation() : this.conversations[0];
		}

		private void ClaimAdsOffAttachment()
		{
			Conversation conversation = this.GetConversation();
			foreach (Message message in conversation.Messages)
			{
				if (message.HasUnclaimedAdsOff())
				{
					this.attachmentsListener.AdsOffClaimed();
					this.AdsOffClaimed(message);
				}
			}
		}

		private readonly IAttachmentsListener attachmentsListener;

		private readonly ISupportedAttachments supportedAttachments;

		private List<Conversation> conversations = new List<Conversation>();

		private int numberOfNewMessagesAvailableOnServer;
	}
}
