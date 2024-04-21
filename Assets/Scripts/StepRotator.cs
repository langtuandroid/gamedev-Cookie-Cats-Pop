using System;
using UnityEngine;

public class StepRotator : MonoBehaviour
{
	private void Update()
	{
		this.zrotate += this.speed * Time.deltaTime;
		float num = this.zrotate;
		if (this.steps > 0)
		{
			float num2 = (float)(360 / this.steps);
			num = (float)Mathf.RoundToInt(num / num2) * num2;
		}
		base.transform.localRotation = Quaternion.Euler(0f, 0f, num);
	}

	public float speed = 180f;

	public int steps = 12;

	private float zrotate;
}
