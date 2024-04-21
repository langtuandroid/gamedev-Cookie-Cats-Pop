using System;

public interface IViewPresenter
{
	UIViewManager.IUIViewStateGeneric<T> ShowViewFromPrefab<T>(T viewPrefab, params object[] initialParameters) where T : UIView;

	UIViewManager.IUIViewStateGeneric<T> ShowViewInstance<T>(T view, params object[] initialParameters) where T : IUIView;
}
