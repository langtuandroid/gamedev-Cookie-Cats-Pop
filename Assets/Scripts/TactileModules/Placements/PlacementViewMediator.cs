using System;

namespace TactileModules.Placements
{
	public class PlacementViewMediator : IPlacementViewMediator, IViewPresenter
	{
		public PlacementViewMediator(IUIViewManager uiViewManager)
		{
			this.uiViewManager = uiViewManager;
		}

		public UIViewManager.IUIViewStateGeneric<T> ShowViewFromPrefab<T>(T viewPrefab, params object[] initialParameters) where T : UIView
		{
			this.IncrementViewShownCount();
			return this.uiViewManager.ShowViewFromPrefab<T>(viewPrefab, initialParameters);
		}

		public UIViewManager.IUIViewStateGeneric<T> ShowViewInstance<T>(T view, params object[] initialParameters) where T : IUIView
		{
			this.IncrementViewShownCount();
			return this.uiViewManager.ShowViewInstance<T>(view, initialParameters);
		}

		public int GetViewShownCount()
		{
			return this.viewShownCount;
		}

		public void ResetViewShownCount()
		{
			this.viewShownCount = 0;
		}

		private void IncrementViewShownCount()
		{
			this.viewShownCount++;
		}

		private readonly IUIViewManager uiViewManager;

		private int viewShownCount;
	}
}
