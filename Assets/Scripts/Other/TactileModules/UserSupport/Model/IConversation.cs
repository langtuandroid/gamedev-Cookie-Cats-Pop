using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace TactileModules.UserSupport.Model
{
	public interface IConversation
	{
		List<Message> Messages { get; [UsedImplicitly] set; }

		List<Message> UnreadMessages { get; }

		List<Message> GetMessagesFromUser();

		List<Attachment> GetUnclaimedAttachments();
	}
}
