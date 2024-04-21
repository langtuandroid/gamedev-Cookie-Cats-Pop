using System;
using System.Collections;
using Fibers;
using TactileModules.NinjaUi.SharedViewControllers;

namespace CookieCatsPop.FrameworkImplementation.NinjaUi
{
	public class BasicDialogViewController : IBasicDialogViewController
	{
		public BasicDialogViewController(IUIViewManager uiViewManager)
		{
			this.uiViewManager = uiViewManager;
		}

		public IUIView ShowBasicDialogView(string header, string message, string button0, string button1)
		{
			return this.uiViewManager.ShowView<MessageBoxView>(new object[]
			{
				header,
				message,
				button0,
				button1
			}).View;
		}

		public IEnumerator WaitForClosingResult(IUIView uiView, EnumeratorResult<object> result)
		{
			while (!uiView.IsClosing)
			{
				yield return null;
			}
			result.value = uiView.ClosingResult;
			yield break;
		}

		public void CloseBasicDialogView(IUIView uiView)
		{
			uiView.Close(0);
		}

		private readonly IUIViewManager uiViewManager;
	}
}
