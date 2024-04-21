using System;

namespace TactileModules.UserSupport.View
{
	public interface IAvailableMessagesPopupView
	{
		event Action ConversationSelected;

		event Action ClosedSelected;

		void Close(int i);
	}
}
