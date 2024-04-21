using System;
using UnityEngine;

public class SuperAimMonocular : MonoBehaviour
{
	private float Alpha
	{
		get
		{
			return this.quads[0].Alpha;
		}
		set
		{
			for (int i = 0; i < this.quads.Length; i++)
			{
				this.quads[i].Alpha = value;
			}
			this.ocular.gameObject.SetActive(value > 0.001f);
		}
	}

	public void Initialize(LevelSession levelSession, Transform boardRoot, UIElement visibleArea, int layer)
	{
		this.levelSession = levelSession;
		this.levelSession.Cannon.AimingStarted += this.AimingStarted;
		this.boardRoot = boardRoot;
		this.visibleArea = visibleArea;
		this.camera.cullingMask = 1 << layer;
		this.ocularZPosition = this.ocular.transform.position.z;
		this.cameraZPosition = this.cameraTransform.position.z;
		this.ocular.gameObject.SetActive(false);
		this.Alpha = 0f;
	}

	public void EndAndDestroySuperAim()
	{
		this.levelSession.Cannon.AimingStarted -= this.AimingStarted;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void AimingStarted(AimingState aimState)
	{
		this.isAiming = true;
		this.aim = aimState.Main;
		this.levelSession.Cannon.AimingEnded -= this.AimingEnded;
		this.levelSession.Cannon.AimingEnded += this.AimingEnded;
		this.ocular.gameObject.SetActive(false);
	}

	private void AimingEnded(AimingState aimState)
	{
		this.isAiming = false;
		this.levelSession.Cannon.AimingEnded -= this.AimingEnded;
		this.ocular.gameObject.SetActive(false);
		this.Alpha = 0f;
	}

	private void Update()
	{
		if (!this.isAiming)
		{
			return;
		}
		if (this.aim.collisionPoints.Count == 0)
		{
			return;
		}
		Vector3 position = this.visibleArea.transform.TransformPoint(new Vector3(0f, this.visibleArea.Size.y * 0.5f, 0f));
		position = this.boardRoot.InverseTransformPoint(position);
		Vector2 vector;
		bool flag = this.aim.TryFindIntersectionWithHorizontalLine(position.y, out vector);
		if (this.levelSession.Level.LevelAsset is BossLevel)
		{
			flag = false;
		}
		this.Alpha = Mathf.Lerp(this.Alpha, (!flag) ? 0f : 1f, 10f * Time.deltaTime);
		if (flag)
		{
			Vector2 v = this.aim.collisionPoints[this.aim.collisionPoints.Count - 1];
			Vector3 vector2 = this.boardRoot.TransformPoint(v);
			this.cameraTransform.position = new Vector3(vector2.x, vector2.y, this.cameraZPosition);
			Vector3 vector3 = this.boardRoot.TransformPoint(new Vector3(vector.x, vector.y, 0f));
			this.ocular.transform.position = new Vector3(vector3.x, vector3.y, this.ocularZPosition);
		}
	}

	[SerializeField]
	private Transform ocular;

	[SerializeField]
	private Camera camera;

	[SerializeField]
	private Transform cameraTransform;

	[SerializeField]
	private UIWidget[] quads;

	private LevelSession levelSession;

	private Transform boardRoot;

	private UIElement visibleArea;

	private bool isAiming;

	private AimInfo aim;

	private float ocularZPosition;

	private float cameraZPosition;

	private bool isCollisionPointInViewPrev;
}
