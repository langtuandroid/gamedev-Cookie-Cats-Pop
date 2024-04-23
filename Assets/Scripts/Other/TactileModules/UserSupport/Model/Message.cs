using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace TactileModules.UserSupport.Model
{
	public class Message
	{
		public Message()
		{
			this.Attachments = new List<Attachment>();
		}

		[JsonSerializable("_id", null)]
		public string Id { get; set; }

		[JsonSerializable("ticketId", null)]
		public string TicketId { get; set; }

		[JsonSerializable("createdAt", null)]
		public string CreatedAt { get; set; }

		[JsonSerializable("updatedAt", null)]
		public string UpdatedAt { get; set; }

		[JsonSerializable("subject", null)]
		public string Subject { get; set; }

		[JsonSerializable("bodyTranslated", null)]
		public string BodyTranslated { get; set; }

		public string GetTranslatedMessageBodyOrDefault()
		{
			if (this.HasTranslatedMessageBody())
			{
				return this.BodyTranslated;
			}
			return this.Body;
		}

		public bool HasTranslatedMessageBody()
		{
			return !string.IsNullOrEmpty(this.BodyTranslated);
		}

		[JsonSerializable("read", null)]
		public bool IsRead
		{
			get
			{
				return this.isRead;
			}
			[UsedImplicitly]
			set
			{
				if (this.Sender == "user")
				{
					this.isRead = true;
					return;
				}
				this.isRead = value;
			}
		}

		[JsonSerializable("body", null)]
		public string Body { get; set; }

		[JsonSerializable("sender", null)]
		public string Sender { get; set; }

		[JsonSerializable("senderEmail", null)]
		public string SenderEmail { get; set; }

		[JsonSerializable("senderName", null)]
		public string SenderName { get; set; }

		[JsonSerializable("attachments", typeof(Attachment))]
		public List<Attachment> Attachments { get; set; }

		public bool HasUnclaimedAdsOff()
		{
			foreach (Attachment attachment in this.Attachments)
			{
				if (attachment.Name.Equals("AdsOff") && !attachment.Claimed)
				{
					return true;
				}
			}
			return false;
		}

		public List<Attachment> GetUnclaimedAttachments()
		{
			List<Attachment> list = new List<Attachment>();
			foreach (Attachment attachment in this.Attachments)
			{
				if (!attachment.Claimed)
				{
					list.Add(attachment);
				}
			}
			return list;
		}

		[JsonSerializable("backup", null)]
		public Backup Backup { get; [UsedImplicitly] set; }

		public bool HasPendingBackup()
		{
			return this.Backup != null && this.Backup.State.Equals("pending");
		}

		private bool isRead = true;
	}
}
