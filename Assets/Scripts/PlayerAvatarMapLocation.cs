using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Fibers;
using TactileModules.SagaCore;
using UnityEngine;

public class PlayerAvatarMapLocation : MonoBehaviour
{
	public MapAvatar PlayerAvatar
	{
		get
		{
			return this.avatarOnHud;
		}
	}

	public void Initialize(SagaAvatarController avatarController, UIScrollablePanel scrollPanel)
	{
		this.isVisible = false;
		this.avatarController = avatarController;
		this.scrollPanel = scrollPanel;
		avatarController.AvatarsChanged += this.AvatarControllerOnAvatarsChanged;
		this.AvatarControllerOnAvatarsChanged(avatarController);
	}

	private void AvatarControllerOnAvatarsChanged(SagaAvatarController avatarController)
	{
		if (this.avatarOnHud != null)
		{
			UnityEngine.Object.Destroy(this.avatarOnHud.gameObject);
		}
		MapAvatar meAvatar = avatarController.GetMeAvatar();
		if (meAvatar == null)
		{
			return;
		}
		this.avatarOnHud = UnityEngine.Object.Instantiate<MapAvatar>(meAvatar);
		this.avatarOnHud.transform.parent = base.transform;
		this.avatarOnHud.transform.position += Vector3.forward * -150f;
		this.avatarOnHud.gameObject.SetLayerRecursively(base.gameObject.layer);
		this.avatarOnHud.gameObject.SetActive(false);
		GameObject gameObject = new GameObject("button");
		gameObject.transform.parent = this.avatarOnHud.FramePivot;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.one;
		gameObject.SetLayerRecursively(this.avatarOnHud.gameObject.layer);
		BoxCollider boxCollider = gameObject.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = meAvatar.FramePivot.GetElementSize();
		Rigidbody rigidbody = gameObject.gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		UIButton uibutton = gameObject.gameObject.AddComponent<UIButton>();
		uibutton.receiver = base.gameObject;
		uibutton.methodName = "AvatarClicked";
		this.avatarOnHud.Initialize(avatarController, meAvatar.CloudUser, null);
		this.avatarTransformOnMap = meAvatar.transform;
	}

	public void UpdatePlayerAvatar()
	{
		this.avatarOnHud.UpdatePortrait();
		this.avatarOnHud.SetVIP(this.avatarController.IsPlayerVIP);
	}

	private void OnDestroy()
	{
		this.avatarController.AvatarsChanged -= this.AvatarControllerOnAvatarsChanged;
	}

	private void OnDisable()
	{
		this.animationFiber.Terminate();
		this.focusFiber.Terminate();
	}

	private void Update()
	{
		Vector3 vector = this.scrollPanel.transform.InverseTransformPoint(this.avatarTransformOnMap.TransformPoint(new Vector3(0f, -this.heightOfAvatar, 0f)));
		Vector3 vector2 = this.scrollPanel.transform.InverseTransformPoint(this.avatarTransformOnMap.TransformPoint(new Vector3(0f, this.heightOfAvatar, 0f)));
		bool visible = vector.y > this.scrollPanel.Size.y * 0.5f - this.topMargin || vector2.y < -this.scrollPanel.Size.y * 0.5f + this.bottomMargin;
		this.SetVisible(visible);
		if (this.isAnimatedIn)
		{
			Vector3 position = this.avatarOnHud.transform.position;
			Vector3 position2 = this.animateInTarget.position;
			this.avatarOnHud.transform.position = new Vector3(position2.x, position2.y, position.z);
		}
		this.animationFiber.Step();
		this.focusFiber.Step();
	}

	private void SetVisible(bool visible)
	{
		if (visible == this.isVisible)
		{
			return;
		}
		this.isVisible = visible;
		if (!this.focusFiber.IsTerminated)
		{
			return;
		}
		this.animationFiber.Terminate();
		this.animationFiber.Start(visible ? this.AnimateAvatarIn(this.movementDuration) : this.AnimateAvatarOut(this.movementDuration));
	}

	private IEnumerator AnimateAvatarIn(float duration)
	{
		this.avatarOnHud.gameObject.SetActive(true);
		this.avatarOnHud.SetFrameFromSide(MapAvatar.BackgroundSide.None);
		this.avatarOnHud.SetPivotPositionFromSide(MapAvatar.BackgroundSide.None);
		float time = 0f;
		for (;;)
		{
			float normalizedTime = Mathf.Clamp01(time / duration);
			float curvePosition = this.movementCurve.Evaluate(normalizedTime);
			Vector3 pos = this.avatarOnHud.transform.position;
			Vector3 pos2 = Vector3.Lerp(this.avatarTransformOnMap.position, this.animateInTarget.position, curvePosition);
			this.avatarOnHud.transform.position = new Vector3(pos2.x, pos2.y, pos.z);
			yield return null;
			if (time > duration)
			{
				break;
			}
			time += Time.deltaTime;
		}
		this.isAnimatedIn = true;
		yield break;
	}

