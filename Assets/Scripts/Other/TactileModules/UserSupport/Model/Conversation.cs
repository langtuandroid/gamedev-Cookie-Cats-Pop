using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace TactileModules.UserSupport.Model
{
	public class Conversation : IConversation
	{
		public Conversation()
		{
			this.Messages = new List<Message>();
		}

		[JsonSerializable("messages", typeof(Message))]
		public List<Message> Messages { get; [UsedImplicitly] set; }

		public List<Message> UnreadMessages
		{
			get
			{
				if (this.unreadMessages == null)
				{
					this.unreadMessages = this.GetUnreadMessages();
				}
				return this.unreadMessages;
			}
			private set
			{
				this.unreadMessages = value;
			}
		}

		public List<Message> GetMessagesFromUser()
		{
			List<Message> list = new List<Message>();
			foreach (Message message in this.Messages)
			{
				if (message.Sender.Equals("user"))
				{
					list.Add(message);
				}
			}
			return list;
		}

		private List<Message> GetUnreadMessages()
		{
			List<Message> list = new List<Message>();
			foreach (Message message in this.Messages)
			{
				if (!message.IsRead)
				{
					list.Add(message);
				}
			}
			return list;
		}

		public List<Attachment> GetUnclaimedAttachments()
		{
			List<Attachment> list = new List<Attachment>();
			foreach (Message message in this.Messages)
			{
				List<Attachment> unclaimedAttachments = message.GetUnclaimedAttachments();
				list.AddRange(unclaimedAttachments);
			}
			return list;
		}

		private List<Message> unreadMessages;
	}
}
