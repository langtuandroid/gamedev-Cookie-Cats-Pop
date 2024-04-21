using System;
using UnityEngine;

public class WaterBobbing : MonoBehaviour
{
	private void Start()
	{
		this.phase = UnityEngine.Random.value * 3.14159274f * 2f;
		this.orgPos = base.transform.localPosition;
		this.orgRot = base.transform.localRotation;
	}

	private void LateUpdate()
	{
		base.transform.localPosition = this.orgPos + Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad * this.speed + this.phase) * this.mag;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Cos(Time.timeSinceLevelLoad * this.rotSpeed + this.phase) * this.rotMag) * this.orgRot;
	}

	private Vector3 orgPos;

	private Quaternion orgRot;

	public float speed;

	public float mag;

	public float rotSpeed;

	public float rotMag;

	public float phase;
}
