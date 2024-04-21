using System;

namespace TactileModules.UserSupport.View
{
	public interface IClaimView
	{
		event Action Closed;

		event Action Claimed;

		void Show();

		void Dismiss(UIEvent e);
	}
}
