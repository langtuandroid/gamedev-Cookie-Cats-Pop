using System;

namespace TactileModules.GameCore.UI
{
	public class ScopedView<T> : IDisposable where T : UIView
	{
		public ScopedView(IUIController uiController, T viewPrefab)
		{
			this.uiController = uiController;
			this.viewPrefab = viewPrefab;
		}

		public T CreateView()
		{
			this.viewInstance = this.uiController.ShowView<T>(this.viewPrefab);
			return this.viewInstance;
		}

		public void Dispose()
		{
			if (this.viewInstance == null)
			{
				return;
			}
			if (this.viewInstance.IsClosing)
			{
				return;
			}
			this.viewInstance.Close(0);
		}

		private readonly IUIController uiController;

		private readonly T viewPrefab;

		private T viewInstance;
	}
}
