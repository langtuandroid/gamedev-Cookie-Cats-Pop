using System;

namespace TactileModules.NinjaUi.SharedViewControllers
{
	public interface ISpinnerViewController
	{
		IUIView ShowSpinnerView(string message);

		void CloseSpinnerView(IUIView uiView);
	}
}
