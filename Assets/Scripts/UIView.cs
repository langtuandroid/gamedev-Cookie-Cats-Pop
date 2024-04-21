using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class UIView : MonoBehaviour, IUIView
{
	GameObject IUIView.gameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	Transform IUIView.transform
	{
		get
		{
			return base.transform;
		}
	}

	string IUIView.name
	{
		get
		{
			return base.name;
		}
	}

	public bool IsClosing
	{
		get
		{
			return this.ClosingResult != null;
		}
	}

	public object ClosingResult { get; private set; }

	public Vector2 OriginalSize { get; protected set; }

	public object[] Parameters { get; private set; }

	public int PoolingAmount
	{
		get
		{
			return this.poolingAmount;
		}
		set
		{
			this.poolingAmount = value;
		}
	}

	public bool BlockOtherViews
	{
		get
		{
			return this.blockOtherViews;
		}
		set
		{
			this.blockOtherViews = value;
		}
	}

	public virtual bool ShowBackfillQuad
	{
		get
		{
			return this.BlockOtherViews;
		}
	}

	public bool IgnoreFocus
	{
		get
		{
			return this.ignoreFocus;
		}
		set
		{
			this.ignoreFocus = value;
		}
	}

	public bool UseDefaultViewAnimator
	{
		get
		{
			return this.useDefaultViewAnimator;
		}
		set
		{
			this.useDefaultViewAnimator = value;
		}
	}

	public UIViewLayerAnimation OverrideLayerAnimation
	{
		get
		{
			return this.overrideLayerAnimation;
		}
		set
		{
			this.overrideLayerAnimation = value;
		}
	}

	public bool UseBottomViewZone
	{
		get
		{
			return this.useBottomViewZone;
		}
		set
		{
			this.useBottomViewZone = value;
		}
	}

	public bool IgnoreSafeArea
	{
		get
		{
			return this.ignoreSafeArea;
		}
	}

	public Color BackgroundColor
	{
		get
		{
			return this.backgroundColor;
		}
	}

	public bool DisableViewAnalytics
	{
		get
		{
			return this.disableViewAnalytics;
		}
	}

	public UIElement GetElement()
	{
		return base.GetComponent<UIElement>();
	}

	public Vector2 GetElementSize()
	{
		UIElement component = base.GetComponent<UIElement>();
		return (!(component != null)) ? Vector2.zero : component.Size;
	}

	public Action OnClosed { get; set; }

	private void Awake()
	{
		this.OriginalSize = this.GetElementSize();
	}

	public void Close(object result)
	{
		if (this.IsClosing)
		{
			return;
		}
		this.ClosingResult = result;
		if (this.OnClosed != null)
		{
			this.OnClosed();
		}
	}

	public void LoadedFromPool(object[] parameters)
	{
		this.ClosingResult = null;
		this.Parameters = parameters;
		this.ViewLoad(parameters);
	}

	public void ReleasedToPool()
	{
		this.ViewUnload();
	}

	public UIViewLayerAnimation GetViewLayerAnimation()
	{
		if (this.useDefaultViewAnimator)
		{
			return UIProjectSettings.Get().defaultLayerAnimation;
		}
		return this.overrideLayerAnimation;
	}

	public virtual IEnumerator AnimateIn()
	{
		UIViewAnimator[] animators = base.GetComponentsInChildren<UIViewAnimator>();
		if (animators.Length == 0)
		{
			yield break;
		}
		List<IEnumerator> animations = new List<IEnumerator>();
		foreach (UIViewAnimator uiviewAnimator in animators)
		{
			animations.Add(uiviewAnimator.AnimateIn());
		}
		yield return FiberHelper.RunParallel(animations.ToArray());
		yield break;
	}

	public virtual IEnumerator AnimateOut()
	{
		UIViewAnimator[] animators = base.GetComponentsInChildren<UIViewAnimator>();
		if (animators.Length == 0)
		{
			yield break;
		}
		List<IEnumerator> animations = new List<IEnumerator>();
		foreach (UIViewAnimator uiviewAnimator in animators)
		{
			animations.Add(uiviewAnimator.AnimateOut());
		}
		yield return FiberHelper.RunParallel(animations.ToArray());
		yield break;
	}

	public virtual Vector2 CalculateViewSizeForScreen(Vector2 screenSize)
	{
		float num = this.OriginalSize.x / this.OriginalSize.y;
		float num2 = screenSize.Aspect();
		Vector2 result = this.OriginalSize;
		if (this.fillToAspect)
		{
			result = UIUtility.CorrectSizeToAspect(this.OriginalSize, num2, AspectCorrection.Fit);
		}
		else if (!this.useFixedWidth)
		{
			if (num2 > num)
			{
				if (this.maxAjustedWidth > 0f)
				{
					result.x = Mathf.Min(result.y * num2, this.maxAjustedWidth);
					result.y = result.x / num2;
				}
				else
				{
					result.x = result.y * num2;
				}
			}
			else
			{
				result.y = result.x / num2;
			}
		}
		else
		{
			result.y = result.x / num2;
		}
		return result;
	}

	protected virtual void ViewLoad(object[] parameters)
	{
	}

	protected virtual void ViewUnload()
	{
	}

	protected T ObtainOverlay<T>() where T : UIView
	{
		return UIViewManager.Instance.ObtainOverlay<T>(this);
	}

	protected void ReleaseOverlay<T>() where T : UIView
	{
		UIViewManager.Instance.ReleaseOverlay<T>();
	}

	protected Camera ViewCamera
	{
		get
		{
			UIViewLayer viewLayerWithView = UIViewManager.Instance.GetViewLayerWithView(this);
			return (!(viewLayerWithView != null)) ? null : viewLayerWithView.ViewCamera;
		}
	}

	protected virtual void ViewWillAppear()
	{
	}

	protected virtual void ViewDidAppear()
	{
	}

	protected virtual void ViewWillDisappear()
	{
	}

	protected virtual void ViewDidDisappear()
	{
	}

	protected virtual void ViewGotFocus()
	{
	}

	protected virtual void ViewLostFocus()
	{
	}

	protected virtual void ScreenSizeChanged()
	{
	}

	[SerializeField]
	private bool useFixedWidth;

	[SerializeField]
	private bool fillToAspect;

	[SerializeField]
	protected float maxAjustedWidth;

	[SerializeField]
	[HideInInspector]
	private int poolingAmount;

	[SerializeField]
	[HideInInspector]
	private bool blockOtherViews = true;

	[SerializeField]
	[HideInInspector]
	private bool ignoreFocus;

	[SerializeField]
	[HideInInspector]
	private bool useDefaultViewAnimator = true;

	[SerializeField]
	[HideInInspector]
	private UIViewLayerAnimation overrideLayerAnimation;

	[SerializeField]
	private bool useBottomViewZone;

	[SerializeField]
	private bool ignoreSafeArea;

	[SerializeField]
	private Color backgroundColor = Color.clear;

	[SerializeField]
	private bool disableViewAnalytics;
}
