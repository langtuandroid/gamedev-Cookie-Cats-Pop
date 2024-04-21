using System;
using TactileModules.NinjaUi.SharedViewControllers;

namespace CookieCatsPop.FrameworkImplementation.NinjaUi
{
	public class SpinnerViewController : ISpinnerViewController
	{
		public SpinnerViewController(IUIViewManager uiViewManager)
		{
			this.uiViewManager = uiViewManager;
		}

		public IUIView ShowSpinnerView(string message)
		{
			return this.uiViewManager.ShowView<ProgressView>(new object[]
			{
				message
			}).View;
		}

		public void CloseSpinnerView(IUIView uiView)
		{
			uiView.Close(0);
		}

		private readonly IUIViewManager uiViewManager;
	}
}
