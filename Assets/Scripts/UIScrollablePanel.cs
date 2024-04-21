using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
[RequireComponent(typeof(BoxCollider))]
[ExecuteInEditMode]
public class UIScrollablePanel : MonoBehaviour
{
	public Transform ScrollRoot { get; private set; }

	private Vector2 LowerLeft
	{
		get
		{
			return this.Size * -0.5f;
		}
	}

	private Vector2 UpperRight
	{
		get
		{
			return this.Size * 0.5f;
		}
	}

	public Rect AutoContentSizePadding { get; set; }

	protected bool IsDragging
	{
		get
		{
			return this.dragging;
		}
	}

	public Vector2 TotalContentSize { get; private set; }

	public bool IsAnimating
	{
		get
		{
			return this.scrollAnimationFiber != null || (this.scrollAnimationFiber != null && !this.scrollAnimationFiber.IsTerminated);
		}
	}

	public Vector2 Speed
	{
		get
		{
			return this.speed;
		}
	}

	protected virtual void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.cachedElement = this.GetElement();
		this.AutoContentSizePadding = new Rect(0f, 0f, 0f, 0f);
		this.ScrollRoot = new GameObject("ScrollRoot").transform;
		this.ScrollRoot.parent = base.transform;
		this.actualScrollOffset = new Vector2(-this.Size.x * 0.5f, -this.Size.y * 0.5f);
		UIElement uielement = this.ScrollRoot.gameObject.AddComponent<UIElement>();
		if (this.cachedElement.layoutChildren)
		{
			uielement.layoutChildren = true;
			uielement.autoSizing = (UIAutoSizing.LeftAnchor | UIAutoSizing.RightAnchor);
		}
		else
		{
			uielement.layoutChildren = false;
		}
	}

	public Vector2 Size
	{
		get
		{
			return this.cachedElement.Size;
		}
		set
		{
			this.cachedElement.Size = value;
		}
	}

	public void MoveChildrenToScrollRoot()
	{
		List<Transform> list = new List<Transform>();
		IEnumerator enumerator = base.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				if (!(transform == this.ScrollRoot))
				{
					list.Add(transform);
				}
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
		foreach (Transform transform2 in list)
		{
			transform2.parent = this.ScrollRoot;
			if (transform2.GetElement() != null)
			{
				this.SetElementVisibility(transform2.GetElement(), false);
			}
		}
		this.CalculateContentSize();
	}

	public void CalculateContentSize()
	{
		Vector2 b = Vector2.one * 9999f;
		Vector2 a = Vector2.one * -9999f;
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Vertical)
		{
			b.x = 0f;
			a.x = 0f;
		}
		if (this.limitAutoContentSize)
		{
			b.x = Mathf.Min(b.x, this.autoContentSizeLimits.x);
			b.y = Mathf.Min(b.y, this.autoContentSizeLimits.y);
			a.x = Mathf.Max(a.x, this.autoContentSizeLimits.width);
			a.y = Mathf.Max(a.y, this.autoContentSizeLimits.height);
		}
		else
		{
			IEnumerator enumerator = this.ScrollRoot.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					UIElement component = transform.GetComponent<UIElement>();
					if (component != null && component.enabled)
					{
						b.x = Mathf.Min(b.x, component.transform.localPosition.x - component.Size.x * 0.5f);
						b.y = Mathf.Min(b.y, component.transform.localPosition.y - component.Size.y * 0.5f);
						a.x = Mathf.Max(a.x, component.transform.localPosition.x + component.Size.x * 0.5f);
						a.y = Mathf.Max(a.y, component.transform.localPosition.y + component.Size.y * 0.5f);
					}
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
		float x = 0f - b.x + this.AutoContentSizePadding.x;
		float y = 0f - b.y + this.AutoContentSizePadding.y;
		IEnumerator enumerator2 = this.ScrollRoot.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object obj2 = enumerator2.Current;
				Transform transform2 = (Transform)obj2;
				transform2.localPosition += new Vector3(x, y, 0f);
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = (enumerator2 as IDisposable)) != null)
			{
				disposable2.Dispose();
			}
		}
		this.TotalContentSize = a - b;
		this.TotalContentSize += new Vector2(this.AutoContentSizePadding.x + this.AutoContentSizePadding.width, this.AutoContentSizePadding.y + this.AutoContentSizePadding.height);
		this.actualScrollOffset = this.Size * -0.5f;
		this.UpdateCellVisibility(false);
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.UpdateCellVisibility(false);
	}

	private void OnDisable()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.SetAllCellsAsInvisible();
		if (this.scrollAnimationFiber != null)
		{
			this.scrollAnimationFiber.Terminate();
		}
	}

	private void OnReleaseAtPos(Vector2 pos)
	{
		if (!this.dragging && this.speed.magnitude < 0.01f)
		{
			UICamera uicamera = UICamera.FindCameraForLayer(base.gameObject.layer);
			if (uicamera != null)
			{
				Ray ray = uicamera.GetComponent<Camera>().ScreenPointToRay(pos);
				RaycastHit raycastHit;
				if (uicamera.RaycastPastObject(ray, base.gameObject, out raycastHit))
				{
					raycastHit.collider.gameObject.SendMessage("OnClick", null, SendMessageOptions.DontRequireReceiver);
					if (this.OnContentClicked != null)
					{
						this.OnContentClicked(raycastHit.collider.gameObject);
					}
				}
			}
		}
		this.dragging = false;
	}

	protected virtual void OnPress(bool pressed)
	{
		if (pressed)
		{
			this.pressedTouch = UnityEngine.Input.mousePosition;
			if (!this.dragging)
			{
				this.speed = Vector2.zero;
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!this.dragging && (UnityEngine.Input.mousePosition - this.pressedTouch).magnitude > 15f)
		{
			this.scrollSpeed = 0f;
			this.dragging = true;
			this.speed = Vector2.zero;
			this.mLastPosition = this.cachedElement.ScreenToContainerCoords(UnityEngine.Input.mousePosition);
			this.mLastPositionGuarded = this.mLastPosition;
			this.mLastOffsetGuarded = this.actualScrollOffset;
		}
	}

	private void OnScroll(float delta)
	{
		if (this.isMouseScrollable)
		{
			this.dragging = false;
			this.scrollSpeed = delta * this.mouseScrollSpeed;
		}
	}

	protected virtual Rect GetOffsetBoundaries()
	{
		return this.cachedElement.GetRectInLocalPos();
	}

	protected virtual void DetectAndAdjustForEdges(ref Vector2 scrollOffset, out float? verticalEdge, out float? horizontalEdge)
	{
		verticalEdge = null;
		horizontalEdge = null;
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Vertical || this.scrollAxis == UIScrollablePanel.ScrollAxis.Both)
		{
			verticalEdge = this.CalculateVerticalEdge();
			if (verticalEdge != null)
			{
				scrollOffset.y += (verticalEdge.Value - scrollOffset.y) * Time.deltaTime * this.edgeSeekSpeed;
				this.speed.y = this.speed.y * this.edgeDamping;
			}
		}
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal || this.scrollAxis == UIScrollablePanel.ScrollAxis.Both)
		{
			horizontalEdge = this.CalculateHorizontalEdge();
			if (horizontalEdge != null)
			{
				scrollOffset.x += (horizontalEdge.Value - scrollOffset.x) * Time.deltaTime * this.edgeSeekSpeed;
				this.speed.x = this.speed.x * this.edgeDamping;
			}
		}
	}

	private float? CalculateVerticalEdge()
	{
		float? result = null;
		float num;
		float num2;
		if (this.startFromTop)
		{
			num = this.Size.y * 0.5f - this.TotalContentSize.y;
			num2 = Mathf.Max(num, -this.Size.y * 0.5f);
		}
		else
		{
			num2 = -this.Size.y * 0.5f;
			num = Mathf.Min(num2, -this.TotalContentSize.y + this.Size.y * 0.5f);
		}
		if (this.actualScrollOffset.y > num2)
		{
			result = new float?(num2);
		}
		else if (this.actualScrollOffset.y < num)
		{
			result = new float?(num);
		}
		return result;
	}

	private float? CalculateHorizontalEdge()
	{
		float? result = null;
		Rect offsetBoundaries = this.GetOffsetBoundaries();
		float xMin = offsetBoundaries.xMin;
		float num = -Mathf.Max(0f, this.TotalContentSize.x - offsetBoundaries.width) + offsetBoundaries.xMin;
		if (this.actualScrollOffset.x < num)
		{
			result = new float?(num);
		}
		else if (this.actualScrollOffset.x > xMin)
		{
			result = new float?(xMin);
		}
		return result;
	}

	protected virtual void ScrollLogic(ref Vector2 scrollOffset, float? verticalEdge, float? horizontalEdge)
	{
		if (this.dragging)
		{
			Vector2 a = this.cachedElement.ScreenToContainerCoords(UnityEngine.Input.mousePosition);
			Vector2 b = a - this.mLastPosition;
			this.mLastPosition = a;
			if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal)
			{
				b.y = 0f;
			}
			if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Vertical)
			{
				b.x = 0f;
			}
			this.speed.x = this.DragSpeedSmoothing(this.speed.x, b.x);
			this.speed.y = this.DragSpeedSmoothing(this.speed.y, b.y);
			scrollOffset += b;
			if (verticalEdge != null)
			{
				scrollOffset.y = this.mLastOffsetGuarded.y + (a.y - this.mLastPositionGuarded.y) * this.edgeElasticity;
			}
			if (horizontalEdge != null)
			{
				scrollOffset.x = this.mLastOffsetGuarded.x + (a.x - this.mLastPositionGuarded.x) * this.edgeElasticity;
			}
			if (verticalEdge == null && horizontalEdge == null)
			{
				this.mLastOffsetGuarded = scrollOffset;
				this.mLastPositionGuarded = a;
			}
		}
		else if (Math.Abs(this.scrollSpeed) > 0.001f)
		{
			Vector2 b2 = new Vector2(this.scrollSpeed, this.scrollSpeed);
			if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal)
			{
				b2.y = 0f;
			}
			if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Vertical)
			{
				b2.x = 0f;
			}
			this.speed.x = this.DragSpeedSmoothing(this.speed.x, b2.x);
			this.speed.y = this.DragSpeedSmoothing(this.speed.y, b2.y);
			scrollOffset += b2;
			this.scrollSpeed *= this.damping;
		}
		else
		{
			scrollOffset += this.speed;
			this.speed *= this.damping;
		}
	}

	private void Update()
	{
		this.OnUpdate();
	}

	protected virtual void OnUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Vector2 actualScrollOffset = this.actualScrollOffset;
		float? verticalEdge;
		float? horizontalEdge;
		this.DetectAndAdjustForEdges(ref actualScrollOffset, out verticalEdge, out horizontalEdge);
		this.ScrollLogic(ref actualScrollOffset, verticalEdge, horizontalEdge);
		if (actualScrollOffset != this.actualScrollOffset)
		{
			this.actualScrollOffset = actualScrollOffset;
			this.UpdateCellVisibility(false);
			this.Scrolling();
		}
	}

	private float DragSpeedSmoothing(float orgSpeed, float newSpeed)
	{
		if (orgSpeed > 0f)
		{
			if (newSpeed > orgSpeed)
			{
				return newSpeed;
			}
			return Mathf.Lerp(orgSpeed, newSpeed, 0.2f);
		}
		else
		{
			if (newSpeed < orgSpeed)
			{
				return newSpeed;
			}
			return Mathf.Lerp(orgSpeed, newSpeed, 0.2f);
		}
	}

	private void UIElementSizeChanged()
	{
		this.UpdateCellVisibility(false);
	}

	protected virtual void Scrolling()
	{
	}

	public void PrepareOptimization()
	{
		List<UIElement> list = new List<UIElement>();
		IEnumerator enumerator = this.ScrollRoot.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				UIElement component = transform.GetComponent<UIElement>();
				if (!(component == null))
				{
					list.Add(component);
				}
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
		this.optimizedChecker = new Overlap2DChecker<UIElement>(list, (UIElement e) => e.GetRectInLocalPos());
	}

	public void InvalidateOptimization()
	{
		this.optimizedChecker = null;
	}

	private void UpdateCellVisibilityOptimized()
	{
		Vector2 vector = -this.ScrollRoot.localPosition;
		Rect insideThisRect = new Rect(vector.x - this.Size.x * 0.5f, vector.y - this.Size.y * 0.5f, this.Size.x, this.Size.y);
		if (this.pixelPerfectCamera != null)
		{
			float num = this.pixelPerfectCamera.orthographicSize * 2f / (float)Screen.height;
			insideThisRect.width -= num;
			insideThisRect.height -= num;
		}
		this.optimizedChecker.Check(insideThisRect, new Action<UIElement, bool>(this.HandleElementVisibilityChange));
	}

	private void HandleElementVisibilityChange(UIElement element, bool isInside)
	{
		bool flag = this.ElementIsVisible(element);
		if (!isInside && flag)
		{
			this.SetElementVisibility(element, false);
		}
		else if (isInside && !flag)
		{
			this.SetElementVisibility(element, true);
		}
	}

	public void UpdateCellVisibility(bool forceAll = false)
	{
		if (this.disableCellVisibility)
		{
			return;
		}
		if (this.optimizedChecker != null && !forceAll)
		{
			this.UpdateCellVisibilityOptimized();
			return;
		}
		Vector2 vector = this.Size * 0.5f;
		IEnumerator enumerator = this.ScrollRoot.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				UIElement component = transform.GetComponent<UIElement>();
				if (!(component == null))
				{
					Vector3 vector2 = component.transform.position - base.transform.position;
					Vector2 vector3 = component.Size * 0.5f;
					float num = vector2.x - vector3.x;
					float num2 = vector2.x + vector3.x;
					float num3 = vector2.y + vector3.y;
					float num4 = vector2.y - vector3.y;
					bool flag = num < vector.x && num2 > -vector.x && num3 > -vector.y && num4 < vector.y;
					bool flag2 = this.ElementIsVisible(component);
					if (!flag && (flag2 || forceAll))
					{
						this.SetElementVisibility(component, false);
					}
					else if (flag && (!flag2 || forceAll))
					{
						this.SetElementVisibility(component, true);
					}
				}
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

	private UIScrollablePanel.IScrollVisibility GetInterface(UIElement e)
	{
		return e as UIScrollablePanel.IScrollVisibility;
	}

	private bool ElementIsVisible(UIElement e)
	{
		UIScrollablePanel.IScrollVisibility @interface = this.GetInterface(e);
		if (@interface != null)
		{
			return @interface.ElementIsVisible;
		}
		return !this.autoDisableHiddenElements || e.gameObject.activeSelf;
	}

	private void SetElementVisibility(UIElement e, bool value)
	{
		UIScrollablePanel.IScrollVisibility @interface = this.GetInterface(e);
		if (@interface != null)
		{
			@interface.ElementIsVisible = value;
		}
		else if (this.autoDisableHiddenElements)
		{
			e.gameObject.SetActive(value);
		}
		if (this.OnCellVisibilityChanged != null)
		{
			this.OnCellVisibilityChanged(e, value);
		}
	}

	public void SetContentOffset(Vector2 offset)
	{
		this.speed = Vector2.zero;
		Vector2 actualScrollOffset = this.LowerLeft - offset;
		this.actualScrollOffset = actualScrollOffset;
		this.UpdateCellVisibility(false);
	}

	public void ScrollRectToVisible(Rect rect)
	{
		Vector2 vector = rect.min + this.actualScrollOffset;
		Vector2 vector2 = rect.max + this.actualScrollOffset;
		Vector2 actualScrollOffset = this.actualScrollOffset;
		if (vector.x < this.LowerLeft.x)
		{
			actualScrollOffset.x += this.LowerLeft.x - vector.x;
		}
		else if (vector2.x > this.UpperRight.x)
		{
			actualScrollOffset.x += this.UpperRight.x - vector2.x;
		}
		this.actualScrollOffset = actualScrollOffset;
		this.UpdateCellVisibility(false);
	}

	public void JumpToTarget(Vector2 target)
	{
		this.SetScrollInternal(target, true);
	}

	public void SetScroll(Vector2 target)
	{
		this.SetScrollInternal(target, false);
	}

	private void SetScrollInternal(Vector2 target, bool isJump)
	{
		this.speed = Vector2.zero;
		this.actualScrollOffset = target;
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Vertical)
		{
			float? num = this.CalculateVerticalEdge();
			if (num != null)
			{
				target.y = num.Value;
			}
		}
		else
		{
			target.y = -this.TotalContentSize.y * 0.5f;
		}
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal)
		{
			float? num2 = this.CalculateHorizontalEdge();
			if (num2 != null)
			{
				target.x = num2.Value;
			}
		}
		else
		{
			target.x = -this.TotalContentSize.x * 0.5f;
		}
		this.actualScrollOffset = target;
		if (isJump)
		{
			this.optimizedChecker.StartJump();
			this.UpdateCellVisibility(false);
			this.optimizedChecker.EndJump(new Action<UIElement, bool>(this.HandleElementVisibilityChange));
		}
		else
		{
			this.UpdateCellVisibility(false);
		}
	}

	public void SetScrollAnimated(Vector2 target, float duration, AnimationCurve curve = null)
	{
		Vector2 actualScrollOffset = this.actualScrollOffset;
		this.scrollAnimationFiber = new Fiber();
		this.scrollAnimationFiber.Start(this.ScrollWithAnimation(actualScrollOffset, target, duration, curve));
	}

	public IEnumerator WaitForScrollAnimation()
	{
		while (this.IsAnimating)
		{
			yield return null;
		}
		yield break;
	}

	private IEnumerator ScrollWithAnimation(Vector2 startPosition, Vector2 endPosition, float duration, AnimationCurve curve = null)
	{
		float timeElapsed = 0f;
		while (timeElapsed < duration)
		{
			Vector2 target;
			if (curve == null)
			{
				target = Vector2.Lerp(startPosition, endPosition, timeElapsed / duration);
			}
			else
			{
				target = Vector2.Lerp(startPosition, endPosition, curve.Evaluate(timeElapsed / duration));
			}
			this.SetScroll(target);
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		this.SetScroll(endPosition);
		this.scrollAnimationFiber = null;
		yield break;
	}

	public Vector2 actualScrollOffset
	{
		get
		{
			return this.actualScrollOffset_;
		}
		protected set
		{
			if (this.ScrollRoot == null)
			{
				return;
			}
			this.actualScrollOffset_ = value;
			Vector2 v = (this.CustomScrollChecker != null) ? this.CustomScrollChecker(this.actualScrollOffset_) : this.actualScrollOffset_;
			if (this.pixelPerfectCamera != null)
			{
				float num = this.pixelPerfectCamera.orthographicSize * 2f / (float)Screen.height;
				v.x = (float)Mathf.FloorToInt(v.x / num) * num;
				v.y = (float)Mathf.FloorToInt(v.y / num) * num;
			}
			this.ScrollRoot.localPosition = v;
		}
	}

	public void SetTotalContentSize(float width, float height)
	{
		this.TotalContentSize = new Vector2(width, height);
		this.UpdateCellVisibility(true);
	}

	public void DestroyAllContent()
	{
		List<Transform> list = new List<Transform>();
		IEnumerator enumerator = this.ScrollRoot.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform item = (Transform)obj;
				list.Add(item);
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
		foreach (Transform transform in list)
		{
			transform.parent = null;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		this.TotalContentSize = Vector2.zero;
	}

	private void SetAllCellsAsInvisible()
	{
		if (this.ScrollRoot == null)
		{
			return;
		}
		IEnumerator enumerator = this.ScrollRoot.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				UIElement component = transform.GetComponent<UIElement>();
				if (!(component == null))
				{
					if (this.ElementIsVisible(component))
					{
						this.SetElementVisibility(component, false);
						if (this.OnCellDisappear != null)
						{
							this.OnCellDisappear(component);
						}
					}
				}
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

	public Action<UIElement, bool> OnCellVisibilityChanged;

	public Action<UIElement> OnCellDisappear;

	public Action<GameObject> OnContentClicked;

	public Material depthMaskMaterial;

	public UIScrollablePanel.ScrollAxis scrollAxis;

	public float edgeDamping = 0.5f;

	public float edgeSeekSpeed = 5f;

	public float damping = 0.98f;

	public float edgeElasticity = 0.5f;

	public bool startFromTop;

	public bool limitAutoContentSize;

	public Rect autoContentSizeLimits;

	public bool autoDisableHiddenElements = true;

	public bool disableCellVisibility;

	[SerializeField]
	protected bool isMouseScrollable;

	[SerializeField]
	protected float mouseScrollSpeed = 30f;

	public Camera pixelPerfectCamera;

	private Vector2 mLastPosition;

	private Vector2 mLastPositionGuarded;

	private Vector2 mLastOffsetGuarded;

	private Vector3 pressedTouch;

	private bool dragging;

	protected Vector2 speed;

	private UIElement cachedElement;

	private Overlap2DChecker<UIElement> optimizedChecker;

	private float scrollSpeed;

	public Func<Vector2, Vector2> CustomScrollChecker;

	protected Fiber scrollAnimationFiber;

	private Vector2 actualScrollOffset_;

	public enum ScrollAxis
	{
		Horizontal,
		Vertical,
		Both
	}

	public interface IScrollVisibility
	{
		bool ElementIsVisible { get; set; }
	}
}
