using System;
using System.Collections;
using Fibers;
using NinjaUI;
using UnityEngine;

public class ScheduledBoosterButtonSwitcher : MonoBehaviour
{
	public void ShowScheduledBooster(bool willShow)
	{
		if (willShow)
		{
			this.showFiber.Start(this.AnimateLabButton());
		}
		else
		{
			this.normalButtonPivot.SetActive(true);
			this.labButtonPivot.SetActive(false);
		}
	}

	private IEnumerator AnimateLabButton()
	{
		UICamera.DisableInput();
		this.labButtonPivot.SetActive(false);
		Vector3 initialScale = Vector3.one * 8f;
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		this.labButtonPivot.SetActive(true);
		yield return FiberAnimation.ScaleTransform(this.labButtonPivot.transform, initialScale, Vector3.one, null, 0.2f);
		this.normalButtonPivot.SetActive(false);
		yield return CameraShaker.ShakeDecreasing(0.3f, 10f, 20f, 0f, false);
		UICamera.EnableInput();
		yield break;
	}

	[SerializeField]
	private GameObject normalButtonPivot;

	[SerializeField]
	private GameObject labButtonPivot;

	private readonly Fiber showFiber = new Fiber();
}