	private IEnumerator AnimateAvatarOut(float duration)
	{
		this.avatarOnHud.gameObject.SetActive(true);
		MapAvatar.BackgroundSide newSize = this.avatarTransformOnMap.GetComponent<MapAvatar>().GetSideFromLocalPosition(this.avatarTransformOnMap.localPosition);
		this.avatarOnHud.SetFrameFromSide(newSize);
		this.avatarOnHud.SetPivotPositionFromSide(newSize);
		float time = 0f;
		for (;;)
		{
			float normalizedTime = Mathf.Clamp01(time / duration);
			float curvePosition = this.movementCurve.Evaluate(normalizedTime);
			Vector3 pos = this.avatarOnHud.transform.position;
			Vector3 pos2 = Vector3.Lerp(this.animateInTarget.position, this.avatarTransformOnMap.position, curvePosition);
			this.avatarOnHud.transform.position = new Vector3(pos2.x, pos2.y, pos.z);
			yield return null;
			if (time > duration)
			{
				break;
			}
			time += Time.deltaTime;
		}
		this.isAnimatedIn = false;
		this.avatarOnHud.gameObject.SetActive(false);
		yield break;
	}

	private IEnumerator AnimateAvatarSideways(float duration)
	{
		this.avatarOnHud.gameObject.SetActive(true);
		Vector3 target = this.animateOutTarget.position;
		target.z = this.avatarOnHud.transform.position.z;
		yield return FiberAnimation.MoveTransform(this.avatarOnHud.transform, this.avatarOnHud.transform.position, target, this.movementCurve, duration);
		this.avatarOnHud.gameObject.SetActive(false);
		this.isAnimatedIn = false;
		yield break;
	}

	private void AvatarClicked(UIEvent e)
	{
		if (!this.focusFiber.IsTerminated)
		{
			return;
		}
		this.animationFiber.Start(this.AnimateAvatarSideways(this.movementDuration));
		this.focusFiber.Start(this.FocusAvatar(1f, this.scrollPanel.actualScrollOffset.y, -this.avatarTransformOnMap.localPosition.y, this.movementCurve));
	}

	private IEnumerator FocusAvatar(float duration, float source, float dest, AnimationCurve curve)
	{
		UICamera.DisableInput();
		if (PlayerAvatarMapLocation._003C_003Ef__mg_0024cache0 == null)
		{
			PlayerAvatarMapLocation._003C_003Ef__mg_0024cache0 = new Fiber.OnExitHandler(UICamera.EnableInput);
		}
		yield return new Fiber.OnExit(PlayerAvatarMapLocation._003C_003Ef__mg_0024cache0);
		float time = 0f;
		if (this.scrollStarted != null)
		{
			this.scrollStarted(Mathf.Abs(source - dest));
		}
		for (;;)
		{
			float normalizedTime = Mathf.Clamp01(time / duration);
			normalizedTime = curve.Evaluate(normalizedTime);
			this.scrollPanel.SetScroll(Vector2.Lerp(new Vector2(0f, source), new Vector2(0f, dest), normalizedTime));
			yield return null;
			if (time > duration)
			{
				break;
			}
			time += Time.deltaTime;
		}
		if (this.scrollEnded != null)
		{
			this.scrollEnded();
		}
		yield break;
	}

	public Action<float> scrollStarted;

	public Action scrollEnded;

	[Tooltip("Offset to determine when the map avatar is out of the top of the screen")]
	[SerializeField]
	private float topMargin = 10f;

	[Tooltip("Offset to determine when the map avatar is out of the bottom of the screen")]
	[SerializeField]
	private float bottomMargin = 110f;

	[Tooltip("The height of the avatar")]
	[SerializeField]
	private float heightOfAvatar = 40f;

	[Tooltip("The onscreen position to animate the map avatar to")]
	[SerializeField]
	private Transform animateInTarget;

	[Tooltip("The offscreen position to animate the map avatar to")]
	[SerializeField]
	private Transform animateOutTarget;

	[Tooltip("The scroll panel the avatars should be children of")]
	[SerializeField]
	private AnimationCurve movementCurve;

	[Tooltip("The duration of the in and out animations")]
	[SerializeField]
	private float movementDuration = 0.75f;

	private bool isVisible;

	private bool isAnimatedIn;

	private SagaAvatarController avatarController;

	private UIScrollablePanel scrollPanel;

	private MapAvatar avatarOnHud;

	private Transform avatarTransformOnMap;

	private readonly Fiber animationFiber = new Fiber(FiberBucket.Manual);

	private readonly Fiber focusFiber = new Fiber(FiberBucket.Manual);

	[CompilerGenerated]
	private static Fiber.OnExitHandler _003C_003Ef__mg_0024cache0;
}
