using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.ComponentLifecycle;
using UnityEngine;

public class SideButtonsArea : LifecycleBroadcaster
{
	private UIElement AreaParentElement
	{
		get
		{
			if (this.cachedAreaParentElement == null)
			{
				this.cachedAreaParentElement = base.transform.GetComponent<UIElement>();
			}
			return this.cachedAreaParentElement;
		}
	}

	protected override void Start()
	{
		UIViewManager.Instance.OnScreenChanged += this.UpdateAllButtonPosition;
		base.Start();
	}

	protected override void OnDestroy()
	{
		UIViewManager.Instance.OnScreenChanged -= this.UpdateAllButtonPosition;
		this.Clear();
		base.OnDestroy();
	}

	public void Init()
	{
		this.updateButtonsFiber.Start(this.UpdateButtonsState());
	}

	public void InitButton(Transform buttonTransform, int side, Vector2 size, Func<object, bool> visibilityChecker)
	{
		this.InitButton(buttonTransform, side, size, null, visibilityChecker);
	}

	public void InitButton(Transform buttonTransform, int side, Vector2 size, object data, Func<object, bool> visibilityChecker)
	{
		this.AddButton(side, size, buttonTransform, data, visibilityChecker);
		this.UpdateButtonState(buttonTransform, visibilityChecker(data), false, null);
		this.UpdateAllButtonPosition();
	}

	public void InitButton(SideMapButton sideMapButton)
	{
		this.InitButton(sideMapButton.transform, (int)sideMapButton.Side, sideMapButton.Size, sideMapButton.Data, new Func<object, bool>(sideMapButton.VisibilityChecker));
	}

	public void Clear()
	{
		this.Stop();
		this.updateButtonsFiber.Terminate();
		this.animFibers.Clear();
		this.buttonDatas.Clear();
	}

	public void Stop()
	{
		foreach (KeyValuePair<SideButtonsArea.ButtonData, Fiber> keyValuePair in this.animFibers)
		{
			keyValuePair.Value.Terminate();
		}
	}

	public void Show(Transform buttonTransform, bool animate, bool isTicking)
	{
		this.SetTicking(buttonTransform, isTicking);
		this.UpdateButtonState(buttonTransform, true, animate, null);
	}

	public void Show(SideMapButton button, bool animate, bool isTicking)
	{
		this.Show(button.transform, animate, isTicking);
	}

	public void Hide(Transform buttonTransform, bool animate, bool isTicking)
	{
		this.SetTicking(buttonTransform, isTicking);
		this.UpdateButtonState(buttonTransform, false, animate, null);
	}

	public void Hide(SideMapButton button, bool animate, bool isTicking)
	{
		this.Hide(button.transform, animate, isTicking);
	}

