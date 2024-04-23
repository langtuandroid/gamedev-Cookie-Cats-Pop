using System;
using System.Collections;
using UnityEngine;

public class VipBox : MonoBehaviour
{
	public void Initialize()
	{
		this.closedPivot.SetActive(true);
		this.openPivot.SetActive(false);
	}

	public IEnumerator Open()
	{
		this.closedPivot.SetActive(false);
		this.openPivot.SetActive(true);
		this.lid.gameObject.SetActive(true);
		yield return FiberAnimation.MoveLocalTransform(this.lid, this.lidStart.localPosition, this.lidEnd.localPosition, this.lidAnimationCurve, 0.5f);
		this.lid.gameObject.SetActive(false);
		yield break;
	}

	public IEnumerator Close()
	{
		this.lid.gameObject.SetActive(true);
		yield return FiberAnimation.MoveLocalTransform(this.lid, this.lidEnd.localPosition, this.lidStart.localPosition, this.lidAnimationCurve, 0.5f);
		this.lid.gameObject.SetActive(false);
		this.closedPivot.SetActive(true);
		this.openPivot.SetActive(false);
		yield break;
	}

	public GameObject closedPivot;

	public GameObject openPivot;

	public Transform lid;

	public Transform lidStart;

	public Transform lidEnd;

	public AnimationCurve lidAnimationCurve;
}
