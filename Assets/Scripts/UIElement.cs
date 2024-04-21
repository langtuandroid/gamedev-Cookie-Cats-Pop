using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class UIElement : MonoBehaviour
{
    public void SizeChanged()
    {
        base.gameObject.SendMessage("UIElementSizeChanged", SendMessageOptions.DontRequireReceiver);
        if (this.boxCollider == null)
        {
            this.boxCollider = base.GetComponent<BoxCollider>();
        }
        if (this.boxCollider != null)
        {
            this.boxCollider.center = new Vector3(0f, 0f, 0f);
            this.boxCollider.size = new Vector3(this.Size.x, this.Size.y, (this.boxCollider.size.z <= 0f) ? 1f : this.boxCollider.size.z);
        }
        if (this.widget == null)
        {
            this.widget = base.GetComponent<UIWidget>();
        }
        if (this.widget != null && !this.widget.IsMarkedForChange)
        {
            this.widget.MarkAsChanged();
        }
    }

    private void GetDefaultSize()
    {
        base.gameObject.SendMessage("UIElementNeedDefaultSize", SendMessageOptions.DontRequireReceiver);
        if (this.mSize == Vector2.zero)
        {
            this.mSize = Vector2.one;
        }
    }

    public virtual Vector2 Size
    {
        get
        {
            return new Vector2(Mathf.Max(this.mSize.x, 0.001f), Mathf.Max(this.mSize.y, 0.001f));
        }
        set
        {
            this.SetNewSize(value, true);
        }
    }

    public bool SizeIsValid
    {
        get
        {
            return this.mSize != Vector2.zero;
        }
    }

    protected virtual void SetNewSize(Vector2 newSize, bool markForLayout = true)
    {
        if (this.mSize != newSize)
        {
            this.mSize = newSize;
            if (markForLayout)
            {
                this.MarkNeedsLayout();
            }
            this.SizeChanged();
        }
    }

    private void MarkNeedsLayout()
    {
        this.needsLayout = true;
    }

    public void MarkParentNeedsLayout()
    {
        UIElement uielement = (!(base.transform.parent != null)) ? null : base.transform.parent.GetComponent<UIElement>();
        if (uielement != null && uielement.layoutChildren)
        {
            uielement.needsLayout = true;
        }
    }

    public Vector2 LocalPosition
    {
        get
        {
            return base.transform.localPosition;
        }
        set
        {
            base.transform.localPosition = new Vector3(value.x, value.y, base.transform.localPosition.z);
        }
    }

    protected virtual void OnEnable()
    {
        this.MarkParentNeedsLayout();
    }

    protected virtual void OnDisable()
    {
        this.MarkParentNeedsLayout();
    }

    protected virtual void Awake()
    {
        this.lastSize = this.Size;
    }

    private void Start()
    {
        if (this.mSize == Vector2.zero)
        {
            this.GetDefaultSize();
        }
        this.MarkParentNeedsLayout();
    }

    private void LateUpdate()
    {
        if (this.needsLayout && !this.AnyParentsNeedLayout())
        {
            this.DoLayout();
        }
    }

    private bool AnyParentsNeedLayout()
    {
        UIElement uielement = this;
        while (uielement != null)
        {
            Transform parent = uielement.transform.parent;
            uielement = ((!(parent != null)) ? null : parent.GetComponent<UIElement>());
            if (uielement != null && uielement.needsLayout)
            {
                return true;
            }
        }
        return false;
    }

    public void SetSizeAndDoLayout(Vector2 newSize)
    {
        this.lastSize = this.Size;
        this.mSize = newSize;
        this.DoLayout();
        this.ForceSizeChangedOnAllChildren(base.gameObject);
    }

    private void ForceSizeChangedOnAllChildren(GameObject go)
    {
        UIElement element = go.GetElement();
        if (element != null)
        {
            element.SizeChanged();
        }
        IEnumerator enumerator = go.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                object obj = enumerator.Current;
                Transform transform = (Transform)obj;
                this.ForceSizeChangedOnAllChildren(transform.gameObject);
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

    public void DoLayout()
    {
        this.needsLayout = false;
        if (!this.layoutChildren)
        {
            return;
        }
        if (this.mSize == Vector2.zero)
        {
            return;
        }
        if (this.lastSize == Vector2.zero)
        {
            this.lastSize = this.mSize;
        }
        if (this.customLayout == null)
        {
            this.customLayout = base.GetComponent<UILayout>();
        }
        if (this.customLayout != null && this.customLayout.enabled)
        {
            this.customLayout.Layout();
        }
        else
        {
            this.DefaultLayout();
        }
        this.lastSize = this.Size;
    }

    public bool HasSizeFlag(UIAutoSizing flag)
    {
        return (this.autoSizing & flag) != (UIAutoSizing)0;
    }

    public void SetSizeFlag(UIAutoSizing flag, bool value)
    {
        if (value)
        {
            this.autoSizing |= flag;
        }
        else
        {
            this.autoSizing &= ~flag;
        }
    }

    public void SetSizeFlags(UIAutoSizing completeflags)
    {
        this.autoSizing = completeflags;
    }

    private void DefaultLayout()
    {
        Vector2 vector = this.mSize - this.lastSize;
        Vector2 vector2 = new Vector2(this.mSize.x / this.lastSize.x, this.mSize.y / this.lastSize.y);
        for (int i = 0; i < base.transform.childCount; i++)
        {
            Transform child = base.transform.GetChild(i);
            UIElement component = child.GetComponent<UIElement>();
            if (component != null)
            {
                float num = component.transform.localPosition.x - component.mSize.x * 0.5f - -this.lastSize.x * 0.5f;
                float num2 = this.lastSize.x * 0.5f - (component.transform.localPosition.x + component.mSize.x * 0.5f);
                float num3 = component.transform.localPosition.y - component.mSize.y * 0.5f - -this.lastSize.y * 0.5f;
                float num4 = this.lastSize.y * 0.5f - (component.transform.localPosition.y + component.mSize.y * 0.5f);
                if (component.HasSizeFlag(UIAutoSizing.FlexibleWidth))
                {
                    if (!component.HasSizeFlag(UIAutoSizing.LeftAnchor))
                    {
                        num *= vector2.x;
                    }
                    if (!component.HasSizeFlag(UIAutoSizing.RightAnchor))
                    {
                        num2 *= vector2.x;
                    }
                }
                else
                {
                    float num5 = (!component.HasSizeFlag(UIAutoSizing.LeftAnchor) && !component.HasSizeFlag(UIAutoSizing.RightAnchor)) ? 0.5f : 1f;
                    if (!component.HasSizeFlag(UIAutoSizing.LeftAnchor))
                    {
                        num += vector.x * num5;
                    }
                    if (!component.HasSizeFlag(UIAutoSizing.RightAnchor))
                    {
                        num2 += vector.x * num5;
                    }
                }
                if (component.HasSizeFlag(UIAutoSizing.FlexibleHeight))
                {
                    if (!component.HasSizeFlag(UIAutoSizing.TopAnchor))
                    {
                        num4 *= vector2.y;
                    }
                    if (!component.HasSizeFlag(UIAutoSizing.BottomAnchor))
                    {
                        num3 *= vector2.y;
                    }
                }
                else
                {
                    float num6 = (!component.HasSizeFlag(UIAutoSizing.TopAnchor) && !component.HasSizeFlag(UIAutoSizing.BottomAnchor)) ? 0.5f : 1f;
                    if (!component.HasSizeFlag(UIAutoSizing.TopAnchor))
                    {
                        num4 += vector.y * num6;
                    }
                    if (!component.HasSizeFlag(UIAutoSizing.BottomAnchor))
                    {
                        num3 += vector.y * num6;
                    }
                }
                float num7 = -this.mSize.x * 0.5f + num;
                float num8 = this.mSize.x * 0.5f - num2;
                float num9 = -this.mSize.y * 0.5f + num3;
                float num10 = this.mSize.y * 0.5f - num4;
                Vector3 localPosition = component.transform.localPosition;
                Vector2 newSize = component.mSize;
                newSize.x = num8 - num7;
                newSize.y = num10 - num9;
                localPosition.x = num7 + newSize.x * 0.5f;
                localPosition.y = num9 + newSize.y * 0.5f;
                component.transform.localPosition = localPosition;
                component.lastSize = component.mSize;
                component.SetNewSize(newSize, false);
                component.DoLayout();
            }
        }
    }

    public Rect GetRectInLocalPos()
    {
        Vector2 size = this.Size;
        Vector2 vector = (Vector2)base.transform.localPosition - size * 0.5f;
        return new Rect(vector.x, vector.y, size.x, size.y);
    }

    public Rect GetRectInWorldPos()
    {
        Vector2 size = this.Size;
        Vector2 vector = (Vector2)base.transform.position - size * 0.5f;
        return new Rect(vector.x, vector.y, size.x, size.y);
    }

    public void SetRectInLocalPos(Rect rect)
    {
        Vector3 localPosition = base.transform.localPosition;
        Vector2 size = this.Size;
        size.x = rect.width;
        size.y = rect.height;
        localPosition.x = rect.x + size.x * 0.5f;
        localPosition.y = rect.y + size.y * 0.5f;
        base.transform.localPosition = localPosition;
        this.mSize = size;
    }

    public Vector2 ScreenToContainerCoords(Vector2 screenPos)
    {
        UICamera uicamera = UICamera.FindCameraForLayer(base.gameObject.layer);
        Vector3 a = uicamera.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
        return a - base.transform.position;
    }

    public UIAutoSizing autoSizing;

    public bool layoutChildren = true;

    protected Vector2 lastSize;

    protected bool needsLayout;

    [HideInInspector]
    [SerializeField]
    private Vector2 mSize = Vector2.zero;

    private UILayout customLayout;

    private BoxCollider boxCollider;

    private UIWidget widget;
}
