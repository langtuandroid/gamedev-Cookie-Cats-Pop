using System;
using System.Collections;
using Fibers;

namespace TactileModules.GameCore.UI
{
	public interface IUIController
	{
		T ShowView<T>(T viewPrefab) where T : UIView;

		event Action<IUIView> ViewCreated;

		event Action<IUIView> ViewDestroyed;

		event Action<IUIView> ViewWillDisappear;

		IEnumerator ShowAndWaitForView<T>(T viewPrefab, Action<T> viewShown = null, EnumeratorResult<object> result = null) where T : UIView;

		IEnumerator FadeCameraFrontFill(float target, float duration = 0f);

		UICamera GetCameraFromView(IUIView view);

		IUIView TopView { get; }

		UICamera TopLayerCamera { get; }

		bool AnyViewsAnimating { get; }

		ScopedView<T> CreateScopedView<T>(T viewPrefab) where T : UIView;
	}
}