	public IEnumerable<T> FindButtons<T>() where T : SideMapButton
	{
		foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> data in this.buttonDatas)
		{
			Transform trans = data.Key;
			T obj = trans.GetComponent<T>();
			if (obj != null)
			{
				yield return obj;
			}
		}
		yield break;
	}

	public T FindButton<T>() where T : MonoBehaviour
	{
		foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> keyValuePair in this.buttonDatas)
		{
			Transform key = keyValuePair.Key;
			T component = key.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
		}
		return (T)((object)null);
	}

	public bool IsButtonVisible(SideMapButton button)
	{
		if (this.buttonDatas.ContainsKey(button.transform))
		{
			SideButtonsArea.ButtonData buttonData = this.buttonDatas[button.transform];
			return buttonData.VisibilityChecker(buttonData.Data);
		}
		return false;
	}

	public bool IsButtonVisible<T>() where T : SideMapButton
	{
		T t = this.FindButton<T>();
		if (t != null)
		{
			SideButtonsArea.ButtonData buttonData = this.buttonDatas[t.transform];
			return buttonData.VisibilityChecker(buttonData.Data);
		}
		return false;
	}

	public void Destroy(List<GameObject> buttonObjects, bool animated)
	{
		foreach (GameObject buttonObject in buttonObjects)
		{
			this.Destroy(buttonObject, animated);
		}
	}

	public void Destroy(GameObject buttonObject, bool animated)
	{
		this.Destroy(buttonObject, animated, null);
	}

	public void Destroy(GameObject buttonObject, bool animated, Action callback)
	{
		SideButtonsArea.ButtonData data = this.buttonDatas[buttonObject.transform];
		if (data == null)
		{
			return;
		}
		data.IsTicking = false;
		if (buttonObject.activeSelf)
		{
			this.UpdateButtonState(buttonObject.transform, false, animated, delegate
			{
				this.DestroyInternal(buttonObject, data, callback);
			});
		}
		else
		{
			this.DestroyInternal(buttonObject, data, callback);
		}
	}

	private void DestroyInternal(GameObject buttonObject, SideButtonsArea.ButtonData data, Action callback)
	{
		this.animFibers[data].Terminate();
		this.animFibers.Remove(data);
		this.buttonDatas.Remove(buttonObject.transform);
		UnityEngine.Object.Destroy(buttonObject);
		this.UpdateAllButtonPosition();
		if (callback != null)
		{
			callback();
		}
	}

	public void AnimateAllButtonsIn()
	{
		foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> keyValuePair in this.buttonDatas)
		{
			this.AnimateIn(keyValuePair.Value.Transform, null);
		}
	}

	public void AnimateAllButtonsOut()
	{
		foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> keyValuePair in this.buttonDatas)
		{
			this.AnimateOut(keyValuePair.Value.Transform, null);
		}
	}

	private void SetTicking(Transform buttonTransform, bool isTicking)
	{
		SideButtonsArea.ButtonData buttonData = this.buttonDatas[buttonTransform];
		if (buttonData != null)
		{
			buttonData.IsTicking = isTicking;
		}
	}

	private void UpdateButtonState(Transform buttonTransform, bool newState, bool animate, Action callback = null)
	{
		SideMapButton component = buttonTransform.GetComponent<SideMapButton>();
		if (newState && !buttonTransform.gameObject.activeSelf)
		{
			buttonTransform.gameObject.SetActive(true);
			foreach (SideButtonsArea.ButtonData data in this.buttonDatas.Values)
			{
				this.PositionButton(data);
			}
			if (animate)
			{
				this.AnimateIn(buttonTransform, callback);
			}
			if (component != null)
			{
				component.OnButtonShown();
			}
		}
		else if (!newState && buttonTransform.gameObject.activeSelf && this.buttonDatas.ContainsKey(buttonTransform))
		{
			this.HideButton(buttonTransform, animate, callback);
			if (component != null)
			{
				component.OnButtonHidden();
			}
		}
	}

	private void UpdateAllButtonPosition()
	{
		foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> keyValuePair in this.buttonDatas)
		{
			this.PositionButton(keyValuePair.Value);
		}
	}

	private IEnumerator UpdateButtonsState()
	{
		for (;;)
		{
			foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> keyValuePair in this.buttonDatas)
			{
				if (keyValuePair.Value.IsTicking)
				{
					this.UpdateButtonState(keyValuePair.Key, keyValuePair.Value.VisibilityChecker(keyValuePair.Value.Data), true, null);
				}
			}
			yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		}
		yield break;
	}

	private void AddButton(int side, Vector2 size, Transform buttonTransform, object data, Func<object, bool> visibilityChecker)
	{
		if (this.buttonDatas.ContainsKey(buttonTransform))
		{
			return;
		}
		buttonTransform.SetParent(this.AreaParentElement.transform);
		buttonTransform.gameObject.SetLayerRecursively(this.AreaParentElement.gameObject.layer);
		SideButtonsArea.ButtonData buttonData = new SideButtonsArea.ButtonData(side, size, buttonTransform, data, visibilityChecker);
		this.buttonDatas.Add(buttonTransform, buttonData);
		this.animFibers.Add(buttonData, new Fiber());
	}

	private void HideButton(Transform buttonTransform, bool animateOut, Action callback)
	{
		if (animateOut)
		{
			this.AnimateOut(buttonTransform, delegate()
			{
				this.HideButtonInternal(buttonTransform, callback);
				this.UpdateAllButtonPosition();
			});
		}
		else
		{
			this.HideButtonInternal(buttonTransform, callback);
		}
	}

	private void HideButtonInternal(Transform buttonTransform, Action callback)
	{
		SideButtonsArea.ButtonData key = this.buttonDatas[buttonTransform];
		this.animFibers[key].Terminate();
		buttonTransform.gameObject.SetActive(false);
		if (callback != null)
		{
			callback();
		}
	}

	private void AnimateIn(Transform buttonTransform, Action callback)
	{
		SideButtonsArea.ButtonData buttonData = this.buttonDatas[buttonTransform];
		this.animFibers[buttonData].Start(this.AnimateIn(buttonData, callback));
	}

	private IEnumerator AnimateIn(SideButtonsArea.ButtonData data, Action callback)
	{
		Vector3 dest = data.LocalPos;
		Vector3 source = data.LocalPos;
		source.x += data.Size.x * 2f * (float)data.Side;
		yield return FiberAnimation.MoveLocalTransform(data.Transform, source, dest, this.animCurve, this.animTime);
		this.UpdateAllButtonPosition();
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	private void AnimateOut(Transform buttonTransform, Action callback)
	{
		SideButtonsArea.ButtonData key = this.buttonDatas[buttonTransform];
		this.animFibers[key].Start(this.AnimateOut(this.buttonDatas[buttonTransform], callback));
	}

	private IEnumerator AnimateOut(SideButtonsArea.ButtonData data, Action callback)
	{
		Vector3 dest = data.LocalPos;
		Vector3 source = data.Transform.localPosition;
		dest.x += data.Size.x * 2f * (float)data.Side;
		yield return FiberAnimation.MoveLocalTransform(data.Transform, source, dest, this.animCurve, this.animTime);
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	private void PositionButton(SideButtonsArea.ButtonData data)
	{
		if (this.AreaParentElement == null)
		{
			return;
		}
		float num = 0f;
		foreach (KeyValuePair<Transform, SideButtonsArea.ButtonData> keyValuePair in this.buttonDatas)
		{
			if (keyValuePair.Value.Transform == data.Transform)
			{
				break;
			}
			if (keyValuePair.Value.Side == data.Side && keyValuePair.Value.Transform.gameObject.activeSelf)
			{
				SideButtonsArea.ButtonData value = keyValuePair.Value;
				num += value.Size.y;
			}
		}
		Vector3 vector = default(Vector3);
		vector.x = (float)data.Side * this.AreaParentElement.Size.x * 0.5f;
		vector.y = -num - data.Size.y * 0.5f + this.AreaParentElement.Size.y * 0.5f;
		data.Transform.localPosition = vector;
		data.LocalPos = vector;
	}

	[SerializeField]
	private float animTime = 0.3f;

	[SerializeField]
	private AnimationCurve animCurve;

	private readonly Dictionary<Transform, SideButtonsArea.ButtonData> buttonDatas = new Dictionary<Transform, SideButtonsArea.ButtonData>();

	private readonly Dictionary<SideButtonsArea.ButtonData, Fiber> animFibers = new Dictionary<SideButtonsArea.ButtonData, Fiber>();

	private UIElement cachedAreaParentElement;

	private readonly Fiber updateButtonsFiber = new Fiber();

	private class ButtonData
	{
		public ButtonData(int side, Vector2 size, Transform transform, object data, Func<object, bool> visibilityChecker)
		{
			this.Side = side;
			this.Size = size;
			this.Transform = transform;
			this.Data = data;
			this.VisibilityChecker = visibilityChecker;
			this.IsTicking = true;
		}

		public int Side;

		public Vector2 Size;

		public readonly Transform Transform;

		public object Data;

		public readonly Func<object, bool> VisibilityChecker;

		public bool IsTicking;

		public Vector3 LocalPos;
	}
}
