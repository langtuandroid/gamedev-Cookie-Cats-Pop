using System;

public interface IUIViewManager : IViewPresenter
{
	UIViewManager.UIViewStateGeneric<T> ShowView<T>(params object[] initialParameters) where T : UIView;

	UIViewManager.UIViewState ShowView(Type viewType, params object[] initialParameters);

	UIViewManager.UIViewState ShowView(string viewName, params object[] initialParameters);

	UICamera FindCameraFromObjectLayer(int objectLayer);
}
