using System;
using UnityEngine;

public class ScalePulsator : MonoBehaviour
{
	private void Start()
	{
		this.originalScale = base.transform.localScale;
		if (this.randomPhase)
		{
			this.elapsed = UnityEngine.Random.value * 6.28318548f;
		}
	}

	private void Update()
	{
		if (!this.pulsate)
		{
			base.transform.localScale = this.originalScale;
			return;
		}
		float num = Mathf.Sin(this.elapsed * this.speed * 3.14159274f * 2f) * 0.5f + 0.5f;
		this.elapsed += Time.deltaTime;
		num = Mathf.Pow(num, this.power);
		Vector3 b = new Vector3(1f, 1f, 1f) + this.scaleInfluence * num;
		base.transform.localScale = Vector3.Scale(this.originalScale, b);
	}

	public bool pulsate = true;

	public float speed = 10f;

	public float power = 1f;

	public bool randomPhase;

	public Vector3 scaleInfluence = new Vector3(0.2f, 0.2f, 0f);

	private float elapsed;

	private Vector3 originalScale;
}
