using System;
using System.Collections;
using UnityEngine;

public class BossCameraController
{
	public BossCameraController(Camera viewCamera, Vector3 topOfShipRailingPosition)
	{
		this.viewCamera = viewCamera;
		this.originalCamSize = this.viewCamera.orthographicSize;
		this.originalCamPos = this.viewCamera.transform.position;
		float levelIntroCameraSize = BossLevelDatabase.Database.levelIntroCameraSize;
		this.viewCamera.orthographicSize = levelIntroCameraSize;
		Vector3 position = this.originalCamPos;
		position.y = topOfShipRailingPosition.y + levelIntroCameraSize + BossLevelDatabase.Database.railingYOffset;
		viewCamera.transform.position = position;
	}

	public IEnumerator GetIntroZoomAndMovementAnimation()
	{
		Vector3 finalMoveCamStartPos = this.viewCamera.transform.position;
		Vector3 finalMoveCamEndPos = this.originalCamPos;
		IEnumerator enumerator = FiberAnimation.Animate(2.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			this.viewCamera.transform.position = Vector3.Lerp(finalMoveCamStartPos, finalMoveCamEndPos, t);
		}, false);
		IEnumerator enumerator2 = FiberAnimation.Animate(2.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			float levelIntroCameraSize = BossLevelDatabase.Database.levelIntroCameraSize;
			this.viewCamera.orthographicSize = Mathf.Lerp(levelIntroCameraSize, this.originalCamSize, t);
		}, false);
		return FiberHelper.RunParallel(new IEnumerator[]
		{
			enumerator,
			enumerator2
		});
	}

	private readonly Camera viewCamera;

	private readonly Vector3 originalCamPos;

	private readonly float originalCamSize;
}
