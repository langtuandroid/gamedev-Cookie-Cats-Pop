using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace TactileModules.UserSupport.View
{
	public class AvailableMessagesPopupView : UIView, IAvailableMessagesPopupView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ConversationSelected;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClosedSelected;



		void IAvailableMessagesPopupView.Close(int result)
		{
			base.Close(result);
		}

		[UsedImplicitly]
		private void GotoMessagesButtonClicked(UIEvent e)
		{
			this.ConversationSelected();
		}

		[UsedImplicitly]
		private void Close(UIEvent e)
		{
			this.ClosedSelected();
		}
	}
}
