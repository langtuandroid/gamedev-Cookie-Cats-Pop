using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Fibers;
using TactileModules.RuntimeTools;
using UnityEngine;

public class UIViewManager : MonoBehaviour, IUIViewManager, IViewStack, IViewPresenter
{
    public static UIViewManager Instance { get; private set; }

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action OnScreenChanged;

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action OnFadedToBlack;

    public float CurrentScreenWidth { get; private set; }

    public float CurrentScreenHeight { get; private set; }

    public float CurrentAspectRatio
    {
        get
        {
            return this.CurrentScreenHeight / this.CurrentScreenWidth;
        }
    }

    public float AvailableScreenWidth
    {
        get
        {
            return (float)Screen.width;
        }
    }

    public float AvailableScreenHeight
    {
        get
        {
            return (float)Screen.height - this.BottomViewZoneHeight;
        }
    }

    public float BottomViewZoneHeight { get; private set; }

    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<IUIView> ViewWillAppear = delegate (IUIView A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<IUIView> ViewDidDisappear = delegate (IUIView A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<IUIView> ViewWillDisappear = delegate (IUIView A_0)
    {
    };




    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<IUIView> OnViewStackPush = delegate (IUIView A_0)
    {
    };



    ////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public event Action<IUIView> OnViewStackPop = delegate (IUIView A_0)
    {
    };



    public int StackCount
    {
        
        get
        {
            UnityEngine.Debug.Log("StackCount");
            return this.layerStack.Count;
        }
    }

    public IUIView GetAtIndex(int index)
    {
        UnityEngine.Debug.Log("GetAtIndex");
        return (index >= this.layerStack.Count) ? null : this.layerStack[index].View;
    }

    public int GetLayerIndexFromGameObjectLayer(GameObject gameObject)
    {
        UnityEngine.Debug.Log("GetLayerIndex");
        for (int i = 0; i < this.layerStack.Count; i++)
        {
            UIViewLayer uiviewLayer = this.layerStack[i];
            if (uiviewLayer.CameraLayer == gameObject.layer)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetBottomViewZoneHeight(float modifier = 0f)
    {
        this.BottomViewZoneHeight = modifier;
        this.fadeCamera.rect = this.GetMainViewZoneRenderRect();
    }

    public Rect GetMainViewZoneRenderRect()
    {
        return new Rect(0f, 1f - UIViewManager.Instance.AvailableScreenHeight / (float)Screen.height, 1f, 1f);
    }

    public UIViewManager.Orientation CurrentOrientation
    {
        get
        {
            return (this.CurrentAspectRatio < 1f) ? UIViewManager.Orientation.Landscape : UIViewManager.Orientation.Portrait;
        }
    }

    public bool AnyViewsAnimating
    {
        get
        {
            UnityEngine.Debug.Log("Anyviews");
            for (int i = 0; i < this.layerStack.Count; i++)
            {
                if (this.layerStack[i].animationState != UIViewLayer.AnimationState.None)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public float BackQuadAlpha { get; private set; }

    public bool IsFading
    {
        get
        {
            return !this.fadeFiber.IsTerminated || this.fadingToNewView;
        }
    }

    public static UIViewManager CreateInstance(Func<string, string> localizationFunction = null, bool recordDebugData = false, int actionQueueSize = 10, float backQuadAlpha = 0.3f)
    {
        if (UIViewManager.Instance != null)
        {
        }
        if (recordDebugData)
        {
            NinjaUIDebugData.Init(actionQueueSize);
        }
        UIViewManager.localizationFunction = localizationFunction;
        UIViewManager.Instance = new GameObject("_ViewManager").AddComponent<UIViewManager>();
        UIViewManager.Instance.BackQuadAlpha = backQuadAlpha;
        UnityEngine.Object.DontDestroyOnLoad(UIViewManager.Instance.gameObject);
        return UIViewManager.Instance;
    }

    public static string Localize(string key)
    {
        if (UIViewManager.localizationFunction != null)
        {
            return UIViewManager.localizationFunction(key);
        }
        return key;
    }

    private void Awake()
    {
        this.CurrentScreenWidth = (float)Screen.width;
        this.CurrentScreenHeight = (float)Screen.height;
        this.poolRoot = new GameObject("_ViewPool").transform;
        this.poolRoot.parent = base.transform;
        this.fadeObject = new GameObject("FadeObject");
        this.fadeObject.transform.parent = base.transform;
        this.fadeCamera = this.fadeObject.AddComponent<Camera>();
        this.fadeCamera.orthographic = true;
        this.fadeCamera.orthographicSize = 1f;
        this.fadeCamera.name = "FadeCamera";
        this.fadeCamera.nearClipPlane = 0.5f;
        this.fadeCamera.farClipPlane = 1.5f;
        this.fadeCamera.clearFlags = CameraClearFlags.Depth;
        Texture2D texture2D = new Texture2D(1, 1);
        texture2D.name = "[Generated] white";
        texture2D.SetPixel(0, 0, Color.white);
        texture2D.Apply();
        texture2D.filterMode = FilterMode.Bilinear;
        this.fadeMaterial = new Material(Shader.Find("Unlit/Transparent Colored"));
        this.fadeMaterial.mainTexture = texture2D;
        this.fadeQuad = new GameObject("quad");
        this.fadeQuad.transform.parent = this.fadeObject.transform;
        this.fadeQuad.transform.localPosition = new Vector3(0f, 0f, 1f);
        this.fadeMesh = new Mesh();
        this.fadeMesh.vertices = new Vector3[]
        {
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f)
        };
        this.fadeColors = new Color[]
        {
            Color.black,
            Color.black,
            Color.black,
            Color.black
        };
        this.fadeMesh.colors = this.fadeColors;
        this.fadeMesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f)
        };
        this.fadeMesh.triangles = new int[]
        {
            0,
            2,
            1,
            0,
            3,
            2
        };
        this.fadeQuad.AddComponent<MeshFilter>().sharedMesh = this.fadeMesh;
        this.fadeQuad.AddComponent<MeshRenderer>().sharedMaterial = this.fadeMaterial;
        this.fadeObject.SetActive(false);
        this.AdjustFadeQuad();
    }

    public bool HasViewInDynamicPool(string viewName)
    {
        return this.dynamicPools.ContainsKey(viewName);
    }

    public List<UIView> GetViewsInDynamicPool()
    {
        List<UIView> list = new List<UIView>();
        foreach (KeyValuePair<string, UIViewManager.ViewPool> keyValuePair in this.dynamicPools)
        {
            list.Add(keyValuePair.Value.prefab);
        }
        return list;
    }

    public bool HasViewInStaticPool(Type viewType)
    {
        return this.pools.ContainsKey(viewType);
    }

    public void AddNewViewToPool(UIView view)
    {
        if (this.HasDynamicView(view.name))
        {
            return;
        }
        UIViewManager.ViewPool value = new UIViewManager.ViewPool(view, this.poolRoot);
        this.dynamicPools.Add(view.name, value);
    }

    public void AddOrReplaceView(UIView view)
    {
        if (this.HasDynamicView(view.name))
        {
            this.dynamicPools[view.name].prefab = view;
            return;
        }
        this.AddNewViewToPool(view);
    }

    public UIViewLayer TopLayer
    {
        get
        {

            if (this.layerStack.Count > 0)
            {
                for (int i = this.layerStack.Count - 1; i >= 0; i--)
                {
                    if (this.layerStack[i].View != null && !this.layerStack[i].View.UseBottomViewZone)
                    {
                        return this.layerStack[i];
                    }
                }
                return null;
            }
            return null;
        }
    }

    public Camera GetCamera()
    {
        if (this.TopLayer != null)
        {
            return this.TopLayer.UICamera.GetComponent<Camera>();
        }
        return null;
    }

    public UIViewLayer ObtainLayer(IUIView view)
    {
        NinjaUIDebugData.LogAction("Obtain:" + view.name);
        UIViewLayer topLayer = this.TopLayer;
        int num = 20;
        if (!view.UseBottomViewZone)
        {
            if (topLayer != null)
            {
                num = topLayer.CameraLayer + 1;
            }
        }
        else
        {
            num = 30;
        }
        UIViewLayer uiviewLayer = new GameObject(string.Format("ViewLayer {0}", num)).AddComponent<UIViewLayer>();
        uiviewLayer.transform.parent = UIViewManager.Instance.transform;
        uiviewLayer.Initialize(this, view, num, view.BackgroundColor);
        if (this.TopLayer != null && !this.TopLayer.View.IgnoreFocus && !view.UseBottomViewZone)
        {
            this.TopLayer.View.gameObject.BroadcastMessage("ViewLostFocus", SendMessageOptions.DontRequireReceiver);
        }
        this.layerStack.Add(uiviewLayer);
        int num2 = num + 1;
        this.fadeCamera.cullingMask = 1 << num2;
        this.fadeCamera.depth = (float)num2;
        this.fadeQuad.layer = num2;
        return uiviewLayer;
    }

    public void ReleaseLayer(UIViewLayer layer)
    {
        UnityEngine.Debug.Log("ReleaseLayer");
        NinjaUIDebugData.LogAction("Release:" + layer.View.name);
        this.ReleaseViewFromOverlays(layer.View);
        this.layerStack.Remove(layer);
        if (this.TopLayer != null && !this.TopLayer.View.IgnoreFocus && !layer.View.UseBottomViewZone)
        {
            this.TopLayer.View.gameObject.BroadcastMessage("ViewGotFocus", SendMessageOptions.DontRequireReceiver);
        }
        this.OnViewStackPop(layer.View);
        UnityEngine.Object.Destroy(layer.gameObject);
    }

    public void MoveLayerToTop(UIViewLayer layer)
    {
        NinjaUIDebugData.LogAction("MoveToTop:" + layer.View.name);
        int num = 20;
        UIViewLayer topLayer = this.TopLayer;
        if (topLayer == layer)
        {
            return;
        }
        if (topLayer)
        {
            num = topLayer.CameraLayer + 1;
        }
        layer.ChangeLayer(num);
        layer.View.gameObject.SetLayerRecursively(num);
        int num2 = num + 1;
        this.fadeCamera.cullingMask = 1 << num2;
        this.fadeCamera.depth = (float)num2;
        this.fadeQuad.layer = num2;
    }

    public T FindOverlay<T>() where T : UIView
    {
        UIViewManager.UIOverlay uioverlay;
        if (this.overlays.TryGetValue(typeof(T), out uioverlay))
        {
            return (T)((object)uioverlay.View);
        }
        return (T)((object)null);
    }

    public void CloseAll(params IUIView[] exceptTheseViews)
    {

        NinjaUIDebugData.LogAction("CloseAll");
        List<IUIView> list = new List<IUIView>(exceptTheseViews);
        for (int i = this.layerStack.Count - 1; i >= 0; i--)
        {
            if (!list.Contains(this.layerStack[i].View) && !this.layerStack[i].View.UseBottomViewZone)
            {
                this.layerStack[i].CloseInstantly();
            }
        }
    }

    public T FindView<T>() where T : UIView
    {
        return (T)((object)this.FindView(typeof(T)));
    }

    public UIView FindView(Type type)
    {
       
        for (int i = this.layerStack.Count - 1; i >= 0; i--)
        {
             Component component = this.layerStack[i].View.gameObject.GetComponent(type);
            if (component != null)
            {
                       return (UIView)component;
             }
            }
          return null;
    }

    private void StartFadeUp(float duration = 0f)
    {
        this.fadeFiber.Start(this.FadeCameraFrontFill(0f, duration, 1));
    }

    public IEnumerator WaitForFadeUp()
    {
        while (!this.fadeFiber.IsTerminated)
        {
            yield return null;
        }
        yield break;
    }

    public IEnumerator WaitForFadeDown()
    {
        while (this.fadingToNewView)
        {
            yield return null;
        }
        yield break;
    }

    public IEnumerator WaitForFadeAndSwitch()
    {
        while (!this.fiber.IsTerminated)
        {
            yield return null;
        }
        yield break;
    }

    public void FadeAndSwitchView<T>(params object[] initialParameters) where T : UIView
    {
        this.fiber.Start(this.FadeAndSwitchViewCr<T>(false, initialParameters));
    }

    public void FadeAndSwitchView(Type viewType, params object[] initialParameters)
    {
        this.fiber.Start(this.FadeAndSwitchViewCr(viewType, false, initialParameters));
    }

    public void FadeAndSwitchView(string viewName, params object[] initialParameters)
    {
        this.fiber.Start(this.FadeAndSwitchViewCr(viewName, false, initialParameters));
    }

    public void FadeAndSwitchViewWithAnimation<T>(params object[] initialParameters) where T : UIView
    {
        this.fiber.Start(this.FadeAndSwitchViewCr<T>(true, initialParameters));
    }

    public UIViewManager.UIViewStateGeneric<T> ShowView<T>(params object[] initialParameters) where T : UIView
    {
        T view = this.GetView<T>(initialParameters);
        UIViewLayer uiviewLayer = this.ObtainLayer(view);
        UIViewManager.UIViewStateGeneric<T> result = new UIViewManager.UIViewStateGeneric<T>(view);
        uiviewLayer.ShowView();
        this.OnLayerViewShow(uiviewLayer);
        return result;

    }

    public UIViewManager.UIViewState ShowView(Type viewType, params object[] initialParameters)
    {
        IUIView view = this.GetView(viewType, initialParameters);
        UIViewLayer uiviewLayer = this.ObtainLayer(view);
        UIViewManager.UIViewStateGeneric<IUIView> result = new UIViewManager.UIViewStateGeneric<IUIView>(view);
        uiviewLayer.ShowView();
        this.OnLayerViewShow(uiviewLayer);
        return result;
    }

    public UIViewManager.UIViewState ShowView(string viewName, params object[] initialParameters)
    {
        IUIView view = this.GetView(viewName, initialParameters);
        UIViewLayer uiviewLayer = this.ObtainLayer(view);
        UIViewManager.UIViewStateGeneric<IUIView> result = new UIViewManager.UIViewStateGeneric<IUIView>(view);
        uiviewLayer.ShowView();
        this.OnLayerViewShow(uiviewLayer);
        return result;
    }

    public UIViewManager.IUIViewStateGeneric<T> ShowViewFromPrefab<T>(T viewPrefab, params object[] initialParameters) where T : UIView
    {
        T view = UnityEngine.Object.Instantiate<T>(viewPrefab);
        return this.ShowViewInstance<T>(view, initialParameters);
    }

    public UIViewManager.IUIViewStateGeneric<T> ShowViewInstance<T>(T view, params object[] initialParameters) where T : IUIView
    {
        if (initialParameters == null)
        {
            initialParameters = new object[1];
        }
        view.LoadedFromPool(initialParameters);
        UIViewLayer uiviewLayer = this.ObtainLayer(view);
        UIViewManager.UIViewStateGeneric<T> result = new UIViewManager.UIViewStateGeneric<T>(view);
        uiviewLayer.ShowView();
        this.OnLayerViewShow(uiviewLayer);
        this.DispatchViewWillAppear(view);
        return result;
    }

    private void OnLayerViewShow(UIViewLayer layer)
    {
        this.OnViewStackPush(layer.View);
    }

    public IUIView GetView(string viewName, params object[] parameters)
    {
        foreach (KeyValuePair<string, UIViewManager.ViewPool> keyValuePair in this.dynamicPools)
        {
        }
        if (this.HasDynamicView(viewName))
        {
            IUIView iuiview = this.dynamicPools[viewName].Obtain();
            if (parameters == null)
            {
                parameters = new object[1];
            }
            iuiview.LoadedFromPool(parameters);
            this.DispatchViewWillAppear(iuiview);
            return iuiview;
        }
        return null;
    }

    public T GetView<T>(params object[] parameters) where T : UIView
    {
        return (T)((object)this.GetView(typeof(T), parameters));
    }

    public IUIView GetView(Type viewType, params object[] parameters)
    {
        if (!this.pools.ContainsKey(viewType))
        {
            UIProjectSettings.UIProjectSettingsData settingsData = UIProjectSettings.Get().SettingsData;
            UIProjectSettings.ViewInfo viewInfo = settingsData.FindInfoWithType(viewType.FullName);
            if (viewInfo == null)
            {
                return null;
            }
            string path = AssetPathUtility.AssetPathToResourcePath(viewInfo.AssetPath);
            this.AddViewFromResources(path, false);
        }
        if (this.pools.ContainsKey(viewType))
        {
            IUIView iuiview = this.pools[viewType].Obtain();
            if (parameters == null)
            {
                parameters = new object[1];
            }
            iuiview.LoadedFromPool(parameters);
            this.DispatchViewWillAppear(iuiview);
            return iuiview;
        }
        return null;
    }

    public bool HasDynamicView(string viewName)
    {
        return this.dynamicPools.ContainsKey(viewName);
    }

    private void DispatchViewWillAppear(IUIView view)
    {
        if (this.OnViewWillAppear != null)
        {
            this.OnViewWillAppear(view);
        }
    }

    public T ObtainOverlay<T>(IUIView layerView) where T : UIView
    {
        UIViewLayer viewLayerWithView = this.GetViewLayerWithView(layerView);
        if (viewLayerWithView == null)
        {
            return (T)((object)null);
        }
        Type typeFromHandle = typeof(T);
        UIViewManager.UIOverlay uioverlay;
        IUIView iuiview;
        if (!this.overlays.TryGetValue(typeFromHandle, out uioverlay))
        {
            iuiview = this.GetView<T>(new object[0]);
            if (iuiview == null)
            {
                return (T)((object)null);
            }
            uioverlay = new UIViewManager.UIOverlay(iuiview);
            this.overlays.Add(typeFromHandle, uioverlay);
            iuiview.gameObject.BroadcastMessage("ViewWillAppear", SendMessageOptions.DontRequireReceiver);
            iuiview.gameObject.BroadcastMessage("ViewDidAppear", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            iuiview = uioverlay.View;
        }
        uioverlay.PushLayer(layerView);
        viewLayerWithView.PrepareOverlay(iuiview, layerView);
        this.AdjustOverlaySize(uioverlay);
        return (T)((object)iuiview);
    }

    public void ReleaseOverlay<T>() where T : UIView
    {
        this.ReleaseOverlay(typeof(T), null);
    }

    private void ReleaseOverlay(Type type, IUIView viewToRemove)
    {
        UIViewManager.UIOverlay uioverlay;
        if (this.overlays.TryGetValue(type, out uioverlay))
        {
            if (viewToRemove == null)
            {
                uioverlay.PopLayer();
            }
            else
            {
                uioverlay.RemoveFromOverlayStack(viewToRemove);
            }
            IUIView view = uioverlay.View;
            if (uioverlay.TopLayer == null)
            {
                this.overlays.Remove(type);
                view.gameObject.BroadcastMessage("ViewWillDisappear", SendMessageOptions.DontRequireReceiver);
                view.gameObject.BroadcastMessage("ViewDidDisappear", SendMessageOptions.DontRequireReceiver);
                this.ReleaseView(view);
            }
            else
            {
                UIViewLayer viewLayerWithView = this.GetViewLayerWithView(uioverlay.TopLayer);
                if (viewLayerWithView != null)
                {
                    viewLayerWithView.PrepareOverlay(view, uioverlay.TopLayer);
                }
                this.AdjustOverlaySize(uioverlay);
            }
        }
    }

    private void ReleaseViewFromOverlays(IUIView view)
    {
        List<Type> list = null;
        foreach (KeyValuePair<Type, UIViewManager.UIOverlay> keyValuePair in this.overlays)
        {
            if (keyValuePair.Value.IsViewUsingOverlay(view))
            {
                if (list == null)
                {
                    list = new List<Type>();
                }
                list.Add(keyValuePair.Key);
            }
        }
        if (list == null)
        {
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            this.ReleaseOverlay(list[i], view);
        }
    }

    public UIViewLayer GetViewLayerWithView(IUIView view)
    {
        for (int i = 0; i < this.layerStack.Count; i++)
        {
            if (this.layerStack[i].View == view)
            {
                return this.layerStack[i];
            }
        }
        return null;
    }

    private void DispatchViewWillRelease(IUIView view)
    {
        if (this.OnViewWillRelease != null)
        {
            this.OnViewWillRelease(view);
        }
    }

    public void ReleaseView(IUIView view)
    {
        this.ReleaseViewFromOverlays(view);
        this.DispatchViewWillRelease(view);
        if (this.pools.ContainsKey(view.GetType()))
        {
            this.pools[view.GetType()].Release(view);
        }
        else if (this.dynamicPools.ContainsKey(view.name))
        {
            this.dynamicPools[view.name].Release(view);
        }
    }

    public bool IsEscapeKeyDownAndAvailable(int layer)
    {
        if (UICamera.InputDisabled)
        {
            return false;
        }
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return false;
        }
        UICamera x = UICamera.FindCameraForLayer(layer);
        return !(x != this.FindTopMostCameraWithViews());
    }

    public UICamera FindTopMostCameraWithViews()
    {
        UnityEngine.Debug.Log("FindTopMostCameraWithViews");
        if (this.layerStack.Count > 0)
        {
            return this.layerStack[this.layerStack.Count - 1].UICamera;
        }
        return null;
    }

    public UICamera FindCameraFromObjectLayer(int objectLayer)
    {
        UnityEngine.Debug.Log("FindCameraFromObjectLayer");
        foreach (UIViewLayer uiviewLayer in this.layerStack)
        {
            if (uiviewLayer.CameraLayer == objectLayer)
            {
                return uiviewLayer.UICamera;
            }
        }
        return null;
    }

    public static UITextureQuad CreateBackfillQuad()
    {
        UITextureQuad uitextureQuad = new GameObject("[BackFillQuad]", new Type[]
        {
            typeof(UIElement)
        }).AddComponent<UITextureQuad>();
        uitextureQuad.Color = Color.clear;
        uitextureQuad.Material = UIViewManager.GetBackFillMaterial();
        return uitextureQuad;
    }

    private void Update()
    {
        if (this.CurrentScreenHeight == this.AvailableScreenHeight && this.CurrentScreenWidth == this.AvailableScreenWidth)
        {
            return;
        }
        this.ReflectScreenChanged();
    }

    private void AdjustFadeQuad()
    {
        float x = this.CurrentScreenWidth / this.CurrentScreenHeight;
        this.fadeQuad.transform.localScale = new Vector3(x, 1f, 0f);
    }

    private void ReflectScreenChanged()
    {
        this.CurrentScreenWidth = this.AvailableScreenWidth;
        this.CurrentScreenHeight = this.AvailableScreenHeight;
        this.AdjustFadeQuad();
        if (this.OnScreenChanged != null)
        {
            this.OnScreenChanged();
        }
        foreach (KeyValuePair<Type, UIViewManager.UIOverlay> keyValuePair in this.overlays)
        {
            this.AdjustOverlaySize(keyValuePair.Value);
            keyValuePair.Value.View.gameObject.SendMessage("ScreenSizeChanged", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void AdjustOverlaySize(UIViewManager.UIOverlay overlay)
    {
        float orthographicSize = this.GetViewLayerWithView(overlay.TopLayer).ViewCamera.orthographicSize;
        float orthographicSize2 = this.GetViewLayerWithView(overlay.BottomLayer).ViewCamera.orthographicSize;
        float num = orthographicSize / orthographicSize2;
        Vector2 size = overlay.TopLayer.gameObject.GetComponent<UIElement>().Size;
        IUIView view = overlay.View;
        UIElement component = view.gameObject.GetComponent<UIElement>();
        view.transform.localScale = new Vector3(num, num, view.transform.localScale.z);
        component.Size = 1f / num * size;
    }

    private IEnumerator FadeAndSwitchViewCr<T>(bool withAnimation, params object[] initialParameters) where T : UIView
    {
        this.fadingToNewView = true;
        yield return new Fiber.OnExit(delegate ()
        {
            this.fadingToNewView = false;
        });
        yield return this.StartFadeAndSwitch();
        T view = this.GetView<T>(initialParameters);
        yield return this.FinishFadeAndSwitch(view, withAnimation);
        yield break;
    }

    private IEnumerator FadeAndSwitchViewCr(string viewName, bool withAnimation, params object[] initialParameters)
    {
        this.fadingToNewView = true;
        yield return new Fiber.OnExit(delegate ()
        {
            this.fadingToNewView = false;
        });
        yield return this.StartFadeAndSwitch();
        IUIView view = this.GetView(viewName, initialParameters);
        yield return this.FinishFadeAndSwitch(view, withAnimation);
        yield break;
    }

    private IEnumerator FadeAndSwitchViewCr(Type viewType, bool withAnimation, params object[] initialParameters)
    {
        this.fadingToNewView = true;
        yield return new Fiber.OnExit(delegate ()
        {
            this.fadingToNewView = false;
        });
        yield return this.StartFadeAndSwitch();
        IUIView view = this.GetView(viewType, initialParameters);
        yield return this.FinishFadeAndSwitch(view, withAnimation);
        yield break;
    }

    private IEnumerator StartFadeAndSwitch()
    {
        UnityEngine.Debug.Log("StartFadeAndSwitch");
        if (this.layerStack.Count > 0)
        {
            yield return this.FadeCameraFrontFill(1f, 0f, 0);
            if (this.OnFadedToBlack != null)
            {
                this.OnFadedToBlack();
            }
            this.CloseAll(new IUIView[0]);
        }
        else
        {
            this.FrontFill = 1f;
        }
        yield break;
    }

    private IEnumerator FinishFadeAndSwitch(IUIView view, bool withAnimation)
    {
        UIViewLayer viewLayer = this.ObtainLayer(view);
        this.fadingToNewView = false;
        if (withAnimation)
        {
            viewLayer.ShowView();
            this.StartFadeUp(0f);
        }
        else
        {
            viewLayer.DoPreFade();
            this.StartFadeUp(0f);
            yield return this.WaitForFadeUp();
            viewLayer.DoPostFade();
        }
        yield break;
    }

    public float FrontFill
    {
        get
        {
            return this.fadeColors[0].a;
        }
        set
        {
            for (int i = 0; i < this.fadeColors.Length; i++)
            {
                this.fadeColors[i].a = value;
            }
            if (value > 0f)
            {
                this.fadeMesh.colors = this.fadeColors;
                this.fadeObject.SetActive(true);
            }
            else
            {
                this.fadeObject.SetActive(this.StackCount == 0);
            }
        }
    }

    public Color FrontFillColor
    {
        get
        {
            return this.fadeColors[0];
        }
        set
        {
            for (int i = 0; i < this.fadeColors.Length; i++)
            {
                this.fadeColors[i].r = value.r;
                this.fadeColors[i].g = value.g;
                this.fadeColors[i].b = value.b;
            }
            this.FrontFill = this.FrontFill;
        }
    }

    public IEnumerator FadeCameraFrontFill(float target, float duration = 0f, int pauseFrames = 0)
    {
        for (int f = 0; f < pauseFrames; f++)
        {
            yield return null;
        }
        if (duration <= 0f)
        {
            duration = UIViewManager.defaultFadeDuration;
        }
        UICamera.DisableInput();
       
        yield return new Fiber.OnExit(new Fiber.OnExitHandler(UICamera.EnableInput));
        float from = this.FrontFill;
        float timer = 0f;
        while (timer < duration)
        {
            this.FrontFill = Mathf.Lerp(from, target, timer / duration);
            timer += Mathf.Min(Time.deltaTime, 0.033f);
            yield return null;
        }
        this.FrontFill = target;
        yield break;
    }

    private static Material GetBackFillMaterial()
    {
        if (UIViewManager.sharedBackFillMaterial == null)
        {
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.name = "[Generated] white";
            texture2D.SetPixel(0, 0, Color.white);
            texture2D.Apply();
            texture2D.filterMode = FilterMode.Bilinear;
            UIViewManager.sharedBackFillMaterial = new Material(Shader.Find("Unlit/Transparent Colored"));
            UIViewManager.sharedBackFillMaterial.renderQueue = 0;
            UIViewManager.sharedBackFillMaterial.mainTexture = texture2D;
        }
        return UIViewManager.sharedBackFillMaterial;
    }

    public void AddViewFromResources(string path, bool removeWhenUsed = true)
    {
        GameObject gameObject = Resources.Load<GameObject>(path);
        if (gameObject == null)
        {
            return;
        }
        UIView component = gameObject.GetComponent<UIView>();
        if (component == null)
        {
            return;
        }
        Type type = component.GetType();
        if (this.pools.ContainsKey(type))
        {
            return;
        }
        UIViewManager.ViewPool viewPool = new UIViewManager.ViewPool(component, this.poolRoot);
        this.pools.Add(type, viewPool);
        viewPool.deleteOnUsedOnce = removeWhenUsed;
    }

    public string[] GetViewNameStack()
    {

        string[] array = new string[this.layerStack.Count];
        for (int i = 0; i < this.layerStack.Count; i++)
        {
            if (this.layerStack[i].View != null)
            {
                array[i] = this.layerStack[i].View.name;
            }
            else
            {
                array[i] = "null";
            }
        }
        return array;
    }

    public void InvokeViewWillAppear(IUIView view)
    {
        this.ViewWillAppear(view);
    }

    public void InvokeViewDidDisappear(IUIView view)
    {
        this.ViewDidDisappear(view);
    }

    public void InvokeViewWillDisappear(IUIView view)
    {
        this.ViewWillDisappear(view);
    }

    private static Func<string, string> localizationFunction;

    private static Material sharedBackFillMaterial;

    public Action<IUIView> OnViewWillAppear;

    public Action<IUIView> OnViewWillRelease;

    private List<UIViewLayer> layerStack = new List<UIViewLayer>();

    private Fiber fiber = new Fiber();

    private Fiber fadeFiber = new Fiber();

    private Transform poolRoot;

    private Dictionary<Type, UIViewManager.ViewPool> pools = new Dictionary<Type, UIViewManager.ViewPool>();

    private Dictionary<string, UIViewManager.ViewPool> dynamicPools = new Dictionary<string, UIViewManager.ViewPool>();

    private Dictionary<Type, UIViewManager.UIOverlay> overlays = new Dictionary<Type, UIViewManager.UIOverlay>();

    private GameObject fadeObject;

    private Camera fadeCamera;

    private Material fadeMaterial;

    private Mesh fadeMesh;

    private GameObject fadeQuad;

    private Color[] fadeColors;

    private bool fadingToNewView;

    public static float defaultFadeDuration = 0.3f;

   

    public enum Orientation
    {
        Portrait,
        Landscape
    }

    protected class ViewPool
    {
        public ViewPool(UIView prefab, Transform root)
        {
            this.prefab = prefab;
            this.root = root;
            for (int i = 0; i < prefab.PoolingAmount; i++)
            {
                this.CreateAvailable();
            }
        }

        private void CreateAvailable()
        {
            UIView uiview = UnityEngine.Object.Instantiate<UIView>(this.prefab);
            uiview.name = this.prefab.name;
            uiview.gameObject.transform.parent = this.root;
            uiview.gameObject.SetActive(false);
            uiview.gameObject.transform.localPosition = Vector3.zero;
            this.available.Enqueue(uiview);
        }

        public IUIView Obtain()
        {
            if (this.available.Count == 0)
            {
                this.CreateAvailable();
            }
            IUIView iuiview = this.available.Dequeue();
            this.used.Add(iuiview);
            iuiview.gameObject.SetActive(true);
            return iuiview;
        }

        public bool Release(IUIView view)
        {
            if (!this.used.Contains(view))
            {
                return false;
            }
            this.used.Remove(view);
            view.ReleasedToPool();
            view.gameObject.SetActive(false);
            view.transform.parent = this.root;
            if (this.prefab.PoolingAmount == 0)
            {
                UnityEngine.Object.Destroy(view.gameObject);
            }
            else
            {
                this.available.Enqueue(view);
            }
            if (this.deleteOnUsedOnce)
            {
                this.DestroyAll();
                foreach (KeyValuePair<Type, UIViewManager.ViewPool> keyValuePair in UIViewManager.Instance.pools)
                {
                    if (keyValuePair.Value == this)
                    {
                        UIViewManager.Instance.pools.Remove(keyValuePair.Key);
                        break;
                    }
                }
            }
            return true;
        }

        public void DestroyAll()
        {
            foreach (IUIView iuiview in this.available)
            {
                UnityEngine.Object.Destroy(iuiview.gameObject);
            }
            foreach (IUIView iuiview2 in this.used)
            {
                UnityEngine.Object.Destroy(iuiview2.gameObject);
            }
            Resources.UnloadUnusedAssets();
            this.prefab = null;
            this.root = null;
        }

        public UIView prefab;

        public Queue<IUIView> available = new Queue<IUIView>();

        public List<IUIView> used = new List<IUIView>();

        public Transform root;

        public bool deleteOnUsedOnce;
    }

    public interface UIViewState
    {
        IUIView View { get; }

        object ClosingResult { get; }

        IEnumerator WaitForClose();
    }

    public interface IUIViewStateGeneric<T> : UIViewManager.UIViewState where T : IUIView
    {
         T View { get; }
    }

    public class UIViewStateGeneric<T> : UIViewManager.IUIViewStateGeneric<T>, UIViewManager.UIViewState where T : IUIView
    {
        public UIViewStateGeneric(T view)
        {
            this.View = view;
            UIViewManager instance = UIViewManager.Instance;
            instance.OnViewWillRelease = (Action<IUIView>)Delegate.Combine(instance.OnViewWillRelease, new Action<IUIView>(this.HandleViewRelease));
        }

        private void HandleViewRelease(IUIView releasedView)
        {
            if (releasedView == (IUIView)this.View)
            {
                UIViewManager instance = UIViewManager.Instance;
                instance.OnViewWillRelease = (Action<IUIView>)Delegate.Remove(instance.OnViewWillRelease, new Action<IUIView>(this.HandleViewRelease));
                T view = this.View;
                object closingResult;
                if (view.ClosingResult != null)
                {
                    T view2 = this.View;
                    closingResult = view2.ClosingResult;
                }
                else
                {
                    closingResult = 0;
                }
                this.ClosingResult = closingResult;
            }
        }

        IUIView UIViewManager.UIViewState.View
        {
            get
            {
                return this.View;
            }
        }

        public T View { get; private set; }

        public object ClosingResult { get; private set; }

        public IEnumerator WaitForClose()
        {
            while (this.ClosingResult == null)
            {
                yield return null;
            }
            yield break;
        }
    }

    private class UIOverlay
    {
        public UIOverlay(IUIView view)
        {
            this.View = view;
            this.layers = new List<IUIView>();
        }

        public IUIView View { get; private set; }

        public void PushLayer(IUIView layer)
        {
            this.layers.Add(layer);
        }

        public void PopLayer()
        {
            if (this.layers.Count == 0)
            {
                return;
            }
            this.layers.RemoveAt(this.layers.Count - 1);
        }

        public IUIView TopLayer
        {
            get
            {
                return (this.layers.Count <= 0) ? null : this.layers[this.layers.Count - 1];
            }
        }

        public IUIView BottomLayer
        {
            get
            {
                return (this.layers.Count <= 0) ? null : this.layers[0];
            }
        }

        public void RemoveFromOverlayStack(IUIView layer)
        {
            this.layers.Remove(layer);
        }

        public bool IsViewUsingOverlay(IUIView view)
        {
            return this.layers.Contains(view);
        }

        private List<IUIView> layers;
    }
}
