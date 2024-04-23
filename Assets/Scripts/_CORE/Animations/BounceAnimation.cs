using System;
using UnityEngine;

public class BounceAnimation : MonoBehaviour
{
	private void Start()
	{
		if (this.inheritLocalPos)
		{
			this.orgPos = base.transform.localPosition;
		}
		if (this.randomPhase)
		{
			this.bouncesLeft += UnityEngine.Random.value;
		}
	}

	private void Update()
	{
		if (this.frequency <= 0f)
		{
			return;
		}
		if (this.delay > 0f)
		{
			this.delay -= Time.deltaTime;
			return;
		}
		if (this.bouncesLeft > 0f || this.endless)
		{
			float num = (!this.phasePerPos) ? 0f : (base.transform.position.y + base.transform.position.x);
			float num2 = Mathf.Cos(this.bouncesLeft * 2f * 3.14159274f + num) * -0.5f + 0.5f;
			num2 = Mathf.Pow(num2, this.bias);
			Vector3 point = new Vector3(num2 * this.axis.x, num2 * this.axis.y, num2 * this.axis.z);
			base.transform.localPosition = this.orgPos + base.transform.localRotation * point;
			this.bouncesLeft -= Time.deltaTime * this.frequency;
			if (this.bouncesLeft <= 0f && !this.endless)
			{
				base.transform.localPosition = this.orgPos;
				this.bouncesLeft = 0f;
			}
		}
	}

	public bool phasePerPos;

	public bool randomPhase;

	public bool inheritLocalPos = true;

	public float delay;

	public float bouncesLeft = 2f;

	public bool endless = true;

	public Vector3 axis = new Vector3(0f, 1f, 0f);

	public float frequency = 1f;

	public float bias = 1f;

	private float phase;

	private Vector3 orgPos = Vector3.zero;
}
