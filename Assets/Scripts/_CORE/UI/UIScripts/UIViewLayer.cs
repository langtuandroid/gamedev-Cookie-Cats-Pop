using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class UIViewLayer : MonoBehaviour
{
	public Camera ViewCamera { get; private set; }

	public int CameraLayer { get; private set; }

	public UICamera UICamera
	{
		get
		{
			return this.ViewCamera.GetComponent<UICamera>();
		}
	}

	public IUIView View { get; private set; }

	public void Initialize(UIViewManager uiViewManager, IUIView view, int layer, Color cameraClearColor)
	{
		this.viewManager = uiViewManager;
		uiViewManager.OnScreenChanged += this.HandleScreenChanged;
		this.CameraLayer = layer;
		int num = 1 << this.CameraLayer;
		this.ViewCamera = new GameObject("ViewCamera", new Type[]
		{
			typeof(Camera),
			typeof(UICamera)
		}).GetComponent<Camera>();
		this.ViewCamera.fieldOfView = 60f;
		this.ViewCamera.orthographic = true;
		this.ViewCamera.cullingMask = num;
		this.ViewCamera.clearFlags = ((!(cameraClearColor != Color.clear)) ? CameraClearFlags.Depth : CameraClearFlags.Color);
		this.ViewCamera.backgroundColor = cameraClearColor;
		this.ViewCamera.depth = (float)this.CameraLayer;
		this.ViewCamera.transparencySortMode = TransparencySortMode.Orthographic;
		this.ViewCamera.transform.parent = base.transform;
		this.UICamera.eventReceiverMask = num;
		this.UICamera.useMouse = true;
		this.UICamera.useTouch = false;
		UICamera.SortCamerasByDepth();
		if (view.ShowBackfillQuad)
		{
			this.backfillQuad = UIViewManager.CreateBackfillQuad();
			this.backfillQuad.transform.parent = base.transform;
			this.backfillQuad.gameObject.layer = this.CameraLayer;
		}
		this.UICamera.BlockOtherCameras = view.BlockOtherViews;
		this.Prepare(view);
		this.View = view;
		IUIView view2 = this.View;
		view2.OnClosed = (Action)Delegate.Combine(view2.OnClosed, new Action(this.HandleCloseView));
	}

	public void ChangeLayer(int layer)
	{
		this.CameraLayer = layer;
		int num = 1 << this.CameraLayer;
		this.ViewCamera.cullingMask = num;
		this.UICamera.eventReceiverMask = num;
		this.ViewCamera.depth = (float)this.CameraLayer;
		UICamera.SortCamerasByDepth();
		if (this.backfillQuad)
		{
			this.backfillQuad.gameObject.layer = this.CameraLayer;
		}
		base.name = string.Format("ViewLayer {0}", this.CameraLayer);
	}

	private void OnDestroy()
	{
		this.viewManager.OnScreenChanged -= this.HandleScreenChanged;
		this.fiber.Terminate();
	}

	private void HandleScreenChanged()
	{
		this.AdjustViewAndCamSize(this.View);
		this.View.gameObject.SendMessage("ScreenSizeChanged", SendMessageOptions.DontRequireReceiver);
	}

	public void ShowView()
	{
		this.fiber.Start(FiberHelper.RunParallel(new IEnumerator[]
		{
			this.AnimateIn(),
			this.FadeBackfill(UIViewManager.Instance.BackQuadAlpha)
		}));
	}

	public void DoPreFade()
	{
		this.InvokeViewWillAppear();
		if (!this.View.IgnoreFocus)
		{
			this.InvokeViewGotFocus();
		}
		this.fiber.Start(this.FadeBackfill(UIViewManager.Instance.BackQuadAlpha));
	}

	public void DoPostFade()
	{
		this.InvokeViewDidAppear();
	}

	private void InvokeViewWillAppear()
	{
		this.View.gameObject.BroadcastMessage("ViewWillAppear", SendMessageOptions.DontRequireReceiver);
		UIViewManager.Instance.InvokeViewWillAppear(this.View);
	}

	private void InvokeViewDidAppear()
	{
		this.View.gameObject.BroadcastMessage("ViewDidAppear", SendMessageOptions.DontRequireReceiver);
	}

	private void InvokeViewWillDisappear()
	{
		this.View.gameObject.BroadcastMessage("ViewWillDisappear", SendMessageOptions.DontRequireReceiver);
		UIViewManager.Instance.InvokeViewWillDisappear(this.View);
	}

	private void InvokeViewDidDisappear()
	{
		this.View.gameObject.BroadcastMessage("ViewDidDisappear", SendMessageOptions.DontRequireReceiver);
		UIViewManager.Instance.InvokeViewDidDisappear(this.View);
	}

	private void InvokeViewGotFocus()
	{
		this.View.gameObject.BroadcastMessage("ViewGotFocus", SendMessageOptions.DontRequireReceiver);
	}

	private void InvokeViewLostFocus()
	{
		this.View.gameObject.BroadcastMessage("ViewLostFocus", SendMessageOptions.DontRequireReceiver);
	}

	public void ToggleBackFill(bool toggle)
	{
		this.backfillQuad.gameObject.SetActive(toggle);
	}

	private IEnumerator FadeBackfill(float target)
	{
		if (this.backfillQuad == null)
		{
			yield break;
		}
		float src = this.backfillQuad.Alpha;
		yield return FiberAnimation.Animate(0.3f, delegate(float f)
		{
			this.backfillQuad.Alpha = Mathf.Lerp(src, target, f);
		});
		yield break;
	}

	private IEnumerator AnimateIn()
	{
		this.animationState = UIViewLayer.AnimationState.AnimatingIn;
		this.InvokeViewWillAppear();
		if (!this.View.IgnoreFocus)
		{
			this.InvokeViewGotFocus();
		}
		List<IEnumerator> animList = new List<IEnumerator>();
		UIViewLayerAnimation layerAnimation = this.View.GetViewLayerAnimation();
		if (layerAnimation != null)
		{
			animList.Add(layerAnimation.AnimateIn(this.View));
		}
		animList.Add(this.View.AnimateIn());
		yield return FiberHelper.RunParallel(animList.ToArray());
		this.InvokeViewDidAppear();
		this.animationState = UIViewLayer.AnimationState.None;
		yield break;
	}

	private IEnumerator AnimateClose()
	{
		this.animationState = UIViewLayer.AnimationState.AnimatingOut;
		this.InvokeViewWillDisappear();
		if (!this.View.IgnoreFocus)
		{
			this.InvokeViewLostFocus();
		}
		List<IEnumerator> animList = new List<IEnumerator>();
		animList.Add(this.View.AnimateOut());
		UIViewLayerAnimation layerAnimation = this.View.GetViewLayerAnimation();
		if (layerAnimation != null)
		{
			animList.Add(layerAnimation.AnimateOut(this.View));
		}
		yield return FiberHelper.RunParallel(animList.ToArray());
		this.InvokeViewDidDisappear();
		UIViewManager.Instance.ReleaseView(this.View);
		UIViewManager.Instance.ReleaseLayer(this);
		this.animationState = UIViewLayer.AnimationState.None;
		yield break;
	}

	private void HandleCloseView()
	{
		IUIView view = this.View;
		view.OnClosed = (Action)Delegate.Remove(view.OnClosed, new Action(this.HandleCloseView));
		this.fiber.Start(this.CloseViewCr());
	}

	public void CloseInstantly()
	{
		this.fiber.Terminate();
		if (this.animationState != UIViewLayer.AnimationState.AnimatingOut)
		{
			this.InvokeViewWillDisappear();
			if (!this.View.IgnoreFocus)
			{
				this.InvokeViewLostFocus();
			}
		}
		this.InvokeViewDidDisappear();
		UIViewManager.Instance.ReleaseView(this.View);
		UIViewManager.Instance.ReleaseLayer(this);
	}

	private IEnumerator CloseViewCr()
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.AnimateClose(),
			this.FadeBackfill(0f)
		});
		if (this.backfillQuad != null)
		{
			UnityEngine.Object.Destroy(this.backfillQuad.gameObject);
		}
		yield break;
	}

	public void Prepare(IUIView v)
	{
		v.transform.parent = base.transform;
		v.transform.localPosition = Vector2.zero;
		v.gameObject.SetActive(true);
		this.AdjustViewAndCamSize(v);
		v.gameObject.SetLayerRecursively(this.CameraLayer);
		UIViewLayer.SetLightsCullingMaskRecursively(v.transform, 1 << this.CameraLayer);
	}

	public void PrepareOverlay(IUIView v, IUIView sourceView)
	{
		v.transform.parent = base.transform;
		v.transform.localPosition = Vector2.zero;
		v.gameObject.SetActive(true);
		v.GetElement().SetSizeAndDoLayout(sourceView.GetElementSize());
		v.gameObject.SetLayerRecursively(this.CameraLayer);
		UIViewLayer.SetLightsCullingMaskRecursively(v.transform, 1 << this.CameraLayer);
	}

	private static void SetLightsCullingMaskRecursively(Transform root, int mask)
	{
		if (root.GetComponent<Light>() != null)
		{
			root.GetComponent<Light>().cullingMask = mask;
		}
		IEnumerator enumerator = root.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform root2 = (Transform)obj;
				UIViewLayer.SetLightsCullingMaskRecursively(root2, mask);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void AdjustViewAndCamSize(IUIView view)
	{
		float bottomViewZoneHeight = UIViewManager.Instance.BottomViewZoneHeight;
		Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Rect rect2 = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Rect rect3 = rect;
		float num = 0f;
		if (bottomViewZoneHeight > 0f)
		{
			num = rect3.yMin + bottomViewZoneHeight;
		}
		if (view.UseBottomViewZone)
		{
			rect3.yMax = num;
			rect.yMax = num;
			rect2.yMax = num;
		}
		else
		{
			rect3.yMin = Mathf.Max(rect3.yMin, num);
			rect.yMin = num;
			rect2.yMin = num;
		}
		Rect rect4 = (!view.IgnoreSafeArea) ? rect3 : rect;
		Vector2 sizeAndDoLayout = view.CalculateViewSizeForScreen(rect4.size);
		float num2 = sizeAndDoLayout.y / rect4.size.y;
		if (this.backfillQuad != null)
		{
			this.backfillQuad.Size = rect2.size * num2 * 1.1f;
		}
		float num3 = num / (float)Screen.height;
		if (view.UseBottomViewZone)
		{
			this.ViewCamera.rect = new Rect(0f, 0f, 1f, num3);
		}
		else
		{
			this.ViewCamera.rect = new Rect(0f, num3, 1f, 1f);
		}
		this.ViewCamera.orthographicSize = (sizeAndDoLayout.y + num2 * (rect2.size.y - rect4.size.y)) * 0.5f;
		this.ViewCamera.transform.localPosition = Vector3.back * this.ViewCamera.orthographicSize / Mathf.Tan(this.ViewCamera.fieldOfView * 0.0174532924f * 0.5f);
		this.ViewCamera.nearClipPlane = -300f - this.ViewCamera.transform.localPosition.z;
		this.ViewCamera.farClipPlane = 300f - this.ViewCamera.transform.localPosition.z;
		foreach (UISafeAreaElement uisafeAreaElement in view.gameObject.GetComponentsInChildren<UISafeAreaElement>())
		{
			uisafeAreaElement.SetScreenAndSafeAreaRects(rect, rect3);
		}
		view.GetElement().SetSizeAndDoLayout(sizeAndDoLayout);
	}

	public UIViewLayer.AnimationState animationState;

	private UITextureQuad backfillQuad;

	private Fiber fiber = new Fiber();

	private UIViewManager viewManager;

	public enum AnimationState
	{
		None,
		AnimatingIn,
		AnimatingOut
	}
}
