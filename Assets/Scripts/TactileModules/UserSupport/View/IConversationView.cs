using System;
using System.Collections.Generic;
using TactileModules.UserSupport.Model;

namespace TactileModules.UserSupport.View
{
	public interface IConversationView
	{
		event Action Closed;

		event Action<string> SubmitMessage;

		event Action<Message> AttachmentClicked;

		void Init();

		void SetUser(IUser user);

		void RenderMessages(List<Message> conversationMessages);

		void Close(int i);

		void ShowLoadingIndicator();

		void HideLoadingIndicator();

		void HideKeyboard();
	}
}
