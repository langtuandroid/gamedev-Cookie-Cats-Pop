using System;

namespace TactileModules.UserSupport.ViewControllers
{
	public interface IUIViewController
	{
		event Action ViewClosed;

		void ShowView();

		bool IsShowing { get; }
	}
}
