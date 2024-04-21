using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	public bool BlockOtherCameras { get; set; }

	public static Camera CurrentCamera
	{
		get
		{
			return UICamera.currentCamera;
		}
	}

	[Obsolete("Use UICamera.currentTouchID instead")]
	public static int lastTouchID
	{
		get
		{
			return UICamera.currentTouchID;
		}
	}

	private bool handlesEvents
	{
		get
		{
			return UICamera.eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	public static GameObject hoveredObject
	{
		get
		{
			return UICamera.mHover;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return UICamera.mSel;
		}
		set
		{
			if (UICamera.mSel != value)
			{
				if (UICamera.mSel != null)
				{
					UICamera uicamera = UICamera.FindCameraForLayer(UICamera.mSel.layer);
					if (uicamera != null)
					{
						UICamera.currentCamera = uicamera.mCam;
						UICamera.mSel.SendMessage("OnSelect", false, SendMessageOptions.DontRequireReceiver);
						if (uicamera.useController || uicamera.useKeyboard)
						{
							UICamera.Highlight(UICamera.mSel, false);
						}
					}
				}
				UICamera.mSel = value;
				if (UICamera.mSel != null)
				{
					UICamera uicamera2 = UICamera.FindCameraForLayer(UICamera.mSel.layer);
					if (uicamera2 != null)
					{
						UICamera.currentCamera = uicamera2.mCam;
						if (uicamera2.useController || uicamera2.useKeyboard)
						{
							UICamera.Highlight(UICamera.mSel, true);
						}
						UICamera.mSel.SendMessage("OnSelect", true, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		UICamera.mHighlighted.Clear();
	}

	public static void DisableInput()
	{
		UICamera.inputDisabled++;
		UICamera.selectedObject = null;
	}

	public static bool InputDisabled
	{
		get
		{
			return UICamera.inputDisabled > 0;
		}
	}

	public static void EnableInput()
	{
		if (UICamera.inputDisabled > 0)
		{
			UICamera.inputDisabled--;
		}
	}

	public bool RaycastPastObject(Ray ray, GameObject pastObject, out RaycastHit hit)
	{
		int layerMask = this.eventReceiverMask;
		float maxDistance = (this.rangeDistance <= 0f) ? (base.GetComponent<Camera>().farClipPlane - base.GetComponent<Camera>().nearClipPlane) : this.rangeDistance;
		RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, layerMask);
		array.SortByDistance();
		bool flag = false;
		foreach (RaycastHit raycastHit in array)
		{
			if (flag)
			{
				hit = raycastHit;
				return true;
			}
			if (raycastHit.collider.gameObject == pastObject)
			{
				flag = true;
			}
		}
		hit = default(RaycastHit);
		return false;
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != null)) ? null : eventHandler.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < UICamera.mList.Count; i++)
			{
				UICamera uicamera = UICamera.mList[i];
				if (!(uicamera == null) && uicamera.enabled && uicamera.gameObject.activeInHierarchy)
				{
					return uicamera;
				}
			}
			return null;
		}
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos, ref RaycastHit hit)
	{
		for (int i = 0; i < UICamera.mList.Count; i++)
		{
			UICamera uicamera = UICamera.mList[i];
			if (uicamera.enabled && uicamera.gameObject.activeInHierarchy)
			{
				bool blockOtherCameras = uicamera.BlockOtherCameras;
				UICamera.currentCamera = uicamera.cachedCamera;
				Vector3 vector = UICamera.currentCamera.ScreenToViewportPoint(inPos);
				if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
				{
					if (blockOtherCameras)
					{
						return false;
					}
				}
				else
				{
					Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
					int layerMask = uicamera.eventReceiverMask;
					float maxDistance = (uicamera.rangeDistance <= 0f) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uicamera.rangeDistance;
					if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
					{
						return true;
					}
					if (blockOtherCameras)
					{
						return false;
					}
				}
			}
		}
		return false;
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < UICamera.mList.Count; i++)
		{
			UICamera uicamera = UICamera.mList[i];
			if (uicamera != null && (uicamera.eventReceiverMask & num) == num)
			{
				return uicamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (UnityEngine.Input.GetKeyDown(up))
		{
			return 1;
		}
		if (UnityEngine.Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (UnityEngine.Input.GetKeyDown(up0) || UnityEngine.Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (UnityEngine.Input.GetKeyDown(down0) || UnityEngine.Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (UICamera.mNextEvent < realtimeSinceStartup)
		{
			float axis2 = UnityEngine.Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				UICamera.mNextEvent = realtimeSinceStartup + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				UICamera.mNextEvent = realtimeSinceStartup + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static bool IsHighlighted(GameObject go)
	{
		int i = UICamera.mHighlighted.Count;
		while (i > 0)
		{
			UICamera.Highlighted highlighted = UICamera.mHighlighted[--i];
			if (highlighted.go == go)
			{
				return true;
			}
		}
		return false;
	}

	private static void Highlight(GameObject go, bool highlighted)
	{
		if (go != null)
		{
			int i = UICamera.mHighlighted.Count;
			while (i > 0)
			{
				UICamera.Highlighted highlighted2 = UICamera.mHighlighted[--i];
				if (highlighted2 == null || highlighted2.go == null)
				{
					UICamera.mHighlighted.RemoveAt(i);
				}
				else if (highlighted2.go == go)
				{
					if (highlighted)
					{
						highlighted2.counter++;
					}
					else if (--highlighted2.counter < 1)
					{
						UICamera.mHighlighted.Remove(highlighted2);
						go.SendMessage("OnHover", false, SendMessageOptions.DontRequireReceiver);
					}
					return;
				}
			}
			if (highlighted)
			{
				UICamera.Highlighted highlighted3 = new UICamera.Highlighted();
				highlighted3.go = go;
				highlighted3.counter = 1;
				UICamera.mHighlighted.Add(highlighted3);
				go.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private UICamera.MouseOrTouch GetTouch(int id)
	{
		UICamera.MouseOrTouch mouseOrTouch;
		if (!this.mTouches.TryGetValue(id, out mouseOrTouch))
		{
			mouseOrTouch = new UICamera.MouseOrTouch();
			this.mTouches.Add(id, mouseOrTouch);
		}
		return mouseOrTouch;
	}

	private void RemoveTouch(int id)
	{
		this.mTouches.Remove(id);
	}

	private void Awake()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			this.useKeyboard = false;
			this.useController = false;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			this.mIsEditor = true;
		}
		UICamera.mMouse[0].pos.x = UnityEngine.Input.mousePosition.x;
		UICamera.mMouse[0].pos.y = UnityEngine.Input.mousePosition.y;
		UICamera.lastTouchPosition = UICamera.mMouse[0].pos;
		UICamera.mList.Add(this);
		UICamera.SortCamerasByDepth();
		if (this.eventReceiverMask == -1)
		{
			this.eventReceiverMask = base.GetComponent<Camera>().cullingMask;
		}
	}

	private void OnDestroy()
	{
		if (this.mCam == this)
		{
			this.mCam = null;
		}
		UICamera.mList.Remove(this);
	}

	public static void SortCamerasByDepth()
	{
		List<UICamera> list = UICamera.mList;
		if (UICamera._003C_003Ef__mg_0024cache0 == null)
		{
			UICamera._003C_003Ef__mg_0024cache0 = new Comparison<UICamera>(UICamera.CompareFunc);
		}
		list.Sort(UICamera._003C_003Ef__mg_0024cache0);
	}

	private void FixedUpdate()
	{
		if (this.useMouse && Application.isPlaying && this.handlesEvents)
		{
			GameObject current = (!UICamera.Raycast(UnityEngine.Input.mousePosition, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject;
			for (int i = 0; i < 3; i++)
			{
				UICamera.mMouse[i].current = current;
			}
		}
	}

	private void Update()
	{
		if (UICamera.inputDisabled > 0)
		{
			return;
		}
		if (!Application.isPlaying || !this.handlesEvents)
		{
			return;
		}
		if (this.useMouse || (this.useTouch && this.mIsEditor))
		{
			this.ProcessMouse();
		}
		if (this.useTouch)
		{
			this.ProcessTouches();
		}
		if (this.useKeyboard && UICamera.mSel != null && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			UICamera.selectedObject = null;
		}
		if (UICamera.mSel != null)
		{
			string text = Input.inputString;
			if (this.useKeyboard && UnityEngine.Input.GetKeyDown(KeyCode.Delete))
			{
				text += "\b";
			}
			if (text.Length > 0)
			{
				if (this.mTooltip != null)
				{
					this.ShowTooltip(false);
				}
				UICamera.mSel.SendMessage("OnInput", text, SendMessageOptions.DontRequireReceiver);
			}
			this.ProcessOthers();
		}
		if (this.useMouse && UICamera.mHover != null)
		{
			float axis = UnityEngine.Input.GetAxis(this.scrollAxisName);
			if (axis != 0f)
			{
				UICamera.mHover.SendMessage("OnScroll", axis, SendMessageOptions.DontRequireReceiver);
			}
			if (this.mTooltipTime != 0f && this.mTooltipTime < Time.realtimeSinceStartup)
			{
				this.mTooltip = UICamera.mHover;
				this.ShowTooltip(true);
			}
		}
	}

	private void ProcessMouse()
	{
		bool flag = Time.timeScale < 0.9f;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					flag = true;
					break;
				}
			}
		}
		UICamera.mMouse[0].pos = UnityEngine.Input.mousePosition;
		UICamera.mMouse[0].delta = UICamera.mMouse[0].pos - UICamera.lastTouchPosition;
		bool flag2 = UICamera.mMouse[0].pos != UICamera.lastTouchPosition;
		UICamera.lastTouchPosition = UICamera.mMouse[0].pos;
		if (flag)
		{
			UICamera.mMouse[0].current = ((!UICamera.Raycast(UnityEngine.Input.mousePosition, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
		}
		for (int j = 1; j < 3; j++)
		{
			UICamera.mMouse[j].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[j].delta = UICamera.mMouse[0].delta;
			UICamera.mMouse[j].current = UICamera.mMouse[0].current;
		}
		bool flag3 = false;
		for (int k = 0; k < 3; k++)
		{
			if (Input.GetMouseButton(k))
			{
				flag3 = true;
				break;
			}
		}
		if (flag3)
		{
			this.mTooltipTime = 0f;
		}
		else if (flag2)
		{
			if (this.mTooltipTime != 0f)
			{
				this.mTooltipTime = Time.realtimeSinceStartup + this.tooltipDelay;
			}
			else if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
		}
		if (!flag3 && UICamera.mHover != null && UICamera.mHover != UICamera.mMouse[0].current)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.Highlight(UICamera.mHover, false);
			UICamera.mHover = null;
		}
		for (int l = 0; l < 3; l++)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(l);
			bool mouseButtonUp = Input.GetMouseButtonUp(l);
			UICamera.currentTouch = UICamera.mMouse[l];
			UICamera.currentTouchID = -1 - l;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
		}
		UICamera.currentTouch = null;
		if (!flag3 && UICamera.mHover != UICamera.mMouse[0].current)
		{
			this.mTooltipTime = Time.realtimeSinceStartup + this.tooltipDelay;
			UICamera.mHover = UICamera.mMouse[0].current;
			UICamera.Highlight(UICamera.mHover, true);
		}
	}

	private void ProcessTouches()
	{
		for (int i = 0; i < UnityEngine.Input.touchCount; i++)
		{
			Touch touch = UnityEngine.Input.GetTouch(i);
			UICamera.currentTouchID = touch.fingerId;
			UICamera.currentTouch = this.GetTouch(UICamera.currentTouchID);
			bool flag = touch.phase == TouchPhase.Began;
			bool flag2 = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
			if (flag)
			{
				UICamera.currentTouch.delta = Vector2.zero;
			}
			else
			{
				UICamera.currentTouch.delta = touch.position - UICamera.currentTouch.pos;
			}
			UICamera.currentTouch.pos = touch.position;
			UICamera.currentTouch.current = ((!UICamera.Raycast(UICamera.currentTouch.pos, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
			UICamera.lastTouchPosition = UICamera.currentTouch.pos;
			if (flag)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(flag, flag2);
			if (flag2)
			{
				this.RemoveTouch(UICamera.currentTouchID);
			}
			UICamera.currentTouch = null;
		}
	}

	private void ProcessOthers()
	{
		UICamera.currentTouchID = -100;
		UICamera.currentTouch = UICamera.mController;
		bool flag = false;
		bool flag2 = this.useKeyboard && (UnityEngine.Input.GetKeyDown(KeyCode.Return) || (!flag && UnityEngine.Input.GetKeyDown(KeyCode.Space)));
		bool flag3 = this.useController && UnityEngine.Input.GetKeyDown(KeyCode.JoystickButton0);
		bool flag4 = this.useKeyboard && (UnityEngine.Input.GetKeyUp(KeyCode.Return) || (!flag && UnityEngine.Input.GetKeyUp(KeyCode.Space)));
		bool flag5 = this.useController && UnityEngine.Input.GetKeyUp(KeyCode.JoystickButton0);
		bool flag6 = flag2 || flag3;
		bool flag7 = flag4 || flag5;
		if (flag6 || flag7)
		{
			UICamera.currentTouch.current = UICamera.mSel;
			this.ProcessTouch(flag6, flag7);
		}
		int num = 0;
		int num2 = 0;
		if (this.useKeyboard)
		{
			if (flag)
			{
				num += UICamera.GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				num2 += UICamera.GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				num += UICamera.GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				num2 += UICamera.GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}
		if (this.useController)
		{
			if (!string.IsNullOrEmpty(this.verticalAxisName))
			{
				num += UICamera.GetDirection(this.verticalAxisName);
			}
			if (!string.IsNullOrEmpty(this.horizontalAxisName))
			{
				num2 += UICamera.GetDirection(this.horizontalAxisName);
			}
		}
		if (num != 0)
		{
			UICamera.mSel.SendMessage("OnKey", (num <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow, SendMessageOptions.DontRequireReceiver);
		}
		if (num2 != 0)
		{
			UICamera.mSel.SendMessage("OnKey", (num2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow, SendMessageOptions.DontRequireReceiver);
		}
		if (this.useKeyboard && UnityEngine.Input.GetKeyDown(KeyCode.Tab))
		{
			UICamera.mSel.SendMessage("OnKey", KeyCode.Tab, SendMessageOptions.DontRequireReceiver);
		}
		if (this.useController && UnityEngine.Input.GetKeyUp(KeyCode.JoystickButton1))
		{
			UICamera.mSel.SendMessage("OnKey", KeyCode.Escape, SendMessageOptions.DontRequireReceiver);
		}
		UICamera.currentTouch = null;
	}

	private void ProcessTouch(bool pressed, bool unpressed)
	{
		if (pressed)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.Always;
			UICamera.currentTouch.totalDelta = Vector2.zero;
			if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentTouch.pressed.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
			}
			if (UICamera.currentTouch.pressed != UICamera.mSel)
			{
				if (this.mTooltip != null)
				{
					this.ShowTooltip(false);
				}
				UICamera.selectedObject = null;
			}
		}
		else if (UICamera.currentTouch.pressed != null && UICamera.currentTouch.delta.magnitude != 0f)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
			bool flag = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
			UICamera.currentTouch.pressed.SendMessage("OnDrag", UICamera.currentTouch.delta, SendMessageOptions.DontRequireReceiver);
			if (flag)
			{
				UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			}
			else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta)
			{
				float num = (UICamera.currentTouch != UICamera.mMouse[0]) ? Mathf.Max(this.touchClickThreshold, (float)Screen.height * 0.1f) : this.mouseClickThreshold;
				if (UICamera.currentTouch.totalDelta.magnitude > num)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
			}
		}
		if (unpressed)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentTouch.pressed.SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
				UICamera.currentTouch.pressed.SendMessage("OnReleaseAtPos", UICamera.currentTouch.pos, SendMessageOptions.DontRequireReceiver);
				if (UICamera.currentTouch.pressed == UICamera.mHover)
				{
					UICamera.currentTouch.pressed.SendMessage("OnHover", true, SendMessageOptions.DontRequireReceiver);
				}
				if (UICamera.currentTouch.pressed == UICamera.currentTouch.current)
				{
					if (UICamera.currentTouch.pressed != UICamera.mSel)
					{
						UICamera.mSel = UICamera.currentTouch.pressed;
						UICamera.currentTouch.pressed.SendMessage("OnSelect", true, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						UICamera.mSel = UICamera.currentTouch.pressed;
					}
					if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None)
					{
						float realtimeSinceStartup = Time.realtimeSinceStartup;
						UICamera.currentTouch.pressed.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
						if (UICamera.currentTouch.clickTime + 0.25f > realtimeSinceStartup)
						{
							UICamera.currentTouch.pressed.SendMessage("OnDoubleClick", SendMessageOptions.DontRequireReceiver);
						}
						UICamera.currentTouch.clickTime = realtimeSinceStartup;
					}
				}
				else if (UICamera.currentTouch.current != null)
				{
					UICamera.currentTouch.current.SendMessage("OnDrop", UICamera.currentTouch.pressed, SendMessageOptions.DontRequireReceiver);
				}
			}
			UICamera.currentTouch.pressed = null;
		}
	}

	private void ShowTooltip(bool val)
	{
		this.mTooltipTime = 0f;
		if (this.mTooltip != null)
		{
			this.mTooltip.SendMessage("OnTooltip", val, SendMessageOptions.DontRequireReceiver);
		}
		if (!val)
		{
			this.mTooltip = null;
		}
	}

	public static UICamera TopCamera
	{
		get
		{
			for (int i = 0; i < UICamera.mList.Count; i++)
			{
				UICamera uicamera = UICamera.mList[i];
				if (!(uicamera == null) && uicamera.enabled && uicamera.gameObject.activeInHierarchy)
				{
					return uicamera;
				}
			}
			return null;
		}
	}

	public static List<UICamera> AllCameras
	{
		get
		{
			return UICamera.mList;
		}
	}

	public bool useMouse = true;

	public bool useTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public LayerMask eventReceiverMask = -1;

	public float tooltipDelay = 1f;

	public float mouseClickThreshold = 10f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	private static int inputDisabled;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	private static Camera currentCamera = null;

	public static int currentTouchID = -1;

	public static UICamera.MouseOrTouch currentTouch = null;

	public static GameObject fallThrough;

	private static List<UICamera> mList = new List<UICamera>();

	private static List<UICamera.Highlighted> mHighlighted = new List<UICamera.Highlighted>();

	private static GameObject mSel = null;

	private static UICamera.MouseOrTouch[] mMouse = new UICamera.MouseOrTouch[]
	{
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch()
	};

	private static GameObject mHover;

	private static UICamera.MouseOrTouch mController = new UICamera.MouseOrTouch();

	private static float mNextEvent = 0f;

	private Dictionary<int, UICamera.MouseOrTouch> mTouches = new Dictionary<int, UICamera.MouseOrTouch>();

	private GameObject mTooltip;

	private Camera mCam;

	private LayerMask mLayerMask;

	private float mTooltipTime;

	private bool mIsEditor;

	[CompilerGenerated]
	private static Comparison<UICamera> _003C_003Ef__mg_0024cache0;

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject current;

		public GameObject pressed;

		public float clickTime;

		public UICamera.ClickNotification clickNotification = UICamera.ClickNotification.Always;
	}

	private class Highlighted
	{
		public GameObject go;

		public int counter;
	}
}
