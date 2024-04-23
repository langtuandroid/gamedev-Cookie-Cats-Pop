using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport
{
	public class NullConversations : IConversations
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Loaded;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> NewMessagesAvailable;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Message> AdsOffClaimed;

		public IConversation Conversation
		{
			get
			{
				return new Conversation();
			}
		}

		public void Set(List<Conversation> conversations)
		{
		}

		public int GetNumberOfUnreadMessagesAvailableOnServer()
		{
			return 0;
		}

		public void SetNewMessagesAvailable(int numMessages)
		{
		}

		public void NotifyClaimed(List<ItemAmount> attachments)
		{
		}

		public List<Conversation> GetEntries()
		{
			return new List<Conversation>();
		}

		public List<Message> GetUnreadMessages()
		{
			return new List<Message>();
		}

		public bool IsAttachmentSupported(Attachment attachment)
		{
			return false;
		}

		public bool IsAttachmentRenderable(Attachment attachment)
		{
			return false;
		}

		public bool HasPendingBackup()
		{
			return false;
		}

		public Message GetPendingBackup()
		{
			return null;
		}
	}
}
