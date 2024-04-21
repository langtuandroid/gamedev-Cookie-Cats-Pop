using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using TactileModules.GameCore.UI;
using UnityEngine;

public class UIController : IUIController
{
	public UIController(UIViewManager viewManager)
	{
		this.viewManager = viewManager;
		this.HookViewManagerEvents();
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<IUIView> ViewCreated;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<IUIView> ViewDestroyed;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<IUIView> ViewWillDisappear;

	private void HookViewManagerEvents()
	{
		UIViewManager uiviewManager = this.viewManager;
		uiviewManager.OnViewWillAppear = (Action<IUIView>)Delegate.Combine(uiviewManager.OnViewWillAppear, new Action<IUIView>(this.OnViewLoad));
		UIViewManager uiviewManager2 = this.viewManager;
		uiviewManager2.OnViewWillRelease = (Action<IUIView>)Delegate.Combine(uiviewManager2.OnViewWillRelease, new Action<IUIView>(this.OnViewWillRelease));
	}

	public T ShowView<T>(T viewPrefab) where T : UIView
	{
		return this.viewManager.ShowViewInstance<T>(UnityEngine.Object.Instantiate<T>(viewPrefab), new object[0]).View;
	}

	public IEnumerator ShowAndWaitForView<T>(T viewPrefab, Action<T> viewShown = null, EnumeratorResult<object> result = null) where T : UIView
	{
		UIViewManager.IUIViewStateGeneric<T> state = this.viewManager.ShowViewFromPrefab<T>(viewPrefab, new object[0]);
		if (viewShown != null)
		{
			viewShown(state.View);
		}
		yield return state.WaitForClose();
		if (result != null)
		{
			result.value = state.ClosingResult;
		}
		yield break;
	}

	public IEnumerator FadeCameraFrontFill(float target, float duration = 0f)
	{
		return this.viewManager.FadeCameraFrontFill(target, duration, 0);
	}

	public UICamera GetCameraFromView(IUIView view)
	{
		return this.viewManager.FindCameraFromObjectLayer(view.gameObject.layer);
	}

	public IUIView TopView
	{
		get
		{
			return (UIView)((!(this.viewManager.TopLayer != null)) ? null : this.viewManager.TopLayer.View);
		}
	}

	public UICamera TopLayerCamera
	{
		get
		{
			return this.viewManager.FindTopMostCameraWithViews();
		}
	}

	public bool AnyViewsAnimating
	{
		get
		{
			return this.viewManager.AnyViewsAnimating;
		}
	}

	public ScopedView<T> CreateScopedView<T>(T viewPrefab) where T : UIView
	{
		return new ScopedView<T>(this, viewPrefab);
	}

	private void OnViewLoad(IUIView view)
	{
		if (this.ViewCreated != null)
		{
			this.ViewCreated((UIView)view);
		}
	}

	private void OnViewWillRelease(IUIView view)
	{
		if (this.ViewDestroyed != null)
		{
			this.ViewDestroyed((UIView)view);
		}
	}

	private void OnViewWillDisappear(UIView view)
	{
		if (this.ViewWillDisappear != null)
		{
			this.ViewWillDisappear(view);
		}
	}

	private readonly UIViewManager viewManager;
}
