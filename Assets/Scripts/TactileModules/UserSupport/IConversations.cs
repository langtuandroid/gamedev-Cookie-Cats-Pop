using System;
using System.Collections.Generic;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport
{
	public interface IConversations
	{
		event Action Loaded;

		event Action<int> NewMessagesAvailable;

		event Action<Message> AdsOffClaimed;

		IConversation Conversation { get; }

		void Set(List<Conversation> conversations);

		int GetNumberOfUnreadMessagesAvailableOnServer();

		void SetNewMessagesAvailable(int numMessages);

		void NotifyClaimed(List<ItemAmount> attachments);

		List<Conversation> GetEntries();

		List<Message> GetUnreadMessages();

		bool IsAttachmentSupported(Attachment attachment);

		bool IsAttachmentRenderable(Attachment attachment);

		bool HasPendingBackup();

		Message GetPendingBackup();
	}
}
