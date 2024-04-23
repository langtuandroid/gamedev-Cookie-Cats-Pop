using System;
using System.Collections;
using Fibers;

namespace TactileModules.NinjaUi.SharedViewControllers
{
	public interface IBasicDialogViewController
	{
		IUIView ShowBasicDialogView(string header, string message, string button0, string button1);

		IEnumerator WaitForClosingResult(IUIView uiView, EnumeratorResult<object> result);

		void CloseBasicDialogView(IUIView uiView);
	}
}
