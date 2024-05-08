using System;
using TactileModules.Analytics.Interfaces;

namespace NinjaUI.Analytics
{
	public class UIAnalyticsEventLogger
	{
		public UIAnalyticsEventLogger(UIViewManager viewManager)
		{
			this.viewManager = viewManager;
			this.SubscribeToEvents();
		}

		private void SubscribeToEvents()
		{
			this.viewManager.ViewWillAppear += this.OnViewWillAppear;
			this.viewManager.ViewDidDisappear += this.OnViewDidDisappear;
		}

		private void OnViewWillAppear(IUIView view)
		{
			if (!view.DisableViewAnalytics)
			{
				this.LogViewShown(view);
			}
		}

		private void OnViewDidDisappear(IUIView view)
		{
			if (!view.DisableViewAnalytics)
			{
				this.LogViewClosed(view);
			}
		}

		private void LogViewShown(IUIView view)
		{
			string viewParams = view.Parameters.ToConcatenatedString(0);
		}

		private void LogViewClosed(IUIView view)
		{
			string viewButton = (!(UIButton.LastActivatedButton == null)) ? UIButton.LastActivatedButton.methodName : "Close";
			string viewParams = view.Parameters.ToConcatenatedString(0);
		}

		private readonly UIViewManager viewManager;
	}
}
