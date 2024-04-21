using System;
using TactileModules.Analytics.Interfaces;

namespace NinjaUI.Analytics
{
	public class UIAnalyticsEventLogger
	{
		public UIAnalyticsEventLogger(IAnalytics analytics, UIViewManager viewManager)
		{
			this.analytics = analytics;
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
			this.analytics.LogEvent(new ViewShownEvent(view.name, viewParams), -1.0, null);
		}

		private void LogViewClosed(IUIView view)
		{
			string viewButton = (!(UIButton.LastActivatedButton == null)) ? UIButton.LastActivatedButton.methodName : "Close";
			string viewParams = view.Parameters.ToConcatenatedString(0);
			this.analytics.LogEvent(new ViewClosedEvent(view.name, viewButton, viewParams), -1.0, null);
		}

		private readonly IAnalytics analytics;

		private readonly UIViewManager viewManager;
	}
}
